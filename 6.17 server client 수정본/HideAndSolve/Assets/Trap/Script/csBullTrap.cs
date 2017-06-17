using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Client;

public class csBullTrap : MonoBehaviour
{
    private const int bullNumber = 9;

    public GameObject[] bull = new GameObject[9];
    public Animator[] anim = new Animator[9];
    public GameObject text;
    public GameObject particle;

    private GameObject target;
    private Vector3 destination;

    public Camera playerCam;
    public Camera BullCam;

    public AudioSource bull_audio;

    public static bool BullTrap = false;

    private bool bullTrapNet = false;
    private bool canMove = true;
    private bool activeTrap = true;

    private float destination_pos_x = 33.0f;
    private float destination_pos_y = 0.0f;
    private float destination_pos_z = -20.0f;

    private float dampSpeed = 0.2f;

    private int ID = 2;

    private void Update()
    {
        // 술래가 함정 활성화
        if (Input.GetKeyDown(KeyCode.Z) && !activeTrap && csMain.trapSetArray[ID] == 0)
        {
            text.SetActive(false);
            csMain.trapSetArray[ID] = 1;

            if (csNetworkManager.TCPclient != null)
                csNetworkManager.TCPclient.TrapSendFunc(ID, 1);
        }

        if (!canMove && BullTrap)
        {
            for (int i = 0; i < bullNumber; i++)
                bull[i].SetActive(true);
            bull_audio.Play();

            playerCam.gameObject.SetActive(false);
            BullCam.gameObject.SetActive(true);

            destination = new Vector3(destination_pos_x, destination_pos_y, destination_pos_z);
            target.GetComponent<csMove>().doRun();
            target.transform.rotation = Quaternion.Euler(0, 0, 0);

            if (csNetworkManager.TCPclient != null)
                csNetworkManager.TCPclient.TrapSendFunc(ID, 2);

            AngryRun(); // 소떼 애니메이션 활성화

            StartCoroutine(Exit());

            canMove = true;
        }

        if (BullTrap && !bullTrapNet)
        {
            target.transform.position = Vector3.Lerp(target.transform.position, destination, Time.deltaTime * dampSpeed);

            BullPosChange();
            MakeParticle();
        }

        if (csMain.trapSetArray[ID] == 2)
        {
            target = csMain.userArray[csMain.traprecvID];

            for (int i = 0; i < bullNumber; i++)
                bull[i].SetActive(true);
            bull_audio.Play();

            AngryRun(); // 소떼 애니메이션 활성화

            StartCoroutine(ExitOther());

            csMain.trapSetArray[ID] = -1;
            bullTrapNet = true;
        }

        if (bullTrapNet)
        {
            BullPosChange();
            MakeParticle();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == csMain.taggerName && csMain.trapSetArray[ID] == 0)
        {
            text.SetActive(true);
            activeTrap = false;
        }

        // 방해물에 부딪힐 경우
        if (other.gameObject.name != csMain.taggerName && csMain.trapSetArray[ID] == 1)
        {
            BullTrap = true;
            canMove = false;
            target = other.gameObject;
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

    IEnumerator Exit()
    {
        yield return new WaitForSeconds(10f);
        for (int i = 0; i < bullNumber; i++)
            bull[i].SetActive(false);

        activeTrap = true;
        BullTrap = false;
        bull_audio.Stop();

        playerCam.gameObject.SetActive(true);
        BullCam.gameObject.SetActive(false);

        csMain.trapSetArray[ID] = 0;

        if (csNetworkManager.TCPclient != null)
            csNetworkManager.TCPclient.TrapSendFunc(ID, 0);
    }

    IEnumerator ExitOther()
    {
        yield return new WaitForSeconds(10f);
        for (int i = 0; i < bullNumber; i++)
            bull[i].SetActive(false);

        activeTrap = true;
        bullTrapNet = false;
        bull_audio.Stop();

        csMain.trapSetArray[ID] = 0;
    }

    public void AngryRun()
    {
        for (int i = 0; i < bullNumber; i++)
            anim[i].SetTrigger("AngryRun");
    }

    private void MakeParticle()
    {
        for (int i = 2; i < 9; ++i)
        {
            Instantiate(particle, bull[i].transform.position, bull[i].transform.rotation);
        }
    }

    private void BullPosChange()
    {
        bull[0].transform.position = target.transform.position + new Vector3(-0.6f, 0, -1.2f);
        bull[1].transform.position = target.transform.position + new Vector3(0.6f, 0, -1.2f);

        bull[2].transform.position = target.transform.position + new Vector3(-1.2f, 0, -2.4f);
        bull[3].transform.position = target.transform.position + new Vector3(0.0f, 0, -2.4f);
        bull[4].transform.position = target.transform.position + new Vector3(1.2f, 0, -2.4f);

        bull[5].transform.position = target.transform.position + new Vector3(-1.8f, 0, -3.6f);
        bull[6].transform.position = target.transform.position + new Vector3(-0.6f, 0, -3.6f);
        bull[7].transform.position = target.transform.position + new Vector3(0.6f, 0, -3.6f);
        bull[8].transform.position = target.transform.position + new Vector3(1.8f, 0, -3.6f);
    }
}
