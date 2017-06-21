using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class csExitDoor : MonoBehaviour {

    public GameObject ObjectExit;

	void Update () {

        // 퍼즐을 모두 풀었을 경우 탈출구 생성
        if (csMain.puzzleCount == 0)
        {
            int iCount = ObjectExit.transform.childCount;
            for (int i = 0; i < iCount; i++)
            {
                Transform trChild = ObjectExit.transform.GetChild(i);
                trChild.gameObject.SetActive(true);
            }

            ObjectExit.GetComponent<BoxCollider>().enabled = true;
        }
    }

    void OnTriggerExit( Collider other)
    {
        // 탈출구를 통과하면 GameOver 화면으로 통과
        GotoGameOver();
    }

    private void GotoGameOver()
    {
        SceneManager.LoadScene("uiGameOver", LoadSceneMode.Single);
    }
}

