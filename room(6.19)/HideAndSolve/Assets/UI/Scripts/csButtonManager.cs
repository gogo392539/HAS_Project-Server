using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using UnityEngine.SceneManagement;

using Client;

public class csButtonManager : MonoBehaviour
{
    public const int MAX_CLIENT = 2;

    public GameObject netManager;
    public Button btNext;
    public Button btPrev;

    public Text[] Users = new Text[5];

    public Text Loading;

    private bool checkConnect;
    private bool startSet;
    private bool exitSet;

    private void Start()
    {
        checkConnect = true;
        startSet = true;
        exitSet = true;
        btNext.interactable = false;
    }

    private void Update()
    {
        UserEntranceSet();

        if (checkConnect && csNetworkManager.TCPclient.getAllClientEnterSet() == 1)
        {
            btNext.interactable = true;
            csNetworkManager.TCPclient.setAllClientEnterSet(0);
        }

        if (checkConnect && csNetworkManager.TCPclient.getAllClientEnterSet() == -1)
        {
            btNext.interactable = false;
            csNetworkManager.TCPclient.setAllClientEnterSet(0);
        }

        if (startSet && (csNetworkManager.TCPclient.getGameStartSet() == 1))
        {
            GameSceneStart();
            startSet = false;
        }

        if (exitSet && (csNetworkManager.TCPclient.getRoomMasterExitSet() == 1))
        {
            //room master가 방에서 나갔을 때 다른 client에서 발동
            RMGameExit();
            exitSet = false;
        }
    }

    public void UserEntranceSet()
    {
        for (int i = 0; i < csMain.MAXCOUNT.MAX_CLIENT; i++)
        {
            if (csNetworkManager.TCPclient.getUserList()[i] == 1)
            {
                Users[i].text = "User" + i;
            }
            else
            {
                Users[i].text = "...";
            }
        }
    }

    public void GameStart()
    {
        netManager.GetComponent<csNetworkManager>().GameStart();
    }

    public void GameExit()
    {
        netManager.GetComponent<csNetworkManager>().GameExit();
        SceneManager.LoadScene("uiMain", LoadSceneMode.Single);
        netManager.GetComponent<csNetworkManager>().DisConnect();
    }

    public void RMGameExit()
    {
        SceneManager.LoadScene("uiMain", LoadSceneMode.Single);
        netManager.GetComponent<csNetworkManager>().DisConnect();
    }

    public void GameSceneStart()
    {
        netManager.GetComponent<csNetworkManager>().GameSceneStart();
        SceneManager.LoadScene("GameScene", LoadSceneMode.Single);
    }

    public void disConnect()
    {
        netManager.GetComponent<csNetworkManager>().DisConnect();
    }
}
