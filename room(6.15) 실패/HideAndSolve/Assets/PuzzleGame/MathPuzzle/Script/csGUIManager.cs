using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class csGUIManager : MonoBehaviour {

    public Text inGameScoreText;

    void Update()
    {
        // 현재 점수 표시
        inGameScoreText.text = csButtonPress.score.ToString();
    }
}
