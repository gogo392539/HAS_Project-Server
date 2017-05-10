using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Client;

public class csRandPos : MonoBehaviour
{
    public static bool startPos = false;

    public GameObject[] SpawnPos = new GameObject[5];
	
	// Update is called once per frame
	void Update () {
		if(startPos)
        {
            for(int i=0; i<csMain.MAXCOUNT.MAX_CLIENT; i++)
            {
                csMain.userArray[i].transform.position = SpawnPos[csNetworkManager.TCPclient.getclientPosIndex()[i]].transform.position;
            }
            startPos = false;
        }
	}
}
