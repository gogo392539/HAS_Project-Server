using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Client;
public class csCatch : MonoBehaviour {

    public GameObject CatchSign;

    private void OnTriggerEnter(Collider other)
    {
        // 상대방을 잡을 수 있을 경우 잡을 수 있다는 정보 표시
        if (other.gameObject.tag == "otherPlayer")
            CatchSign.SetActive(true);
    }

    private void OnTriggerStay(Collider other)
    {
        // Z키를 이용해 상대방을 잡음
        if(other.gameObject.tag == "otherPlayer")
        {
            if(Input.GetKeyDown(KeyCode.Z))
            {
                CatchSign.SetActive(false);
                for (int i=0; i<csMain.MAXCOUNT.MAX_CLIENT; i++)
                {
                    if(csMain.userArray[i].name == other.gameObject.name)
                    {
                        csNetworkManager.TCPclient.setKillID(i);
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
}
