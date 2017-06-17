using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class csRandomID : MonoBehaviour {

    public GameObject[] myobj = new GameObject[5];
    public GameObject[] youobj = new GameObject[5];
    public static int ID;

    private GameObject myCharacter;

    public GameObject PlayerCamera;

    // Use this for initialization
    void Start()
    {
        ID = Random.Range(0, 5);
        for (int i = 0; i < 5; i++)
        {
            if (i != ID)
            {               
                csMain.userArray[i] = GameObject.Find("Other" + (i + 1));
                Destroy(myobj[i]);
            }
            if (i == ID)
            {
                csMain.userArray[i] = GameObject.Find("Player" + (i + 1));
                Destroy(youobj[i]);
            }
        }

        myCharacter = myobj[ID];
        myCharacter.GetComponent<csMove>().setCharacterController();

        PlayerCamera.transform.parent = myCharacter.transform;
        PlayerCamera.transform.position = myCharacter.transform.position - Vector3.forward * 3.0f + Vector3.up * 1.6f;
    }
	
	// Update is called once per frame
	void Update () {

    }
}
