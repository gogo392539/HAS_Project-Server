using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

using Client;

public class csMoveOther : MonoBehaviour {

    // 클라이언트 ID
    public int ID;

    // 애니메이터
    private Animator m_animators;

    void Start()
    {
        m_animators = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update () {
        AnimUpdate();
        if (csNetworkManager.UDPclient != null)
        {
            csMain.userArray[ID].transform.position = new Vector3
                (csNetworkManager.UDPclient.getClients()[ID].pos.x,
                csNetworkManager.UDPclient.getClients()[ID].pos.y,
                csNetworkManager.UDPclient.getClients()[ID].pos.z);

            csMain.userArray[ID].transform.rotation = Quaternion.Euler
                (csNetworkManager.UDPclient.getClients()[ID].pos.rotX,
                csNetworkManager.UDPclient.getClients()[ID].pos.rotY,
                csNetworkManager.UDPclient.getClients()[ID].pos.rotZ);

            // 해당 플레이어가 땅에 있는지 없는지 검사
            if (csNetworkManager.TCPclient.getGroundCheck()[ID].aniSet == 1)
                m_animators.SetBool("Ground", false);
            else
                m_animators.SetBool("Ground", true);
        }
    }

    void AnimUpdate()
    {
        if (csNetworkManager.TCPclient != null)
        {
            if (csNetworkManager.TCPclient.getAniNormal()[ID].aniSet == 0)
                doIdle();
            if (csNetworkManager.TCPclient.getAniNormal()[ID].aniSet == 1)
                doWalk();
            if (csNetworkManager.TCPclient.getAniNormal()[ID].aniSet == 2)
                doRun();
            if (csNetworkManager.TCPclient.getAniNormal()[ID].aniSet == 3)
                doJump();
            if (csNetworkManager.TCPclient.getAniNormal()[ID].aniSet == 4)
                doBackWalk();
        }
    }

    void doIdle()
    {
        m_animators.SetInteger("aniStep", 0);
    }

    void doWalk()
    {
        m_animators.SetInteger("aniStep", 1);
    }

    void doRun()
    {
        m_animators.SetInteger("aniStep", 2);
    }

    void doJump()
    {
        m_animators.SetInteger("aniStep", 3);
    }

    void doBackWalk()
    {
        m_animators.SetInteger("aniStep", 4);
    }
}

