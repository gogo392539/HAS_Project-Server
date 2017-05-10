using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Client;
public class csCatch : MonoBehaviour {

    public GameObject CatchSign;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "otherPlayer")
            CatchSign.SetActive(true);
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject.tag == "otherPlayer")
        {
 //           CatchSign.SetActive(true);
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
