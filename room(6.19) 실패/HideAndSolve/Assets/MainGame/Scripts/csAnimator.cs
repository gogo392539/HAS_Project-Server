using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Client;

public class csAnimator : MonoBehaviour {

    private Animator m_animators;
    private static string[] Ani = new string[] {"Throw","Win","Lose","Wave","Dance"};
    private bool isDancing;

    void Start () {
        m_animators = GetComponent<Animator>();
    }
	
	void Update () {
        if (!isDancing)
        {
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
    }

    public void PlayThrow()
    {
        isDancing = false;
        m_animators.Play(Ani[0]);
        if (csNetworkManager.TCPclient != null && csNetworkManager.TCPclient.getAniEvent()[csNetworkManager.TCPclient.getMyID()].aniSet != 0)
        {
            csNetworkManager.TCPclient.OtherAnimSendFunc(0);
            csNetworkManager.TCPclient.getAniEvent()[csNetworkManager.TCPclient.getMyID()].aniSet = -1;
        }
    }

    public void PlayWin()
    {
        isDancing = false;
        m_animators.Play(Ani[1]);
        if (csNetworkManager.TCPclient != null && csNetworkManager.TCPclient.getAniEvent()[csNetworkManager.TCPclient.getMyID()].aniSet != 1)
        { 
            csNetworkManager.TCPclient.OtherAnimSendFunc(1);
            csNetworkManager.TCPclient.getAniEvent()[csNetworkManager.TCPclient.getMyID()].aniSet = -1;
        }
    }

    void PlayLose()
    {
        isDancing = false;
        m_animators.Play(Ani[2]);
        if (csNetworkManager.TCPclient != null && csNetworkManager.TCPclient.getAniEvent()[csNetworkManager.TCPclient.getMyID()].aniSet != 2)
        {
            csNetworkManager.TCPclient.OtherAnimSendFunc(2);
            csNetworkManager.TCPclient.getAniEvent()[csNetworkManager.TCPclient.getMyID()].aniSet = -1;
        }
    }

    public void PlayWave()
    {
        isDancing = false;
        m_animators.Play(Ani[3]);
        if (csNetworkManager.TCPclient != null && csNetworkManager.TCPclient.getAniEvent()[csNetworkManager.TCPclient.getMyID()].aniSet != 3)
        {
            csNetworkManager.TCPclient.OtherAnimSendFunc(3);
            csNetworkManager.TCPclient.getAniEvent()[csNetworkManager.TCPclient.getMyID()].aniSet = -1;
        }
    }

    public void PlayDance()
    {
        isDancing = true;
        m_animators.Play(Ani[4]);
        if (csNetworkManager.TCPclient != null && csNetworkManager.TCPclient.getAniEvent()[csNetworkManager.TCPclient.getMyID()].aniSet != 4)
        {
            csNetworkManager.TCPclient.OtherAnimSendFunc(4);
            csNetworkManager.TCPclient.getAniEvent()[csNetworkManager.TCPclient.getMyID()].aniSet = -1; 
        }
    }
}
