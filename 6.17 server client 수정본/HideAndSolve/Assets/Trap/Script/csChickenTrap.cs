using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class csChickenTrap : MonoBehaviour {

    public GameObject allchicken;
    public GameObject[] chickens;

    private Vector3 fisrtPos;

    private void Update ()
    {
        if (chickens[0] != null)
        {
            for(int i=1; i < chickens.Length; i++)
            {
                chickens[i].transform.position = Vector3.Lerp(chickens[i].transform.position, chickens[i-1].transform.position, 3.0f * Time.deltaTime);
                chickens[i].transform.LookAt(chickens[i - 1].transform);
                GameObject.Find(chickens[i].gameObject.name).SendMessage("Run");
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (chickens[0] == null)
        {
            chickens[0] = other.gameObject;
            fisrtPos = allchicken.transform.position;
        }

//        Invoke("EndTrap", 30f);
    }

    private void OnTriggerStay(Collider other)
    {

    }

    private void OnTriggerExit(Collider other)
    {

    }

    private void EndTrap()
    {
        chickens[0] = null;
        allchicken.transform.position = fisrtPos;
    }
}
