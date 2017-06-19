using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Client;

public class csAnimator : MonoBehaviour {

    private Animator m_animators;
    private static string[] Ani = new string[] {"Throw","Win","Lose","Wave","Dance"};

    void Start () {
        m_animators = GetComponent<Animator>();
    }
	
	void Update () {
        // 잡기 애니메이션
        if (Input.GetKeyDown(KeyCode.Z))
            PlayThrow();
        // 승리 애니메이션
        if (Input.GetKeyDown(KeyCode.X))
            PlayWin();
        // 패배 애니메이션
        if (Input.GetKeyDown(KeyCode.C))
            PlayLose();
        // 인사 애니메이션
        if (Input.GetKeyDown(KeyCode.V))
            PlayWave();
    }

    public void PlayThrow()
    {
        m_animators.Play(Ani[0]);
        if (csNetworkManager.TCPclient != null && csNetworkManager.TCPclient.getAni()[csNetworkManager.TCPclient.getMyID()].aniSet != 5)
            csNetworkManager.TCPclient.AnimationSendFunc(5);
    }

    void PlayWin()
    {
        m_animators.Play(Ani[1]);
        if (csNetworkManager.TCPclient != null && csNetworkManager.TCPclient.getAni()[csNetworkManager.TCPclient.getMyID()].aniSet != 6)
            csNetworkManager.TCPclient.AnimationSendFunc(6);
    }

    void PlayLose()
    {
        m_animators.Play(Ani[2]);
        if (csNetworkManager.TCPclient != null && csNetworkManager.TCPclient.getAni()[csNetworkManager.TCPclient.getMyID()].aniSet != 7)
            csNetworkManager.TCPclient.AnimationSendFunc(7);
    }

    void PlayWave()
    {
        m_animators.Play(Ani[3]);
        if (csNetworkManager.TCPclient != null && csNetworkManager.TCPclient.getAni()[csNetworkManager.TCPclient.getMyID()].aniSet != 8)
            csNetworkManager.TCPclient.AnimationSendFunc(8);
    }

    public void PlayDance()
    {
        m_animators.Play(Ani[4]);
        if (csNetworkManager.TCPclient != null && csNetworkManager.TCPclient.getAni()[csNetworkManager.TCPclient.getMyID()].aniSet != 9)
            csNetworkManager.TCPclient.AnimationSendFunc(9);
    }

    // 트리거 이벤트가 발생하면 춤 애니메이션 실행
    void OnTriggerEnter(Collider other)
    {
        //PlayDance();
    }
    // 트리거 이벤트에서 나오면 인사 애니메이션 실행
    void OnTriggerExit(Collider other)
    {
        //PlayWave();
    }
}
