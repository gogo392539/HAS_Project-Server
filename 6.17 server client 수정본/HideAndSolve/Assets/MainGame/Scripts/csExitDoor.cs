using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class csExitDoor : MonoBehaviour {

    public GameObject ObjectExit;
    public GameObject PlayerCam;

    private bool win = false;

    private GameObject Player;

	void Update () {

        if (Input.GetKeyDown(KeyCode.Z))
        {
            int iCount = ObjectExit.transform.childCount;
            for (int i = 0; i < iCount; i++)
            {
                Transform trChild = ObjectExit.transform.GetChild(i);
                trChild.gameObject.SetActive(true);
            }

            ObjectExit.GetComponents<BoxCollider>()[0].enabled = true;
            ObjectExit.GetComponents<BoxCollider>()[1].enabled = true;
        }

        if (win)
        {
            Player.GetComponent<csAnimator>().PlayWin();
        }
        // 퍼즐을 모두 풀었을 경우 탈출구 생성
        //if (csMain.puzzleCount == 0 || csMain.playerCount == 2)
        //{
        //    ExitDoor.SetActive(true);
        //}
        //if (csMain.puzzleCount == 0)
        //{
        //    int iCount = ObjectExit.transform.childCount;
        //    for (int i = 0; i < iCount; i++)
        //    {
        //        Transform trChild = ObjectExit.transform.GetChild(i);
        //        trChild.gameObject.SetActive(true);
        //    }

        //    ObjectExit.GetComponent<BoxCollider>().enabled = true;
        //}
    }

    void OnTriggerEnter(Collider other)
    {
        win = true;
        Player = other.gameObject;

        PlayerCam.transform.RotateAround(Player.transform.position, Vector3.right, 10f);
        // 탈출구를 통과하면 GameOver 화면으로 통과
        //GotoGameOver();
    }

    private void GotoGameOver()
    {
        SceneManager.LoadScene("uiGameOver", LoadSceneMode.Single);
    }
}

