using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class csGameUIManager : MonoBehaviour {

    public static bool textSwitch;

    public GameObject EventText;

    private Vector3 textOpen;
    private Vector3 textClose;

    private void Start()
    {
        textClose = EventText.GetComponent<RectTransform>().localPosition;
        textOpen = textClose + new Vector3(0, 120, 0);
    }

    private void Update()
    {
        if (textSwitch)
            EventText.GetComponent<RectTransform>().localPosition = Vector3.Lerp(EventText.GetComponent<RectTransform>().localPosition, textOpen, .5f);
        else
            EventText.GetComponent<RectTransform>().localPosition = Vector3.Lerp(EventText.GetComponent<RectTransform>().localPosition, textClose, .5f);
    }
}
