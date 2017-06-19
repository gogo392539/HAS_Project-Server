using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using Client;

public class csKillPlayer : MonoBehaviour {

	void Update () {
        // 죽일 플레이어의 ID를 통해 상대방을 죽임
        if (csMain.killrecvID != -1)
        {
            csMain.userArray[csMain.killrecvID].GetComponentInChildren<SkinnedMeshRenderer>().enabled = false;
            csMain.userArray[csMain.killrecvID].GetComponent<CapsuleCollider>().enabled = false;
            if(csNetworkManager.TCPclient != null && csMain.killrecvID != csNetworkManager.TCPclient.getMyID())
                csMain.userArray[csMain.killrecvID].GetComponent<BoxCollider>().enabled = false;
            csMain.killrecvID = -1;
            csMain.playerCount--;
        }

        // 플레이어 수가 한명남으면 게임오버 화면으로 넘어감
        if(csMain.playerCount == 1)
        {
            GotoGameOver();
        }
	}

    private void GotoGameOver()
    {
        SceneManager.LoadScene("uiGameOver", LoadSceneMode.Single);
        csMain.NetManager.GetComponent<csNetworkManager>().DisConnect();
    }
}
