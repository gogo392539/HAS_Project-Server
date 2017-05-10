using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class csActiveTrap : MonoBehaviour {

    bool canMove = true;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(canMove == false && csMove.controller != null)
        {
            csMove.controller = null;
        }
	}

    void OnTriggerEnter(Collider other)
    {
        canMove = false;
    }

    void OnTriggerStay(Collider other)
    {
        if(Input.GetKeyDown(KeyCode.Z))
        {
            canMove = true;
            csMove.controller = other.gameObject.GetComponent<CharacterController>();          
        }
    }
}
