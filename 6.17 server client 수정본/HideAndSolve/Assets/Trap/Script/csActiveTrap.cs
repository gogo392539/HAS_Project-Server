using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Client;

public class csActiveTrap : MonoBehaviour {

    public GameObject text;

    private bool canMove = true;
    private bool activeTrap = true;

    private int ID = 0;

    private void Update () {
        // 술래가 함정 활성화
        if (Input.GetKeyDown(KeyCode.Z) && !activeTrap && csMain.trapSetArray[ID] == 0)
        {
            text.SetActive(false);
            csMain.trapSetArray[ID] = 1;
            if (csNetworkManager.TCPclient != null)
                csNetworkManager.TCPclient.TrapSendFunc(ID, 1);
        }

        // 캐릭터를 움직이지 못하게 설정
		if(!canMove && csMove.controller != null)
        {
            csMove.controller = null;
        }
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == csMain.taggerName && csMain.trapSetArray[ID] == 0)
        {
            text.SetActive(true);
            activeTrap = false;
        }

        // 방해물에 부딪힐 경우
        if (other.gameObject.name != csMain.taggerName && csMain.trapSetArray[ID] == 1)
            canMove = false;
    }

    private void OnTriggerStay(Collider other)
    {
        // Z키를 이용해 방해물 탈출
        if(Input.GetKeyDown(KeyCode.Z) && other.gameObject.name != csMain.taggerName)
        {
            canMove = true;
            activeTrap = true;
            csMove.controller = other.gameObject.GetComponent<CharacterController>();
            csMain.trapSetArray[ID] = 0;
            if(csNetworkManager.TCPclient != null)
                csNetworkManager.TCPclient.TrapSendFunc(ID, 0);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name == csMain.taggerName)
        {
            text.SetActive(false);
            activeTrap = true;
        }
    }
}
