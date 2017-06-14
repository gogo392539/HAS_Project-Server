using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class csTimeBarController : MonoBehaviour {

    public static csTimeBarController instance;

    public static bool iscomplete;

    public GameObject gManager;

    //ref to the images which will display the time in fill type
    public Transform fillBar;
    //ref to the fill amount or bar 
    [HideInInspector]
    public float currentAmount;
    //ref to the time
    private float timeT;

    void Start()
    {
        timeT = csGameManager.timeForQuestion;

        iscomplete = false;

        currentAmount = 1;
    }

    void Awake()
    {
        MakeInstance();
    }

    void MakeInstance()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    void Update()
    {
        if (csGameManager.isStart)
            Start();
        //we reduces the time when quesition is asked with respect to game time
        if (!iscomplete)
        {
            currentAmount -= (timeT) * Time.deltaTime;

            fillBar.GetComponent<Image>().fillAmount = currentAmount;

            if (currentAmount <= 0)
            {
                //if the fill become zero , means the time is over we declare game over
                gManager.GetComponent<csGameManager>().gobackbutton();
            }
        }
    }
}
