using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class csMathAndAnswer : MonoBehaviour {

    public static csMathAndAnswer instance;

    public enum MathsType
    {
        addition,
        subtraction,
        multiplication
    }

    public MathsType mathsType;

    private float a, b;
    [HideInInspector]
    public float answer;
    private float locationOfAnswer;
    public GameObject[] ansButtons;
    public Image mathSymbolObject;
    public Sprite[] mathSymbols;
    public string tagOfButton;

    private int currentMode;

    public float timeForQuestion;

    [HideInInspector]
    public int score;

    public Text valueA, valueB;

    private float scoreMileStone;
    public float scoreMileStoneCount;


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

    void Start()
    {
        tagOfButton = changeAnwtoTag(locationOfAnswer);
        scoreMileStone = scoreMileStoneCount;
        csGameManager.timeForQuestion = timeForQuestion;
    
        currentMode = csGameManager.currentMode;

        CurrentMode();
        MathsProblem();
    }

    void CurrentMode()
    {
        if (currentMode == 1)
            mathsType = MathsType.addition;
        else if (currentMode == 2)
            mathsType = MathsType.subtraction;
        else if (currentMode == 3)
            mathsType = MathsType.multiplication;
    }

    void Update()
    {
        if (csGameManager.isStart)
            Start();

        tagOfButton = changeAnwtoTag(locationOfAnswer);
        MileStoneProcess();
    }

    void MileStoneProcess()
    {
        if (scoreMileStone < csGameManager.currentScore)
        {
            scoreMileStone += scoreMileStoneCount;
            timeForQuestion += 0.02f;

            if (timeForQuestion >= 0.2f)
                timeForQuestion = 0.2f;
        }
    }

    public void MathsProblem()
    {
        switch (mathsType)
        {
            case (MathsType.addition):
                AdditionMethod();
                break;

            case (MathsType.subtraction):
                SubtractionMethod();
                break;

            case (MathsType.multiplication):
                MultiplicationMethod();
                break;
        }
    }

    void AdditionMethod()
    {
        a = Random.Range(0, 21);
        b = Random.Range(0, 21);

        locationOfAnswer = Random.Range(0, ansButtons.Length);

        answer = a + b;

        valueA.text = "" + a;
        valueB.text = "" + b;

        mathSymbolObject.sprite = mathSymbols[0];

        for (int i = 0; i < ansButtons.Length; i++)
        {
            if (i == locationOfAnswer)
            {
                ansButtons[i].GetComponentInChildren<Text>().text = "" + answer;
            }
            else
            {
                ansButtons[i].GetComponentInChildren<Text>().text = "" + Random.Range(1, 41);
                while (ansButtons[i].GetComponentInChildren<Text>().text == "" + answer)
                    ansButtons[i].GetComponentInChildren<Text>().text = "" + Random.Range(1, 41);
            }
        }
    }

    void SubtractionMethod()
    {
        a = Random.Range(0, 21);
        b = Random.Range(0, 21);

        while (a <= b)
        {
            a = Random.Range(0, 21);
            b = Random.Range(0, 21);
        }

        locationOfAnswer = Random.Range(0, ansButtons.Length);

        answer = a - b;

        valueA.text = "" + a;
        valueB.text = "" + b;

        mathSymbolObject.sprite = mathSymbols[1];

        for (int i = 0; i < ansButtons.Length; i++)
        {
            if (i == locationOfAnswer)
            {
                ansButtons[i].GetComponentInChildren<Text>().text = "" + answer;
            }
            else
            {
                ansButtons[i].GetComponentInChildren<Text>().text = "" + Random.Range(1, 41);
                while (ansButtons[i].GetComponentInChildren<Text>().text == "" + answer)
                {
                    ansButtons[i].GetComponentInChildren<Text>().text = "" + Random.Range(1, 41);
                }
            }

        }

    }
  
    void MultiplicationMethod()
    {
        a = Random.Range(1, 21);
        b = Random.Range(1, 21);

        locationOfAnswer = Random.Range(0, ansButtons.Length);

        answer = a * b;

        valueA.text = "" + a;
        valueB.text = "" + b;

        mathSymbolObject.sprite = mathSymbols[2];

        for (int i = 0; i < ansButtons.Length; i++)
        {
            if (i == locationOfAnswer)
            {
                ansButtons[i].GetComponentInChildren<Text>().text = "" + answer;
            }
            else
            {
                if (a * b <= 100)
                {
                    ansButtons[i].GetComponentInChildren<Text>().text = "" + Random.Range(1, 101);
                }
                else if (a * b <= 200 & a * b > 100)
                {
                    ansButtons[i].GetComponentInChildren<Text>().text = "" + Random.Range(101, 201);
                }
                else if (a * b <= 300 & a * b > 200)
                {
                    ansButtons[i].GetComponentInChildren<Text>().text = "" + Random.Range(201, 301);
                }
                else if (a * b <= 400 & a * b > 300)
                {
                    ansButtons[i].GetComponentInChildren<Text>().text = "" + Random.Range(301, 401);
                }

                while (ansButtons[i].GetComponentInChildren<Text>().text == "" + answer)
                {
                    ansButtons[i].GetComponentInChildren<Text>().text = "" + Random.Range(1, 401);
                }
            }
        }
    }

    string changeAnwtoTag(float l)
    {
        if (l == 0)
            return "zero";
        else if (l == 1)
            return "one";
        else if (l == 2)
            return "two";
        else
            return "three";
    }
}
