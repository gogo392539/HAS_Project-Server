using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class csSandBox : MonoBehaviour {

    public Material invisible_M;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	    if(Input.GetKeyDown(KeyCode.Z))
        {
            gameObject.GetComponent<Renderer>().material = invisible_M;
        }	
	}
}
