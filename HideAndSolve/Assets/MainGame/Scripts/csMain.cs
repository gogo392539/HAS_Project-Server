using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Client;

public class csMain : MonoBehaviour
{
    public class MAXCOUNT
    {
        public const int MAX_CLIENT = 2;
        public const int MAX_PUZZLE = 2;
        public const int MAX_TRAP = 2;
    }
    
    public class IPADDRESS
    {
        public const string GO_SERVER = "192.168.63.41";
        public const string HY_SERVER = "192.168.63.47";
        public const string DH_SERVER = "172.30.32.244";
    }

    public static GameObject[] userArray = new GameObject[MAXCOUNT.MAX_CLIENT];
    public static int[] puzzleSetArray = new int[MAXCOUNT.MAX_PUZZLE];
    public static int killrecvID;

    public static int puzzleCount;
    public static int playerCount;

    private bool checkConnect = true;

    private void Start()
    {
        puzzleCount = MAXCOUNT.MAX_PUZZLE;
        playerCount = MAXCOUNT.MAX_CLIENT;
        for(int i=0; i<MAXCOUNT.MAX_PUZZLE; i++)
        {
            puzzleSetArray[i] = 0;
        }
        killrecvID = -1;
    }
    private void Update()
    {
        if (checkConnect && csNetworkManager.UDPclient.getClients()[MAXCOUNT.MAX_CLIENT - 1].id == MAXCOUNT.MAX_CLIENT - 1)
        {
            int temp = 0;
            for (int i = 0; i < MAXCOUNT.MAX_CLIENT; i++)
            {
                if (i == csNetworkManager.TCPclient.getMyID())
                    userArray[i] = GameObject.Find("Player");
                else
                    userArray[i] = GameObject.Find("Other" + (++temp));
            }
            checkConnect = false;
            csRandPos.startPos = true;
        }
    }
}
