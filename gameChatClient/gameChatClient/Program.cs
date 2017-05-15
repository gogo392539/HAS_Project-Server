using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading;



namespace gameChatClient
{
    public class CONSTANTNUMBER
    {
        public const int CLIENT_MAX = 3;
        public const int DEFAULT_MESSAGA = 1024;
        public const int DEFAULT_PORT = 30000;
    }

    public class IPADDRESS
    {
        public const string GO_SERVER = "192.168.63.41";
        public const string HY_SERVER = "192.168.63.47";
        public const string DH_SERVER = "172.30.32.244";
        public const string LOCAL_SERVER = "127.0.0.1";
    }

    public class ByteAndString
    {
        // 바이트 배열을 String으로 변환 
        public static string ByteToString(byte[] strByte) {
            string str = Encoding.Default.GetString(strByte);
            return str;
        }
        // String을 바이트 배열로 변환 
        public static byte[] StringToByte(string str) {
            //byte[] StrByte = Encoding.UTF8.GetBytes(str);
            byte[] StrByte = Encoding.Default.GetBytes(str);
            return StrByte;
        }

       
    }

    public class ChatClient
    {
        private int myID;
        private Socket clientChatSock;
        private IPEndPoint chatIpep;

        private ThreadStart sendThreadStart;
        private ThreadStart recvThreadStart;
        private Thread sendThread;
        private Thread recvThread;

        public ChatClient()
        {
            myID = -1;
        }

        public void serverInit()
        {
            chatIpep = new IPEndPoint(IPAddress.Parse(IPADDRESS.LOCAL_SERVER), 30000);
            clientChatSock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            clientChatSock.Connect(chatIpep);
        }

        public void recvClientID()
        {
            Byte[] tempData = new Byte[sizeof(int)];
            clientChatSock.Receive(tempData);
            myID = BitConverter.ToInt32(tempData, 0);           //ID setting
        }

        public void ChatThreadStart()
        {
            sendThreadStart = new ThreadStart(this.ChatSendThread);
            sendThread = new Thread(sendThreadStart);
            sendThread.Start();

            recvThreadStart = new ThreadStart(this.ChatRecvThread);
            recvThread = new Thread(recvThreadStart);
            recvThread.Start();
        }

        public void ChatSendThread()
        {
            byte[] sendData = new byte[CONSTANTNUMBER.DEFAULT_MESSAGA];
            String str;

            while (true)
            {
                str = Console.ReadLine();
                //Console.WriteLine(str); //
                sendData = ByteAndString.StringToByte(str);
                //Console.WriteLine(sendData.Length); //
                clientChatSock.Send(sendData);
            }
        }

        public void ChatRecvThread()
        {
            byte[] recvData = new byte[CONSTANTNUMBER.DEFAULT_MESSAGA];
            string str;

            while (true)
            {
                clientChatSock.Receive(recvData);
                str = ByteAndString.ByteToString(recvData);
                Console.WriteLine(str);
            }
        }

        public void ThreadJoin()
        {
            sendThread.Join();
            recvThread.Join();
        }

        public void clientClosed()
        {
            clientChatSock.Close();
        }

    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("chatting Start");
            ChatClient client = new ChatClient();
            client.serverInit();
            client.recvClientID();
            client.ChatThreadStart();
            client.ThreadJoin();
            client.clientClosed();
        }
    }
}
