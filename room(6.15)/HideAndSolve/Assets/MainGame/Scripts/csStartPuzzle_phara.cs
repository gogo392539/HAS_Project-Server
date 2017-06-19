﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Client;

public class csStartPuzzle_phara : MonoBehaviour
{
    // 퍼즐 풀이 완료 확인 변수
    public static bool checkComplete = false;

    // 퍼즐 풀이 중지 확인 변수
    public static bool checkGoback = false;

    // 퍼즐 요청 변수
    public static bool checkStart = false;

    // 탈출구 활성화 변수
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

    // 퍼즐 ID 세팅
    private int id = 1;

    private void Start()
    {
        text.SetActive(false);
    }

    private void Update()
    {
        // 다른 플레이어에 의해 퍼즐이 완료됨
        if(csMain.puzzleSetArray[id] == 2)
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
        if (csMain.puzzleSetArray[id] == 0 && Input.GetKeyDown(KeyCode.Z))
        {
            checkStart = true;
            playerCam.gameObject.SetActive(false);
            puzzleCam.gameObject.SetActive(true);
            aroundCam.gameObject.SetActive(true);

            g_Canvas.SetActive(false);
            p_Canvas.SetActive(true);

            csMove.controller = null;

            if (csNetworkManager.TCPclient != null)
                csNetworkManager.TCPclient.PuzzleSendFunc(id, 1);
        }

        // 내가 퍼즐을 완료함
        if (checkComplete)
        {
            checkComplete = false;

            ExitActive = true;
            csMove.controller = other.gameObject.GetComponent<CharacterController>();

            if (csNetworkManager.TCPclient != null)
                csNetworkManager.TCPclient.PuzzleSendFunc(id, 2);
            Destroy(Puzzleobj);
            csMain.puzzleCount--;
        }

        // 내가 퍼즐은 풀다 도망감
        if (checkGoback)
        {
            checkGoback = false;
            csMove.controller = other.gameObject.GetComponent<CharacterController>();

            if (csNetworkManager.TCPclient != null)
                csNetworkManager.TCPclient.PuzzleSendFunc(id, 0);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        text.SetActive(false);       
    }
}