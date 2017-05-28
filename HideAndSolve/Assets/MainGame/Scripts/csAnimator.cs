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
        {
            PlayThrow();
            if (csNetworkManager.TCPclient.getAni()[csNetworkManager.TCPclient.getMyID()].aniSet != 5)
                csNetworkManager.TCPclient.setAni(5);
        }
        // 승리 애니메이션
        if (Input.GetKeyDown(KeyCode.X))
        {
            PlayWin();
            if (csNetworkManager.TCPclient.getAni()[csNetworkManager.TCPclient.getMyID()].aniSet != 6)
                csNetworkManager.TCPclient.setAni(6);
        }
        // 패배 애니메이션
        if (Input.GetKeyDown(KeyCode.C))
        {
            PlayLose();
            if (csNetworkManager.TCPclient.getAni()[csNetworkManager.TCPclient.getMyID()].aniSet != 7)
                csNetworkManager.TCPclient.setAni(7);
        }
        // 인사 애니메이션
        if (Input.GetKeyDown(KeyCode.V))
        {
            PlayWave();
            if (csNetworkManager.TCPclient.getAni()[csNetworkManager.TCPclient.getMyID()].aniSet != 8)
                csNetworkManager.TCPclient.setAni(8);
        }
    }

    public void PlayThrow()
    {
        m_animators.Play(Ani[0]);
    }

    void PlayWin()
    {
        m_animators.Play(Ani[1]);
    }

    void PlayLose()
    {
        m_animators.Play(Ani[2]);
    }

    void PlayWave()
    {
        m_animators.Play(Ani[3]);
    }

    void PlayDance()
    {
        m_animators.Play(Ani[4]);
    }

    // 트리거 이벤트가 발생하면 춤 애니메이션 실행
    void OnTriggerEnter(Collider other)
    {
        PlayDance();
        if (csNetworkManager.TCPclient != null && csNetworkManager.TCPclient.getAni()[csNetworkManager.TCPclient.getMyID()].aniSet != 9)
            csNetworkManager.TCPclient.setAni(9);
    }
    // 트리거 이벤트에서 나오면 인사 애니메이션 실행
    void OnTriggerExit(Collider other)
    {
        PlayWave();
        if (csNetworkManager.TCPclient != null && csNetworkManager.TCPclient.getAni()[csNetworkManager.TCPclient.getMyID()].aniSet != 8)
            csNetworkManager.TCPclient.setAni(8);
    }
}
