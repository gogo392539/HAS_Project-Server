using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class csGameManager : MonoBehaviour {

    // 수식퍼즐에 사용하는 전역변수
    public static float timeForQuestion;
    public static int currentScore;
    public static bool isGameOver;
    public static int currentMode;
    public static bool isStart;

    public GameObject mathController;
    public GameObject timeBar;

    // UI 캔버스
    public GameObject g_Canvas;
    public GameObject p_Canvas;

    // 카메라
    public Camera playerCam;
    public Camera puzzleCam;
    public Camera[] aroundCam = new Camera[3];

    // 버튼
    public Button completeButton;
    public Button gobackButton;

    // 퍼즐이 완성되었는지 설정
    public bool Complete = false;

    // 현재 실행되고 있는지 확인
    private bool check = false;

    private void Update()
    {
        // 퍼즐을 풀기 시작하면 모든 스크립트를 시작하도록 설정
        if (csStartPuzzle_math.checkStart == true)
        {
            check = true;
            csStartPuzzle_math.checkStart = false;
            completeButton.gameObject.SetActive(false);
            StartCoroutine(SettingStart());
        }
    }


    public void completebutton()
    {
        // 퍼즐 풀기 완료 버튼
        if (check)
        {
            GetComponent<AudioSource>().Stop();
            playerCam.gameObject.SetActive(true);
            puzzleCam.gameObject.SetActive(false);
            aroundCam[currentMode-1].gameObject.SetActive(false);

            g_Canvas.SetActive(true);
            p_Canvas.SetActive(false);

            check = false;
            csStartPuzzle_math.checkComplete = true;
        }
    }

    public void gobackbutton()
    {
        // 풀다가 도망갈 경우의 버튼
        if (check)
        {
            GetComponent<AudioSource>().Stop();
            playerCam.gameObject.SetActive(true);
            puzzleCam.gameObject.SetActive(false);
            aroundCam[currentMode-1].gameObject.SetActive(false);

            g_Canvas.SetActive(true);
            p_Canvas.SetActive(false);

            csStartPuzzle_math.checkGoback = true;
            check = false;
        }
    }

    // 풀기를 완료하면 완료버튼 활성화
    public void complete()
    {
        completeButton.gameObject.SetActive(true);
    }

    IEnumerator SettingStart()
    {
        isStart = true;
        GetComponent<AudioSource>().Play();
        // 0.05초 후에 isStart를 false로 세팅함으로 계속 시작되는 현상을 막음
        yield return new WaitForSeconds(0.05f);
        isStart = false;
    }
}
