using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Client;

public class csAnimOther : MonoBehaviour {

    // 클라이언트 ID
    public int ID;

    // 애니메이터
    private Animator m_animators;

    // Use this for initialization
    void Start ()
    {
        m_animators = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (csNetworkManager.TCPclient != null)
        {
            if (csNetworkManager.TCPclient.getAniEvent()[ID].aniSet == 0)
                doThrow(ID);
            if (csNetworkManager.TCPclient.getAniEvent()[ID].aniSet == 1)
                doWin(ID);
            if (csNetworkManager.TCPclient.getAniEvent()[ID].aniSet == 2)
                doLose(ID);
            if (csNetworkManager.TCPclient.getAniEvent()[ID].aniSet == 3)
                doWave(ID);
            if (csNetworkManager.TCPclient.getAniEvent()[ID].aniSet == 4)
                doDance(ID);
        }
    }

    void doThrow(int index)
    {
        m_animators.Play("Throw");
        csNetworkManager.TCPclient.getAniEvent()[index].aniSet = -1;
    }

    void doWin(int index)
    {
        m_animators.Play("Win");
        csNetworkManager.TCPclient.getAniEvent()[index].aniSet = -1;
    }

    void doLose(int index)
    {
        m_animators.Play("Lose");
        csNetworkManager.TCPclient.getAniEvent()[index].aniSet = -1;
    }

    void doWave(int index)
    {
        m_animators.Play("Wave");
        csNetworkManager.TCPclient.getAniEvent()[index].aniSet = -1;
    }

    void doDance(int index)
    {
        m_animators.Play("Dance");
        csNetworkManager.TCPclient.getAniEvent()[index].aniSet = -1;
    }
}
