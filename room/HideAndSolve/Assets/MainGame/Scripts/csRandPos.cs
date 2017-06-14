using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Client;

public class csRandPos : MonoBehaviour
{
    public static bool startPos = false;

    public GameObject[] SpawnPos = new GameObject[5];
	
	void Update () {
		if(startPos)
        {
            // 클라이언트로부터 받은 위치 정보를 통해 각 클라이언트의 위치 재설정
            for(int i=0; i<csMain.MAXCOUNT.MAX_CLIENT; i++)
            {
                Debug.Log(csNetworkManager.TCPclient.getclientPosIndex()[i]);
                csMain.userArray[i].transform.position = SpawnPos[csNetworkManager.TCPclient.getclientPosIndex()[i]].transform.position;
            }
            startPos = false;
        }
	}
}
