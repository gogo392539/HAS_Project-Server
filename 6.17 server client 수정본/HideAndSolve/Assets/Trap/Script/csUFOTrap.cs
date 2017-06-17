using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class csUFOTrap : MonoBehaviour {
    
    private GameObject target;
    public GameObject UFO;

    public Camera playerCam;
    public Camera UFOCam;
    //public Camera playerFrontCam;

    public AudioSource UFO_audio;

    //public Animator UFOAni;

    public static bool UFOTrap = false;

    private float FinalDestination;

    //private float playerPosY;

    private float dampSpeed = 0.5f;

    private bool canMove = true;
    //public static bool b_TrapActive = false;
    //private bool activeTrap = true;

    private void Update()
    {
        // 술래가 함정 활성화
        //if (Input.GetKeyDown(KeyCode.Z) && !activeTrap && csMain.trapSetArray[ID] == 0)
        //{
        //    text.SetActive(false);
        //        csMain.trapSetArray[ID] = 1;
        //        if (csNetworkManager.TCPclient != null)
        //            csNetworkManager.TCPclient.setTrap(ID, 1);
        //    }

        // 캐릭터를 움직이지 못하게 설정
        //if (!canMove && csMove.controller != null)
        //{
        //    csMove.controller = null;
        //    bull.SetActive(true);
        //}

        if (UFOTrap && !canMove)
        {
            target.GetComponent<csAnimator>().PlayWave();

            Vector3 UFOD = new Vector3(target.transform.position.x, target.transform.position.y+1.5f, target.transform.position.z);
            UFO.transform.position = Vector3.Lerp(UFO.transform.position, UFOD, Time.deltaTime * dampSpeed);
            playerCam.gameObject.SetActive(false);
            UFOCam.gameObject.SetActive(true);
            //target.transform.rotation = Quaternion.Euler(0, 0, 0);
        }

        if (target != null)
        {
            if (((UFO.transform.position.x - target.transform.position.x) < 0.3) && !((target.transform.position.y - UFO.transform.position.y) > 1.1))
            {
                canMove = true;
                target.GetComponent<csAnimator>().PlayWin();

                Vector3 targetD = new Vector3(UFO.transform.position.x, UFO.transform.position.y + 1.5f, target.transform.position.z);

                target.transform.position = Vector3.Lerp(target.transform.position, targetD, Time.deltaTime * 0.5f);
            }

            if ((target.transform.position.y - UFO.transform.position.y) > 1.1)
            {
                target.GetComponent<csAnimator>().PlayWave();
                Vector3 targetD2 = new Vector3(target.transform.position.x + 2.0f, target.transform.position.y, target.transform.position.z);
                target.transform.position = Vector3.Lerp(target.transform.position, targetD2, Time.deltaTime * 1.0f);

                Vector3 UFOD2 = new Vector3(UFO.transform.position.x + 2.0f, UFO.transform.position.y, UFO.transform.position.z);
                UFO.transform.position = Vector3.Lerp(UFO.transform.position, UFOD2, Time.deltaTime * 1.0f);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //text.SetActive(true);
        UFOTrap = true;
        canMove = false;

        target = other.gameObject;
        
        UFO_audio.Play();

        //playerPosY = target.transform.position.y;

        //if (other.gameObject.name == "Player1" && csMain.trapSetArray[ID] == 0)
        //{
        //    text.SetActive(true);
        //    activeTrap = false;
        //}

        //// 방해물에 부딪힐 경우
        //if (other.gameObject.name != "Player1" && csMain.trapSetArray[ID] == 1)
        //    canMove = false;

    }

    private void OnTriggerStay(Collider other)
    {
        //// Z키를 이용해 방해물 탈출
        //if (Input.GetKeyDown(KeyCode.Z) && other.gameObject.name != "Player1")
        //{
        //    canMove = true;
        //    activeTrap = true;
        //    csMove.controller = other.gameObject.GetComponent<CharacterController>();
        //    csMain.trapSetArray[ID] = 0;
        //    if (csNetworkManager.TCPclient != null)
        //        csNetworkManager.TCPclient.setTrap(ID, 0);
        //}

        //if (Input.GetKeyDown(KeyCode.Z))
        //{
        //    text.SetActive(false);
        //    canMove = true;
        //    csMove.controller = other.gameObject.GetComponent<CharacterController>();
        //}
    }

    void OnTriggerExit(Collider other)
    {
        //if (other.gameObject.name == "Player1")
        //{
        //    text.SetActive(false);
        //    activeTrap = true;
        //}
        //text.SetActive(false);
    }

    //void Exit()
    //{
    //    for (int i = 0; i < bullNumber; i++)
    //        bull[i].SetActive(false);
    //    canMove = true;
    //    b_TrapActive = false;
    //    bull_audio.Stop();

    //    playerCam.gameObject.SetActive(true);
    //    BullCam.gameObject.SetActive(false);
    //}

    //void CamChange()
    //{
    //    playerCam.gameObject.SetActive(false);
    //    UFOCam.gameObject.SetActive(true);
    //}
}
