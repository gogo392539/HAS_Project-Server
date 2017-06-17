using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using Client;

public class csCatch : MonoBehaviour {

    public GameObject CatchSign;

    private void Update()
    {
        // 플레이어 수가 한명남으면 게임오버 화면으로 넘어감
        if (csMain.playerCount == 1)
        {
            GotoGameOver();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // 상대방을 잡을 수 있을 경우 잡을 수 있다는 정보 표시
        if (other.gameObject.tag == "other")
            CatchSign.SetActive(true);
    }

    private void OnTriggerStay(Collider other)
    {
        // Z키를 이용해 상대방을 잡음
        if(other.gameObject.tag == "other")
        {
            if(Input.GetKeyDown(KeyCode.Z))
            {
                CatchSign.SetActive(false);
                for (int i=0; i<csMain.MAXCOUNT.MAX_CLIENT; i++)
                {
                    if(csMain.userArray[i].name == other.gameObject.name)
                    {
                        csMain.userArray[i].GetComponentInChildren<SkinnedMeshRenderer>().enabled = false;
                        csMain.userArray[i].GetComponent<CapsuleCollider>().enabled = false;
                        csMain.userArray[i].GetComponent<BoxCollider>().enabled = false;
                        if(csNetworkManager.TCPclient != null)
                            csNetworkManager.TCPclient.KillPlayerSendFunc(i);
                        csMain.playerCount--;
                        break;
                    }
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        CatchSign.SetActive(false);
    }

    private void GotoGameOver()
    {
        SceneManager.LoadScene("uiGameOver", LoadSceneMode.Single);
        csMain.NetManager.GetComponent<csNetworkManager>().DisConnect();
    }
}
