using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class csExitDoor : MonoBehaviour {

    public GameObject ObjectExit;

	// Use this for initialization
	void Start () {
  
	}
	
	// Update is called once per frame
	void Update () {
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
        GotoGameOver();
    }

    private void GotoGameOver()
    {
        SceneManager.LoadScene("uiGameOver", LoadSceneMode.Single);
    }
}

