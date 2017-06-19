using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class csButtonPress : MonoBehaviour {

    public GameObject gManager;
    public static int score;

    public Image backgroundSprite;

    private AudioSource ansSound;

    [SerializeField]
    private AudioClip[] soundToPlay;

    void Start()
    {
        score = 0;

        ansSound = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (csGameManager.isStart)
            Start();
    }

    public void checkTheTextofButton()
    {
        // 정답을 맞출 경우
        if (gameObject.CompareTag(csMathAndAnswer.instance.tagOfButton))
        {
            score++;
            // 10문제 모두 성공하면 퍼즐 완료
            if (score == 10)
            {
                csTimeBarController.iscomplete = true;
                gManager.GetComponent<csGameManager>().complete();
            }
            csTimeBarController.instance.currentAmount = 1;
            ansSound.PlayOneShot(soundToPlay[0]);
        }
        else
        {
            ansSound.PlayOneShot(soundToPlay[1]);
            StartCoroutine(ColorChange());
        }

        csMathAndAnswer.instance.MathsProblem();
    }

    // 틀렸을 경우 색깔 변화를 통해 알려줌
    IEnumerator ColorChange()
    {
        backgroundSprite.color = new Color32(221, 127, 127, 255);

        yield return new WaitForSeconds(0.05f);

        backgroundSprite.color = new Color32(255, 255, 255, 255);
    }
}
