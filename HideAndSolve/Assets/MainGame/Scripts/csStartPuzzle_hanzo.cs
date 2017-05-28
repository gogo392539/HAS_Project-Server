using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Client;

public class csStartPuzzle_hanzo : MonoBehaviour
{
    // 퍼즐 풀이 완료 확인 변수
    public static bool checkComplete = false;

    // 퍼즐 풀이 중지 확인 변수
    public static bool checkGoback = false;

    // 퍼즐 요청 변수
    public static bool checkStart = false;

    // 탈출구 활성화 컨트롤 변수
    public static bool ExitActive = false;

    // 퍼즐 매개체 오브젝트
    public GameObject Puzzleobj;

    // 카메라 오브젝트
    public Camera playerCam;
    public Camera puzzleCam;
    public Camera aroundCam;

    // 기타 UI
    public GameObject text;
    public GameObject g_Canvas;
    public GameObject p_Canvas;

    // 플레이어 움직임 컨트롤 변수
    bool canMove = true;

    // 퍼즐의 ID 값
    private int id = 0;

    private void Start()
    {
        text.SetActive(false);
    }

    private void Update()
    {
        // 다른 플레이어가 퍼즐 완료
        if (csMain.puzzleSetArray[id] == 2)
        {
            Destroy(Puzzleobj);
            csMain.puzzleCount--;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        text.SetActive(true);
    }

    private void OnTriggerStay(Collider other)
    {
        // 내가 퍼즐을 풀러 들어감
        if (csMain.puzzleSetArray[id] == 0 && Input.GetKeyDown("z"))
        {
            checkStart = true;
            playerCam.gameObject.SetActive(false);
            puzzleCam.gameObject.SetActive(true);
            aroundCam.gameObject.SetActive(true);

            g_Canvas.SetActive(false);
            p_Canvas.SetActive(true);

            csMove.controller = null;

            csNetworkManager.TCPclient.setPuzzle(id, 1);
        }

        // 내가 퍼즐을 완료함
        if (checkComplete)
        {
            checkComplete = false;

            ExitActive = true;
            canMove = true;
            csMove.controller = other.gameObject.GetComponent<CharacterController>();

            csNetworkManager.TCPclient.setPuzzle(id, 2);
            Destroy(Puzzleobj);
            csMain.puzzleCount--;
        }

        // 퍼즐을 풀다가 도망감
        if (checkGoback)
        {
            checkGoback = false;
            canMove = true;
            csMove.controller = other.gameObject.GetComponent<CharacterController>();

            csNetworkManager.TCPclient.setPuzzle(id, 0);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        text.SetActive(false);
    }
}
