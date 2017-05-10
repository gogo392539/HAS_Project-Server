using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Client;

public class csMoveOther : MonoBehaviour {
	
	// Update is called once per frame
	void Update () {
		for(int i=0; i<csMain.MAXCOUNT.MAX_CLIENT; i++)
        {
            if(this.gameObject.name == csMain.userArray[i].name)
            {
                transform.position = new Vector3
                    (csNetworkManager.UDPclient.getClients()[i].pos.x,
                    csNetworkManager.UDPclient.getClients()[i].pos.y,
                    csNetworkManager.UDPclient.getClients()[i].pos.z);

                transform.rotation = Quaternion.Euler
                    (csNetworkManager.UDPclient.getClients()[i].pos.rotX,
                    csNetworkManager.UDPclient.getClients()[i].pos.rotY,
                    csNetworkManager.UDPclient.getClients()[i].pos.rotZ);
            }
        }
	}
}
