using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

using Client;

public class csMoveOther : MonoBehaviour {

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
            // 다른 플레이어의 움직임 전송
            for (int i = 0; i < csMain.MAXCOUNT.MAX_CLIENT; i++)
            {
                if (this.gameObject.name == csMain.userArray[i].name)
                {
                    transform.position = new Vector3
                        (csNetworkManager.UDPclient.getClients()[i].pos.x,
                        csNetworkManager.UDPclient.getClients()[i].pos.y,
                        csNetworkManager.UDPclient.getClients()[i].pos.z);

                    transform.rotation = Quaternion.Euler
                        (csNetworkManager.UDPclient.getClients()[i].pos.rotX,
                        csNetworkManager.UDPclient.getClients()[i].pos.rotY,
                        csNetworkManager.UDPclient.getClients()[i].pos.rotZ);

                    // 해당 플레이어가 땅에 있는지 없는지 검사
                    if (csNetworkManager.TCPclient.getAni()[i].aniSet == 10)
                        m_animators.SetBool("Ground", false);
                    else
                        m_animators.SetBool("Ground", true);
                }
            }
        }
    }

    void AnimUpdate()
    {
        for (int i = 0; i < csMain.MAXCOUNT.MAX_CLIENT; i++) {
            // 각 캐릭터의 애니메이션 실행
            if (csNetworkManager.TCPclient != null && this.gameObject.name == csMain.userArray[i].name)
            {
                if (csNetworkManager.TCPclient.getAni()[i].aniSet == 5)
                {
                    m_animators.Play("Throw");
                    csNetworkManager.TCPclient.getAni()[i].aniSet = -1;
                }
                if (csNetworkManager.TCPclient.getAni()[i].aniSet == 6)
                {
                    m_animators.Play("Win");
                    csNetworkManager.TCPclient.getAni()[i].aniSet = -1;
                }
                if (csNetworkManager.TCPclient.getAni()[i].aniSet == 7)
                {
                    m_animators.Play("Lose");
                    csNetworkManager.TCPclient.getAni()[i].aniSet = -1;
                }
                if (csNetworkManager.TCPclient.getAni()[i].aniSet == 8)
                {
                    m_animators.Play("Wave");
                    csNetworkManager.TCPclient.getAni()[i].aniSet = -1;
                }
                if (csNetworkManager.TCPclient.getAni()[i].aniSet == 9)
                {
                    m_animators.Play("Dance");
                    csNetworkManager.TCPclient.getAni()[i].aniSet = -1;
                }

                if (csNetworkManager.TCPclient.getAni()[i].aniSet == 1)
                    doWalk();
                if (csNetworkManager.TCPclient.getAni()[i].aniSet == 2)
                    doRun();
                if (csNetworkManager.TCPclient.getAni()[i].aniSet == 3)
                    doJump();
                if (csNetworkManager.TCPclient.getAni()[i].aniSet == 4)
                    doBackWalk();
                if (csNetworkManager.TCPclient.getAni()[i].aniSet == 0)
                    doIdle();
            }
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

