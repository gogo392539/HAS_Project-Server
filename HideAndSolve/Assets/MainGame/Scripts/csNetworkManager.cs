using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading;

using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace Client
{
    public class byteAndStruct
    {
        //byte 배열을 구조체로
        public static object ByteToStructure(Byte[] data, Type type)
        {
            IntPtr buff = Marshal.AllocHGlobal(data.Length);        // 배열의 크기만큼 비관리 메모리 영역에 메모리를 할당한다.
            Marshal.Copy(data, 0, buff, data.Length);               // 배열에 저장된 데이터를 위에서 할당한 메모리 영역에 복사한다.
            object obj = Marshal.PtrToStructure(buff, type);        // 복사된 데이터를 구조체 객체로 변환한다.
            Marshal.FreeHGlobal(buff);                              // 비관리 메모리 영역에 할당했던 메모리를 해제함

            if (Marshal.SizeOf(obj) != data.Length)  // (((PACKET_DATA)obj).TotalBytes != data.Length) // 구조체와 원래의 데이터의 크기 비교
            {
                return null;                                        // 크기가 다르면 null 리턴
            }
            return obj;                                             // 구조체 리턴
        }


        // 구조체를 byte 배열로
        public static Byte[] StructureToByte(object obj)
        {
            int datasize = Marshal.SizeOf(obj);                     //((PACKET_DATA)obj).TotalBytes; // 구조체에 할당된 메모리의 크기를 구한다.
            IntPtr buff = Marshal.AllocHGlobal(datasize);           // 비관리 메모리 영역에 구조체 크기만큼의 메모리를 할당한다.
            Marshal.StructureToPtr(obj, buff, false);               // 할당된 구조체 객체의 주소를 구한다.
            byte[] data = new byte[datasize];                       // 구조체가 복사될 배열
            Marshal.Copy(buff, data, 0, datasize);                  // 구조체 객체를 배열에 복사
            Marshal.FreeHGlobal(buff);                              // 비관리 메모리 영역에 할당했던 메모리를 해제함
            return data;                                            // 배열을 리턴
        }
    }

    public struct Pos
    {
        public float x;
        public float y;
        public float z;
        public float rotX;
        public float rotY;
        public float rotZ;
    }

    public struct Client_State
    {
        public int id;
        public Pos pos;
    }

    public struct Puzzle
    {
        public int id;
        public int puzzleSet;   // 0 -> 평상시
                                // 1 -> 푸는 중
                                // 2 -> 퍼즐 완료
    }

    public struct Trap
    {
        public int id;
        public int trapSet;     // 0 -> 비활성화
                                // 1 -> 활성화
    }

    public struct clientAnimation
    {
        public int id;
        public int aniSet;      // 0 -> 걷기
                                // 1 -> 뛰기
                                // 2 -> 점프
                                // 3 -> 뒤로가기
                                // 4 -> 잡기
                                // 5 -> 승리 모션
                                // 6 -> 패패 모션
                                // 7 -> 인사
                                // 8 -> 춤
    }

    public struct eventPacket
    {
        public int flag;        // 1 -> puzzle
                                // 2 -> PK
                                // 3 -> animation
                                // 4 -> trap
        public int id;
        public int eventSet;
    }

    public class eventState
    {
        private Puzzle[] puzzle;
        private Trap[] trap;
        private clientAnimation[] ani;
        private int pkID;

        public eventState()
        {
            puzzle = new Puzzle[csMain.MAXCOUNT.MAX_PUZZLE];
            trap = new Trap[csMain.MAXCOUNT.MAX_TRAP];
            ani = new clientAnimation[csMain.MAXCOUNT.MAX_CLIENT];
            pkID = -1;

            for(int i=0; i< csMain.MAXCOUNT.MAX_PUZZLE; i++)
            {
                puzzle[i].id = i;
                puzzle[i].puzzleSet = -1;
            }

            for(int i=0; i< csMain.MAXCOUNT.MAX_TRAP; i++)
            {
                trap[i].id = i;
                trap[i].trapSet = -1;
            }

            for(int i=0; i< csMain.MAXCOUNT.MAX_CLIENT; i++)
            {
                ani[i].id = i;
                ani[i].aniSet = -1;
            }
        }

        public Puzzle[] getPuzzle()
        {
            return puzzle;
        }

        public Trap[] getTrap()
        {
            return trap;
        }

        public clientAnimation[] getAni()
        {
            return ani;
        }
    }

    public class UDPClient
    {
        private IPEndPoint serverIpep;
        private IPEndPoint sender;
        private EndPoint remoteIpep;
        private Socket UDPclient;
        private int myID;
        private Client_State[] Clients;

        private ThreadStart UDPSendStart;
        private ThreadStart UDPRecvStart;
        private Thread sendThread;
        private Thread recvThread;

        public UDPClient(int id, Client_State[] clients)
        {
            myID = id;
            Clients = clients;
            Clients[myID].id = myID;
        }

        public void UDPServerInit()
        {
            serverIpep = new IPEndPoint(IPAddress.Parse(csMain.IPADDRESS.GO_SERVER), 36872);
            UDPclient = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            sender = new IPEndPoint(IPAddress.Any, 0);
            remoteIpep = (EndPoint)sender;
        }

        public void snedClientAddr()
        {
            Byte[] tempData = new Byte[4];
            tempData = BitConverter.GetBytes(Clients[myID].id);
            UDPclient.SendTo(tempData, tempData.Length, SocketFlags.None, serverIpep);
        }

        public void UDPThreadStart()
        {
            UDPSendStart = new ThreadStart(this.SendPosFunc);
            UDPRecvStart = new ThreadStart(this.RecvPosFunc);
            sendThread = new Thread(UDPSendStart);
            recvThread = new Thread(UDPRecvStart);
            sendThread.Start();
            recvThread.Start();
        }

        public void SendPosFunc()
        {
            Byte[] UDPdata = new Byte[28];
            while (csNetworkManager.quit)
            {
                if (Clients[myID].id != -1)
                {
                    UDPdata = byteAndStruct.StructureToByte(Clients[myID]);
                    UDPclient.SendTo(UDPdata, UDPdata.Length, SocketFlags.None, serverIpep);
                }                
                Thread.Sleep(10);
            } // while end
        }

        public void RecvPosFunc()
        {
            Byte[] tempUDPdata = new Byte[28];
            while (csNetworkManager.quit)
            {
                Client_State temp;
                temp.id = -1;
                temp.pos.x = -1;
                temp.pos.y = -1;
                temp.pos.z = -1;
                temp.pos.rotX = -1;
                temp.pos.rotY = -1;
                temp.pos.rotZ = -1;

                UDPclient.ReceiveFrom(tempUDPdata, SocketFlags.None, ref remoteIpep);             
                temp = (Client_State)byteAndStruct.ByteToStructure(tempUDPdata, temp.GetType());

                switch (temp.id)
                {
                    case 0:
                        Clients[0] = temp;
                        break;
                    case 1:
                        Clients[1] = temp;
                        break;
                    case 2:
                        Clients[2] = temp;
                        break;
                    case 3:
                        Clients[3] = temp;
                        break;
                    case 4:
                        Clients[4] = temp;
                        break;
                    default:
                        break;
                }
            } // while end
        }

        public void setPos(float x, float y, float z, float rotx, float roty, float rotz)
        {
            Clients[myID].pos.x = x;
            Clients[myID].pos.y = y;
            Clients[myID].pos.z = z;
            Clients[myID].pos.rotX = rotx;
            Clients[myID].pos.rotY = roty;
            Clients[myID].pos.rotZ = rotz;
        }

        public Client_State[] getClients()
        {
            return Clients;
        }

        public void threadJoin()
        {
            sendThread.Join();
            recvThread.Join();
        }

        public void closedUDP()
        {
            UDPclient.Close();
        }
    }

    public class TCPClient
    {
        private Socket TCPclient;
        private Client_State[] clients;
        private IPEndPoint TCPipep;

        private ThreadStart SendThreadStart;
        private Thread SendThread;
        private ThreadStart RecvThreadStart;
        private Thread RecvThread;

        private int myID;
        private int[] clientPosIndex;                   // client의 시작 위치를 설정하기 위한 변수

        private int pkID;
        private Trap[] trap;
        private clientAnimation[] ani;
        private Puzzle[] puzzle;

        public TCPClient(Client_State[] clients , Puzzle[] TcpPuzzle, Trap[] TcpTrap, clientAnimation[] TcpAni)
        {
            this.clients = clients;
            clientPosIndex = new int[csMain.MAXCOUNT.MAX_CLIENT];

            puzzle = TcpPuzzle;
            trap = TcpTrap;
            ani = TcpAni;
            pkID = -1;
        }

        public void TCPServerInit()
        {
            TCPipep = new IPEndPoint(IPAddress.Parse(csMain.IPADDRESS.GO_SERVER), 36872);
            TCPclient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            TCPclient.Connect(TCPipep);
        }

        public void recvClientID()
        {
            Byte[] tempTCPdata = new Byte[sizeof(int)];
            TCPclient.Receive(tempTCPdata);
            myID = BitConverter.ToInt32(tempTCPdata, 0);     // byte[] to int32 
        }

        public void TCPThreadStart()
        {
            SendThreadStart = new ThreadStart(this.TCPSendThreadFunc);
            SendThread = new Thread(SendThreadStart);
            SendThread.Start();

            RecvThreadStart = new ThreadStart(this.TCPRecvThreadFunc);
            RecvThread = new Thread(RecvThreadStart);
            RecvThread.Start();
        }

        public void recvRandomIdx()
        {
            Byte[] tempRandomPosData = new Byte[sizeof(int) * clientPosIndex.Length];
            TCPclient.Receive(tempRandomPosData);
            for (int i = 0; i < clientPosIndex.Length; i++)
            {
                clientPosIndex[i] = BitConverter.ToInt32(tempRandomPosData, i * sizeof(int));
            }
        }

        public void TCPSendThreadFunc()
        {
            eventPacket eventpacket;

            Byte[] eventData = new Byte[12];

            while (csNetworkManager.quit)
            {
                switch (csNetworkManager.flag)
                {
                    case 1:
                        for (int i = 0; i < csMain.MAXCOUNT.MAX_PUZZLE; i++)
                        {
                            switch (puzzle[i].puzzleSet)
                            {
                                case 2:
                                    eventpacket.eventSet = 2;
                                    eventpacket.flag = 1;
                                    eventpacket.id = puzzle[i].id;
                                    eventData = byteAndStruct.StructureToByte(eventpacket);
                                    TCPclient.Send(eventData);
                                    setPuzzle(i, -1);
                                    break;
                                case 1:
                                    eventpacket.eventSet = 1;
                                    eventpacket.flag = 1;
                                    eventpacket.id = puzzle[i].id;
                                    eventData = byteAndStruct.StructureToByte(eventpacket);
                                    TCPclient.Send(eventData);
                                    setPuzzle(i, -1);
                                    break;
                                case 0:
                                    eventpacket.eventSet = 0;
                                    eventpacket.flag = 1;
                                    eventpacket.id = puzzle[i].id;
                                    eventData = byteAndStruct.StructureToByte(eventpacket);
                                    TCPclient.Send(eventData);
                                    setPuzzle(i, -1);
                                    break;
                            }

                            //if (puzzle[i].puzzleSet == 2)
                            //{
                            //    eventpacket.eventSet = 2;
                            //    eventpacket.flag = 1;
                            //    eventpacket.id = puzzle[i].id;
                            //    eventData = byteAndStruct.StructureToByte(eventpacket);
                            //    TCPclient.Send(eventData);
                            //    setPuzzle(i, -1);
                            //    continue;
                            //}
                            //else if (puzzle[i].puzzleSet == 1)
                            //{
                            //    eventpacket.eventSet = 1;
                            //    eventpacket.flag = 1;
                            //    eventpacket.id = puzzle[i].id;
                            //    eventData = byteAndStruct.StructureToByte(eventpacket);
                            //    TCPclient.Send(eventData);
                            //    setPuzzle(i, -1);
                            //    continue;
                            //}
                            //else if (puzzle[i].puzzleSet == 0)
                            //{
                            //    eventpacket.eventSet = 0;
                            //    eventpacket.flag = 1;
                            //    eventpacket.id = puzzle[i].id;
                            //    eventData = byteAndStruct.StructureToByte(eventpacket);
                            //    TCPclient.Send(eventData);
                            //    setPuzzle(i, -1);
                            //    continue;
                            //}
                        }
                            break;
                    case 2:
                        eventpacket.flag = 2;
                        eventpacket.id = pkID;
                        eventpacket.eventSet = 0;
                        eventData = byteAndStruct.StructureToByte(eventpacket);
                        TCPclient.Send(eventData);
                        csNetworkManager.flag = -1;
                        pkID = -1;
                        break;
                    case 3:
                        eventpacket.flag = 3;
                        eventpacket.id = myID;
                        eventpacket.eventSet = ani[myID].aniSet;
                        eventData = byteAndStruct.StructureToByte(eventpacket);
                        TCPclient.Send(eventData);
                        csNetworkManager.flag = -1;
                        break;
                    case 4:
                        // trap event 처리를 위한 곳
                        break;
                }

                //if (csNetworkManager.flag == 1)
                //{
                //    for (int i = 0; i < csMain.MAXCOUNT.MAX_PUZZLE; i++)
                //    {
                //        if (puzzle[i].puzzleSet == 2)
                //        {
                //            eventpacket.eventSet = 2;
                //            eventpacket.flag = 1;
                //            eventpacket.id = puzzle[i].id;
                //            eventData = byteAndStruct.StructureToByte(eventpacket);
                //            TCPclient.Send(eventData);
                //            setPuzzle(i, -1);
                //            continue;
                //        }
                //        else if (puzzle[i].puzzleSet == 1)
                //        {
                //            eventpacket.eventSet = 1;
                //            eventpacket.flag = 1;
                //            eventpacket.id = puzzle[i].id;
                //            eventData = byteAndStruct.StructureToByte(eventpacket);
                //            TCPclient.Send(eventData);
                //            setPuzzle(i, -1);
                //            continue;
                //        }
                //        else if (puzzle[i].puzzleSet == 0)
                //        {
                //            eventpacket.eventSet = 0;
                //            eventpacket.flag = 1;
                //            eventpacket.id = puzzle[i].id;
                //            eventData = byteAndStruct.StructureToByte(eventpacket);
                //            TCPclient.Send(eventData);
                //            setPuzzle(i, -1);
                //            continue;
                //        }
                //    }
                //}
                //else if(csNetworkManager.flag == 2)
                //{
                //    eventpacket.flag = 2;
                //    eventpacket.id = pkID;
                //    eventpacket.eventSet = 0;
                //    eventData = byteAndStruct.StructureToByte(eventpacket);
                //    TCPclient.Send(eventData);
                //    csNetworkManager.flag = -1;
                //    pkID = -1;
                //}
                //else if(csNetworkManager.flag == 3)
                //{
                //    eventpacket.flag = 3;
                //    eventpacket.id = myID;
                //    eventpacket.eventSet = ani[myID].aniSet;
                //    eventData = byteAndStruct.StructureToByte(eventpacket);
                //    TCPclient.Send(eventData);
                //    csNetworkManager.flag = -1;
                //}
            }
        }

        public void TCPRecvThreadFunc()
        {
            Byte[] eventData = new Byte[12];
            eventPacket eventpacket;
            eventpacket.flag = -1;
            eventpacket.id = -1;
            eventpacket.eventSet = -1;

            while (csNetworkManager.quit)
            {
                TCPclient.Receive(eventData);
                eventpacket = (eventPacket)byteAndStruct.ByteToStructure(eventData, eventpacket.GetType());

                switch (eventpacket.flag)
                {
                    case 1:     //퍼즐 이벤트 세팅
                        switch (eventpacket.eventSet)
                        {
                            case 2:
                                csMain.puzzleSetArray[eventpacket.id] = 2;
                                break;
                            case 1:
                                csMain.puzzleSetArray[eventpacket.id] = 1;
                                break;
                            case 0:
                                csMain.puzzleSetArray[eventpacket.id] = 0;
                                break;
                        }
                        break;
                    case 2:     //잡기 이벤트 셋팅
                        csMain.killrecvID = eventpacket.id;
                        break;
                    case 3:     //플레이어 에니메이션 세팅
                        ani[eventpacket.id].aniSet = eventpacket.eventSet;
                        break;
                    case 4:     //방해물 이벤트 셋팅
                        //trap event 처리를 위한 부분
                        break;
                }

                //if (eventpacket.flag == 1)
                //{
                //    if (eventpacket.eventSet == 2)
                //    {
                //        csMain.puzzleSetArray[eventpacket.id] = 2;
                //    }
                //    else if (eventpacket.eventSet == 1)
                //    {
                //        csMain.puzzleSetArray[eventpacket.id] = 1;
                //    }
                //    else if (eventpacket.eventSet == 0)
                //    {
                //        csMain.puzzleSetArray[eventpacket.id] = 0;
                //    }
                //}
                //else if(eventpacket.flag == 2)
                //{
                //    csMain.killrecvID = eventpacket.id;
                //}
                //else if (eventpacket.flag == 3)
                //{
                //    ani[eventpacket.id].aniSet = eventpacket.eventSet;
                //}
            }
        }

        public clientAnimation[] getAni()
        {
            return ani;
        }

        public void setAni(int aniSet)
        {
            ani[myID].aniSet = aniSet;
        }

        public Puzzle[] getPuzzleState()
        {
            return puzzle;
        }

        public void setPuzzle(int id, int state)
        {
            csNetworkManager.flag = 1;
            puzzle[id].puzzleSet = state;
        }

        public int getKillID()
        {
            return pkID;
        }

        public void setKillID(int id)
        {
            pkID = id;
            if (pkID == -1)
            {
                csNetworkManager.flag = 0;          //event 발생 X
            }
            else
            {
                csNetworkManager.flag = 2;
            }
        }

        public int[] getclientPosIndex()
        {
            return clientPosIndex;
        }

        public int getMyID()
        {
            return myID;
        }

        public void threadJoin()
        {
            SendThread.Join();
            RecvThread.Join();
        }

        public void closedTCP()
        {
            TCPclient.Close();
        }
    }

    public class csNetworkManager : MonoBehaviour
    {
        static public int flag;
        static public bool quit;

        static public TCPClient TCPclient;
        static public UDPClient UDPclient;

        private void Start()
        {
            quit = true;

            Client_State[] Clients = new Client_State[csMain.MAXCOUNT.MAX_CLIENT];
            for (int i = 0; i < Clients.Length; i++)
            {
                //OtherClients 초기화
                Clients[i].id = -1;
                Clients[i].pos.x = -1;
                Clients[i].pos.y = -1;
                Clients[i].pos.z = -1;
                Clients[i].pos.rotX = -1;
                Clients[i].pos.rotY = -1;
                Clients[i].pos.rotZ = -1;
            }

            eventState eventstate = new eventState();

            TCPclient = new TCPClient(Clients, eventstate.getPuzzle(), eventstate.getTrap(), eventstate.getAni());
            TCPclient.TCPServerInit();
            TCPclient.recvClientID();

            UDPclient = new UDPClient(TCPclient.getMyID(), Clients);
            UDPclient.UDPServerInit();
            UDPclient.snedClientAddr();

            TCPclient.recvRandomIdx();

            UDPclient.UDPThreadStart();

            TCPclient.TCPThreadStart();
        }

        private void OnDisable()
        {
            quit = false;

            TCPclient.closedTCP();        //TCP close
            UDPclient.closedUDP();
        }
    }
}
