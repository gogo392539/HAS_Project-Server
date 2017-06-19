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
                                // 2 -> 작동
    }

    public struct clientAnimation
    {
        public int id;
        public int aniSet;      // 0 -> 평상시 / 잡기 / 땅
                                // 1 -> 걷기 / 승리 / 하늘
                                // 2 -> 뛰기 / 패배
                                // 3 -> 점프 / 춤
                                // 4 -> 뒤로가기
    }

    public struct roomPacket
    {
        public int flag;        // 0 -> all client 접솓
                                // 1 -> client 접속
                                // 2 -> exit
                                // 3 -> room master exit
                                // 4 -> game start
        public int id;
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
        private clientAnimation[] aniNormal;
        private clientAnimation[] aniEvent;
        private clientAnimation[] groundCheck;

        public eventState()
        {
            puzzle = new Puzzle[csMain.MAXCOUNT.MAX_PUZZLE];
            trap = new Trap[csMain.MAXCOUNT.MAX_TRAP];
            aniNormal = new clientAnimation[csMain.MAXCOUNT.MAX_CLIENT];
            aniEvent = new clientAnimation[csMain.MAXCOUNT.MAX_CLIENT];
            groundCheck = new clientAnimation[csMain.MAXCOUNT.MAX_CLIENT];

            for (int i = 0; i < csMain.MAXCOUNT.MAX_PUZZLE; i++)
            {
                puzzle[i].id = i;
                puzzle[i].puzzleSet = -1;
            }

            for (int i = 0; i < csMain.MAXCOUNT.MAX_TRAP; i++)
            {
                trap[i].id = i;
                trap[i].trapSet = -1;
            }

            for (int i = 0; i < csMain.MAXCOUNT.MAX_CLIENT; i++)
            {
                aniNormal[i].id = i;
                aniNormal[i].aniSet = -1;
            }

            for (int i = 0; i < csMain.MAXCOUNT.MAX_CLIENT; i++)
            {
                aniEvent[i].id = i;
                aniEvent[i].aniSet = -1;
            }

            for (int i = 0; i < csMain.MAXCOUNT.MAX_CLIENT; i++)
            {
                groundCheck[i].id = i;
                groundCheck[i].aniSet = -1;
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

        public clientAnimation[] getAniNorm()
        {
            return aniNormal;
        }

        public clientAnimation[] getAniEvent()
        {
            return aniEvent;
        }

        public clientAnimation[] getGroundCheck()
        {
            return groundCheck;
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
            //Byte[] tempData = new Byte[4];
            //tempData = BitConverter.GetBytes(Clients[myID].id);
            //UDPclient.SendTo(tempData, tempData.Length, SocketFlags.None, serverIpep);
            Byte[] tempData = new Byte[28];
            tempData = byteAndStruct.StructureToByte(Clients[myID]);
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
            snedClientAddr();
            //Thread.Sleep(10);

            Byte[] UDPdata = new Byte[28];
            while (csNetworkManager.quit)
            {
                if (Clients[myID].id != -1)
                {
                    UDPdata = byteAndStruct.StructureToByte(Clients[myID]);
                    UDPclient.SendTo(UDPdata, UDPdata.Length, SocketFlags.None, serverIpep);
                    Debug.Log("send : " + Clients[myID].id + ", " + Clients[myID].pos.x);
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

                Debug.Log("recv : " + temp.id + ", " + temp.pos.x);

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

        private ThreadStart RecvThreadStart;
        private Thread RecvThread;

        private ThreadStart GameWaitThreadStart;
        private Thread GameWaitThread;
        private int GameStartSet;
        private int roomMasterExitSet;
        private int AllClientEnterSet;
        private int[] userEntranceList;

        private int taggerSet;

        private int myID;
        private int[] clientPosIndex;                   // client의 시작 위치를 설정하기 위한 변수

        private int pkID;
        private Trap[] trap;
        private clientAnimation[] aniNormal;
        private clientAnimation[] aniEvent;
        private clientAnimation[] groundCheck;
        private Puzzle[] puzzle;

        public TCPClient(Client_State[] clients, Puzzle[] TcpPuzzle, Trap[] TcpTrap, clientAnimation[] TcpAniNormal, clientAnimation[] TcpAniEvent, clientAnimation[] TcpGround)
        {
            this.clients = clients;
            clientPosIndex = new int[csMain.MAXCOUNT.MAX_CLIENT];

            puzzle = TcpPuzzle;
            trap = TcpTrap;
            aniNormal = TcpAniNormal;
            aniEvent = TcpAniEvent;
            groundCheck = TcpGround;
            pkID = -1;

            GameStartSet = -1;
            roomMasterExitSet = -1;

            userEntranceList = new int[csMain.MAXCOUNT.MAX_CLIENT];
            for (int i = 0; i < csMain.MAXCOUNT.MAX_CLIENT; i++)
            {
                userEntranceList[i] = -1;
            }
        }

        public void TCPServerInit()
        {
            TCPipep = new IPEndPoint(IPAddress.Parse(csMain.IPADDRESS.GO_SERVER), 36872);
            TCPclient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            TCPclient.Connect(TCPipep);
        }

        public void recvClientID()
        {
            Byte[] recvIDData = new Byte[sizeof(int)];
            Byte[] sendData = new Byte[csMain.MAXCOUNT.MAX_ROOMPACKET];
            Byte[] CLData = new Byte[sizeof(int) * userEntranceList.Length];
            roomPacket sendpacket;
            sendpacket.flag = -1;
            sendpacket.id = -1;

            TCPclient.Receive(recvIDData);
            myID = BitConverter.ToInt32(recvIDData, 0);     // byte[] to int32 
            clients[myID].id = myID;    //6.11 추가
            Debug.Log("ID : " + myID);

            TCPclient.Receive(CLData);
            for (int i = 0; i < userEntranceList.Length; i++)
            {
                //현재 접속중인 user의 상태 확인
                userEntranceList[i] = BitConverter.ToInt32(CLData, i * sizeof(int));
            }

            sendpacket.flag = 1;
            sendpacket.id = myID;
            sendData = byteAndStruct.StructureToByte(sendpacket);
            TCPclient.Send(sendData);
        }

        public void GameExit()
        {
            roomPacket exitPacket;
            exitPacket.id = myID;
            exitPacket.flag = 2;
            Byte[] exitData = new Byte[csMain.MAXCOUNT.MAX_ROOMPACKET];
            exitData = byteAndStruct.StructureToByte(exitPacket);
            TCPclient.Send(exitData);
        }

        public void GameStart()
        {
            if (myID == 0)
            {
                roomPacket startPacket;
                startPacket.id = myID;
                startPacket.flag = 3;
                Byte[] startData = new Byte[csMain.MAXCOUNT.MAX_ROOMPACKET];
                startData = byteAndStruct.StructureToByte(startPacket);
                TCPclient.Send(startData);
            }
        }

        public void GameWait()
        {
            GameWaitThreadStart = new ThreadStart(this.recvGameWait);
            GameWaitThread = new Thread(GameWaitThreadStart);
            GameWaitThread.Start();
        }

        public int[] getUserList()
        {
            return userEntranceList;
        }

        public void recvGameWait()
        {
            while (true)
            {
                byte[] sendData = new byte[csMain.MAXCOUNT.MAX_ROOMPACKET];
                byte[] recvData = new byte[csMain.MAXCOUNT.MAX_ROOMPACKET];

                roomPacket recvPacket;
                recvPacket.flag = -1;
                recvPacket.id = -1;
                TCPclient.Receive(recvData);
                recvPacket = (roomPacket)byteAndStruct.ByteToStructure(recvData, recvPacket.GetType());
                if (recvPacket.flag == 1)
                {
                    //client entrance
                    //roomPacket enterPacket;
                    userEntranceList[recvPacket.id] = 1;

                }
                else if (recvPacket.flag == 2)
                {
                    //client exit
                    //roomPacket exitPacket;
                    AllClientEnterSet = -1;
                    userEntranceList[recvPacket.id] = -1;

                }
                else if (recvPacket.flag == 3)
                {
                    //Start
                    roomPacket startPacket;
                    startPacket.flag = 4;
                    startPacket.id = myID;
                    sendData = byteAndStruct.StructureToByte(startPacket);
                    TCPclient.Send(sendData);
                    GameStartSet = 1;
                    break;
                }
                else if (recvPacket.flag == 9)
                {
                    //Room Master Exit
                    roomPacket RMExitPacket;
                    RMExitPacket.flag = 2;
                    RMExitPacket.id = myID;
                    sendData = byteAndStruct.StructureToByte(RMExitPacket);
                    TCPclient.Send(sendData);
                    roomMasterExitSet = 1;
                    break;
                }
                else if (recvPacket.flag == 10)
                {
                    //All client entrance
                    //roomPacket AllEnterPacket;
                    AllClientEnterSet = 1;

                }
            } // while end
        }

        public int getAllClientEnterSet()
        {
            return AllClientEnterSet;
        }

        public void setAllClientEnterSet(int set)
        {
            AllClientEnterSet = set;
        }

        public int getRoomMasterExitSet()
        {
            return roomMasterExitSet;
        }

        public int getGameStartSet()
        {
            return GameStartSet;
        }

        public void TCPThreadStart()
        {
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

        public void recvTaggerFunc()
        {
            Byte[] taggerData = new Byte[sizeof(int)];
            TCPclient.Receive(taggerData);
            taggerSet = BitConverter.ToInt32(taggerData, 0);
            Debug.Log("tagger : " + taggerSet);
        }

        public int getTaggerSet()
        {
            return taggerSet;
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
                    case 3:    //방해물 이벤트 셋팅    
                        if (eventpacket.eventSet >= 2)
                        {
                            csMain.trapSetArray[eventpacket.id] = 2;
                            csMain.traprecvID = eventpacket.eventSet - 2;
                        }
                        else if (eventpacket.eventSet == 1)
                        {
                            csMain.trapSetArray[eventpacket.id] = 1;
                        }
                        else if (eventpacket.eventSet == 0)
                        {
                            csMain.trapSetArray[eventpacket.id] = 0;
                        }                  
                        break;
                    case 4:     //플레이어 에니메이션 세팅                    
                        aniNormal[eventpacket.id].aniSet = eventpacket.eventSet;
                        break;
                    case 5:     //플레이어 에니메이션 세팅
                        aniEvent[eventpacket.id].aniSet = eventpacket.eventSet;
                        break;
                    case 6:     //플레이어 에니메이션 세팅
                        groundCheck[eventpacket.id].aniSet = eventpacket.eventSet;
                        break;
                }
            }
        }

        public void GroundCheckSendFunc(int groundSet)
        {
            groundCheck[myID].aniSet = groundSet;
            Byte[] aniData = new Byte[12];
            eventPacket aniPacket;
            aniPacket.flag = 6;
            aniPacket.id = myID;
            aniPacket.eventSet = groundSet;
            aniData = byteAndStruct.StructureToByte(aniPacket);
            TCPclient.Send(aniData);
        }

        public void OtherAnimSendFunc(int aniSet)
        {
            aniEvent[myID].aniSet = aniSet;
            Byte[] aniData = new Byte[12];
            eventPacket aniPacket;
            aniPacket.flag = 5;
            aniPacket.id = myID;
            aniPacket.eventSet = aniSet;
            aniData = byteAndStruct.StructureToByte(aniPacket);
            TCPclient.Send(aniData);
        }

        public void AnimationSendFunc(int aniSet)
        {
            aniNormal[myID].aniSet = aniSet;
            Byte[] aniData = new Byte[12];
            eventPacket aniPacket;
            aniPacket.flag = 4;
            aniPacket.id = myID;
            aniPacket.eventSet = aniSet;
            aniData = byteAndStruct.StructureToByte(aniPacket);
            TCPclient.Send(aniData);
        }

        public void TrapSendFunc(int trapID, int trapSet)
        {
            trap[trapID].trapSet = trapSet;
            Byte[] trapData = new Byte[12];
            eventPacket trapPacket;
            trapPacket.flag = 3;
            trapPacket.id = trapID;
            if (trapSet == 2)
                trapSet += myID;
            trapPacket.eventSet = trapSet;
            trapData = byteAndStruct.StructureToByte(trapPacket);
            TCPclient.Send(trapData);
        }

        public void KillPlayerSendFunc(int playerkillID)
        {
            pkID = playerkillID;
            Byte[] killData = new Byte[12];
            eventPacket killPacket;
            killPacket.flag = 2;
            killPacket.id = pkID;
            killPacket.eventSet = 0;
            killData = byteAndStruct.StructureToByte(killPacket);
            TCPclient.Send(killData);
        }

        public void PuzzleSendFunc(int puzzleID, int puzzleSet)
        {
            puzzle[puzzleID].puzzleSet = puzzleSet;
            Byte[] puzzleData = new Byte[12];
            eventPacket puzzlePacket;
            puzzlePacket.flag = 1;
            puzzlePacket.id = puzzleID;
            puzzlePacket.eventSet = puzzleSet;
            puzzleData = byteAndStruct.StructureToByte(puzzlePacket);
            TCPclient.Send(puzzleData);
        }

        public clientAnimation[] getGroundCheck()
        {
            return groundCheck;
        }

        public clientAnimation[] getAniEvent()
        {
            return aniEvent;
        }

        public clientAnimation[] getAniNormal()
        {
            return aniNormal;
        }

        public Puzzle[] getPuzzleState()
        {
            return puzzle;
        }

        public int getKillID()
        {
            return pkID;
        }

        public int[] getclientPosIndex()
        {
            return clientPosIndex;
        }

        public int getMyID()
        {
            return myID;
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

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            quit = true;

            Client_State[] Clients = new Client_State[csMain.MAXCOUNT.MAX_CLIENT];
            for (int i = 0; i < Clients.Length; i++)
            {
                //OtherClients 초기화
                Clients[i].id = -1;
            }

            eventState eventstate = new eventState();

            TCPclient = new TCPClient(Clients, eventstate.getPuzzle(), eventstate.getTrap(), eventstate.getAniNorm(), eventstate.getAniEvent(), eventstate.getGroundCheck());
            TCPclient.TCPServerInit();
            TCPclient.recvClientID();
            TCPclient.GameWait();

            UDPclient = new UDPClient(TCPclient.getMyID(), Clients);
            UDPclient.UDPServerInit();
        }

        public void GameSceneStart()
        {
            TCPclient.recvRandomIdx();
            TCPclient.recvTaggerFunc();
            UDPclient.UDPThreadStart();
            TCPclient.TCPThreadStart();
        }

        public void GameStart()
        {
            TCPclient.GameStart();
        }

        public void GameExit()
        {
            TCPclient.GameExit();
        }

        private void OnDisable()
        {
            DisConnect();
        }

        public void DisConnect()
        {
            quit = false;

            TCPclient.closedTCP();        //TCP close
            UDPclient.closedUDP();

            Destroy(gameObject);
        }
    }
}
