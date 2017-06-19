using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Client;

public class csMain : MonoBehaviour
{
    // 각 오브젝트의 최대 개수
    public class MAXCOUNT
    {
        public const int MAX_CLIENT = 5;
        public const int MAX_PUZZLE = 8;
        public const int MAX_TRAP = 3;
        public const int MAX_ROOMPACKET = 8;
    }
    
    // 임의로 사용할 서버 IP
    public class IPADDRESS
    {
        public const string GO_SERVER = "192.168.63.41";
        public const string HY_SERVER = "192.168.63.47";
        public const string DH_SERVER = "172.30.32.244";
    }

    // 플레이어 배열
    public static GameObject[] userArray = new GameObject[MAXCOUNT.MAX_CLIENT];
    // 퍼즐 배열
    public static int[] puzzleSetArray = new int[MAXCOUNT.MAX_PUZZLE];
    // 함정 배열
    public static int[] trapSetArray = new int[MAXCOUNT.MAX_TRAP];

    public static int killrecvID;       // 죽일 ID
    public static int puzzleCount;      // 남은 퍼즐 개수
    public static int playerCount;      // 남은 플레이어 수
    public static int traprecvID;       // 함정을 밟은 플레이어의 ID

    public static GameObject NetManager;

    public GameObject PlayerCamera;

    private int temp = 0;

    private void Start()
    {
        puzzleCount = MAXCOUNT.MAX_PUZZLE;
        playerCount = MAXCOUNT.MAX_CLIENT;

        NetManager = GameObject.Find("NetworkManager");

        // 퍼즐 상태 초기화
        for(int i=0; i<MAXCOUNT.MAX_PUZZLE; i++)
        {
            puzzleSetArray[i] = 0;
        }

        // 함정 상태 초기화
        for(int i=0; i<MAXCOUNT.MAX_TRAP; i++)
        {
            trapSetArray[i] = 1;
        }

        killrecvID = -1;
        playerCount = csMain.MAXCOUNT.MAX_CLIENT;

        // 플에이어 배열 생성
        if (csNetworkManager.TCPclient != null)
        {
            for (int i = 0; i < MAXCOUNT.MAX_CLIENT; i++)
            {
                // 자기자신에 해당하는 오브젝트 생성
                if (i == csNetworkManager.TCPclient.getMyID())
                {
                    userArray[i] = GameObject.Find("Player" + (i + 1));
                    userArray[i].GetComponent<csMove>().setCharacterController();
                    Destroy(GameObject.Find("Other" + (i + 1)));
                    PlayerCamera.transform.parent = userArray[i].transform;
                    PlayerCamera.transform.position = userArray[i].transform.position - Vector3.forward * 3.0f + Vector3.up * 1.6f;
                }
                // 다른 클라이언트에 해당하는 오브젝트 생성
                else
                {
                    userArray[i] = GameObject.Find("Other" + (i + 1));
                    Destroy(GameObject.Find("Player" + (i + 1)));
                }
                temp++;
            }
            // 남아있는 오브젝트 제거
            for (int i = temp; i < 5; i++)
            {
                Destroy(GameObject.Find("Player" + (i + 1)));
                Destroy(GameObject.Find("Other" + (i + 1)));
            }

            csRandPos.startPos = true;
        }
    }

    public void DisconnectNet()
    {
        NetManager.GetComponent<csNetworkManager>().DisConnect();
    }

    /*
    private void Update()
    {
        if (checkConnect && (csNetworkManager.UDPclient.getClients()[MAXCOUNT.MAX_CLIENT - 1].id == MAXCOUNT.MAX_CLIENT - 1))
        {
            for (int i = 0; i < MAXCOUNT.MAX_CLIENT; i++)
            {
                // 자기자신에 해당하는 오브젝트 생성
                if (i == csNetworkManager.TCPclient.getMyID())
                {
                    userArray[i] = GameObject.Find("Player" + (i + 1));
                    userArray[i].GetComponent<csMove>().setCharacterController();
                    Destroy(GameObject.Find("Other" + (i + 1)));
                    PlayerCamera.transform.parent = userArray[i].transform;
                    PlayerCamera.transform.position = userArray[i].transform.position - Vector3.forward * 3.0f + Vector3.up * 1.6f;
                }
                // 다른 클라이언트에 해당하는 오브젝트 생성
                else
                {
                    userArray[i] = GameObject.Find("Other" + (i + 1));
                    Destroy(GameObject.Find("Player" + (i + 1)));
                }
                temp++;
            }
            // 남아있는 오브젝트 제거
            for (int i = temp; i < 5; i++)
            {
                Destroy(GameObject.Find("Player" + (i + 1)));
                Destroy(GameObject.Find("Other" + (i + 1)));
            }
            checkConnect = false;
            csRandPos.startPos = true;
        }
    }
    */
}
