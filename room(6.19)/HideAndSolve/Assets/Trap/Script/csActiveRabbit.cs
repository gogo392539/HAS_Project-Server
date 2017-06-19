using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Client;

public class csActiveRabbit : MonoBehaviour {

    public GameObject[] rabbit = new GameObject[5];
    public GameObject text;
    public GameObject particle;

    private bool canMove = true;
    private bool activeTrap = true;

    private Collider target;

    private int ID = 1;

    void Update()
    {
        // 술래가 함정 활성화
        if (Input.GetKeyDown(KeyCode.Z) && !activeTrap && csMain.trapSetArray[ID] == 0)
        {
            text.SetActive(false);
            csMain.trapSetArray[ID] = 1;
            if (csNetworkManager.TCPclient != null)
                csNetworkManager.TCPclient.TrapSendFunc(ID, 1);
        }

        // 캐릭터를 움직이지 못하게 설정
        if (!canMove && csMove.controller != null)
        {
            csMove.controller = null;
            StartCoroutine(MakeRabbit());
            StartCoroutine(ActiveTrap());
            if (csNetworkManager.TCPclient != null)
                csNetworkManager.TCPclient.TrapSendFunc(ID, 2);
        }

        if(csMain.trapSetArray[ID] == 2)
        {
            StartCoroutine(MakeRabbit());
            StartCoroutine(ActiveTrapOther());
            csMain.trapSetArray[ID] = -1;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "Player1" && csMain.trapSetArray[ID] == 0)
        {
            text.SetActive(true);
            activeTrap = false;
        }

        // 방해물에 부딪힐 경우
        if (other.gameObject.name != "Player1" && csMain.trapSetArray[ID] == 1)
        {
            canMove = false;
            target = other;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name == "Player1")
        {
            text.SetActive(false);
            activeTrap = true;
        }
    }

    private void FinishTrap()
    {
        canMove = true;
        activeTrap = true;
        csMove.controller = target.gameObject.GetComponent<CharacterController>();
        csMain.trapSetArray[ID] = 0;
        if (csNetworkManager.TCPclient != null)
            csNetworkManager.TCPclient.TrapSendFunc(ID, 0);
    }

    private void FinishTrapOther()
    {
        activeTrap = true;
        csMain.trapSetArray[ID] = 0;
    }

    IEnumerator ActiveTrap()
    {
        target.GetComponent<csAnimator>().PlayDance();
        GetComponent<AudioSource>().Play();
        yield return new WaitForSeconds(40.0f);
        GetComponent<AudioSource>().Stop();
        for(int i=0; i<5; i++)
            rabbit[i].SetActive(false);
        yield return new WaitForSeconds(3.0f);
        target.GetComponent<csAnimator>().PlayThrow();
        FinishTrap();
    }

    IEnumerator ActiveTrapOther()
    {
        GetComponent<AudioSource>().Play();
        yield return new WaitForSeconds(40.0f);
        GetComponent<AudioSource>().Stop();
        for (int i = 0; i < 5; i++)
            rabbit[i].SetActive(false);
        yield return new WaitForSeconds(3.0f);
        FinishTrapOther();
    }

    IEnumerator MakeRabbit()
    {
        // 5초마다 토끼를 한마리씩 총 5마리 생성
        for (int i = 0; i < 5; i++)
        {
            Instantiate(particle, rabbit[i].transform.position - new Vector3(0, 1, 0), rabbit[i].transform.rotation);
            rabbit[i].SetActive(true);
            yield return new WaitForSeconds(5.0f);
        }
    }
}
