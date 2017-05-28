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

    private bool checkConnect;
    private bool startSet;

    private void Start()
    {
        checkConnect = true;
        startSet = true;
        //btNext.interactable = false;
    }

    private void Update()
    {
        if (checkConnect && csNetworkManager.UDPclient.getClients()[MAX_CLIENT - 1].id == MAX_CLIENT - 1)
        {
            checkConnect = false;
            //btNext.interactable = true;
        }

        if(startSet && (csNetworkManager.TCPclient.getGameStartSet() == 1))
        {
            GameSceneStart();
            startSet = false;
        }
    }

    public void GameSceneStart()
    {
        netManager.GetComponent<csNetworkManager>().GameSceneStart();
        SceneManager.LoadScene("GameScene", LoadSceneMode.Single);
    }

    public void GameStart()
    {
        if (csNetworkManager.TCPclient.getMyID() == 0)
        {
            netManager.GetComponent<csNetworkManager>().GameStart();
        }
    }

    public void GameReady()
    {
        netManager.GetComponent<csNetworkManager>().GameReady();
    }

    public void GameExit()
    {
        netManager.GetComponent<csNetworkManager>().GameExit();
    }

    public void initialAndThread()
    {
        netManager.GetComponent<csNetworkManager>().InitialAndThread();
    }

    public void disConnect()
    {
        netManager.GetComponent<csNetworkManager>().DisConnect();
    }
}
