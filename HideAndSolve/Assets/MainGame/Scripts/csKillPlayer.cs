using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class csKillPlayer : MonoBehaviour {
    public Material invisible_M;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (csMain.killrecvID != -1)
        {
            csMain.userArray[csMain.killrecvID].GetComponent<Renderer>().material = invisible_M;
            csMain.userArray[csMain.killrecvID].GetComponent<CapsuleCollider>().enabled = false;
            csMain.userArray[csMain.killrecvID].GetComponent<BoxCollider>().enabled = false;
            csMain.killrecvID = -1;
            csMain.playerCount--;
        }

        if(csMain.playerCount == 1)
        {
            GotoGameOver();
        }
	}

    private void GotoGameOver()
    {
        SceneManager.LoadScene("uiGameOver", LoadSceneMode.Single);
    }
}
