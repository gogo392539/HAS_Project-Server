using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class csColorPuzzleController : MonoBehaviour {

    public static int currentMode;

    public GameObject pzObj;
    public Button btComplete;

    // UI 캔버스
    public GameObject g_Canvas;
    public GameObject p_Canvas;

    // 카메라
    public Camera playerCam;
    public Camera puzzleCam;
    public Camera[] aroundCam = new Camera[2];

    // 퍼즐이 완성되었는지 설정
    public bool Complete = false;

    void Update () {
        if (csStartPuzzle_color.checkStart)
        {
            GetComponent<AudioSource>().Play();
            pzObj.GetComponent<textinstantiate1>().StartPuzzle();
            ctrlCompleteButton(false);
            csStartPuzzle_color.checkStart = false;
        }
    }

    public void ctrlCompleteButton(bool check)
    {
        btComplete.gameObject.SetActive(check);
    }

    public void CompleteButton()
    {
        GetComponent<AudioSource>().Stop();

        playerCam.gameObject.SetActive(true);
        puzzleCam.gameObject.SetActive(false);

        if (currentMode == 0)
            aroundCam[0].gameObject.SetActive(false);
        else if (currentMode == 2)
            aroundCam[1].gameObject.SetActive(false);
             
        g_Canvas.SetActive(true);
        p_Canvas.SetActive(false);

        csStartPuzzle_color.checkComplete = true;
    }

    public void GoBakcButton()
    {
        GetComponent<AudioSource>().Stop();

        playerCam.gameObject.SetActive(true);
        puzzleCam.gameObject.SetActive(false);

        if (currentMode == 0)
            aroundCam[0].gameObject.SetActive(false);
        else if (currentMode == 2)
            aroundCam[1].gameObject.SetActive(false);

        g_Canvas.SetActive(true);
        p_Canvas.SetActive(false);

        csStartPuzzle_color.checkGoback = true;
    }
}
