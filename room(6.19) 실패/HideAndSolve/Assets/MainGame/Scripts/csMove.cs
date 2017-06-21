using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Client;

public class csMove : MonoBehaviour {

    // 체력바
    public Slider staminaBarSlider;

    // 움직임 속도
    public float movSpeed = 2.0f;
    // 방향전환 속도
    public float rotSpeed = 120.0f;

    // 캐릭터 컨트롤러
    public static CharacterController controller;

    // 움직임 정보
    private Vector3 moveDirection;

    // 점프 속도
    private float jumpSpeed = 2.0f;
    // 중력
    private float gravity = 3.8f;

    // 캐릭터
    private Animator anim;

    // 캐릭터 컨트롤러 세팅
    public void setCharacterController()
    {
        controller = GetComponent<CharacterController>();
    }

    private void Start () {
        // 애니메이터 세팅
        anim = GetComponent<Animator>();
    }
	
	private void Update () {
        float amtRot = rotSpeed * Time.deltaTime;
        float ang = Input.GetAxis("Horizontal");

        if (controller != null && !csBullTrap.BullTrap && !csUFOTrap.UFOTrap)
        {
            // 방향전환
            transform.Rotate(Vector3.up * ang * amtRot);
            if (controller.isGrounded)
            {
                // 땅에 있을 경우의 애니메이션 세팅
                anim.SetBool("Ground", controller.isGrounded);
                if (csNetworkManager.TCPclient != null && csNetworkManager.TCPclient.getGroundCheck()[csNetworkManager.TCPclient.getMyID()].aniSet != 0)
                    csNetworkManager.TCPclient.GroundCheckSendFunc(0);

                float ver = Input.GetAxis("Vertical");

                // shift에 대한 속도 변화
                if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.UpArrow) && staminaBarSlider.value > 0)
                    movSpeed = 5.0f;
                else
                    movSpeed = 2.0f;

                moveDirection = new Vector3(0, 0, ver * movSpeed);
                moveDirection = transform.TransformDirection(moveDirection);

                // 점프
                if (Input.GetKey(KeyCode.LeftAlt))
                    moveDirection.y = jumpSpeed;

                // 체력바 소모 구현
                if (Input.GetKey(KeyCode.UpArrow) && movSpeed == 5.0 && staminaBarSlider.value > 0)
                    staminaBarSlider.value -= 0.01f;
                else if (staminaBarSlider.value <= 100 && !Input.GetKey(KeyCode.LeftShift))
                    staminaBarSlider.value += .3f;

                AnimUpdate();
            }
            else
            {
                // 땅에 없을 경우 애니메이션 세팅
                anim.SetBool("Ground", controller.isGrounded);
                if (csNetworkManager.TCPclient != null && csNetworkManager.TCPclient.getGroundCheck()[csNetworkManager.TCPclient.getMyID()].aniSet != 1)
                    csNetworkManager.TCPclient.GroundCheckSendFunc(1);
            }

            moveDirection.y -= gravity * Time.deltaTime;

            // 실제 움직임
            controller.Move(moveDirection * Time.deltaTime);
        }
    }

    private void LateUpdate()
    {
        // 자신의 위치정보 전송
        if(csNetworkManager.UDPclient != null)
            csNetworkManager.UDPclient.setPos(this.transform.position.x, this.transform.position.y, this.transform.position.z,
                this.transform.rotation.eulerAngles.x, this.transform.rotation.eulerAngles.y, this.transform.rotation.eulerAngles.z);
    }

    void AnimUpdate()
    {
        // 점프 애니메이션
        if (Input.GetKey(KeyCode.LeftAlt))
            doJump();
        // 달리기 애니메이션
        else if (Input.GetKey(KeyCode.UpArrow) && movSpeed == 5.0f)
            doRun();

        // 걷기 애니메이션
        else if (Input.GetKey(KeyCode.UpArrow) && movSpeed == 2.0f)
            doWalk();
        // 뒤로 걷기 애니메이션
        else if (Input.GetKey(KeyCode.DownArrow))
            doBackWalk();
        // 평상시 애니메이션
        else
            doIdle();    
    }

    void doIdle()
    {
        anim.SetInteger("aniStep", 0);
        if (csNetworkManager.TCPclient != null && csNetworkManager.TCPclient.getAniNormal()[csNetworkManager.TCPclient.getMyID()].aniSet != 0)
            csNetworkManager.TCPclient.AnimationSendFunc(0);
    }

    void doWalk()
    {
        anim.SetInteger("aniStep", 1);
        if (csNetworkManager.TCPclient != null && csNetworkManager.TCPclient.getAniNormal()[csNetworkManager.TCPclient.getMyID()].aniSet != 1)
            csNetworkManager.TCPclient.AnimationSendFunc(1);
    }

    public void doRun()
    {
        anim.SetInteger("aniStep", 2);
        if (csNetworkManager.TCPclient != null && csNetworkManager.TCPclient.getAniNormal()[csNetworkManager.TCPclient.getMyID()].aniSet != 2)
            csNetworkManager.TCPclient.AnimationSendFunc(2);
    }

    void doJump()
    {
        anim.SetInteger("aniStep", 3);
        if (csNetworkManager.TCPclient != null && csNetworkManager.TCPclient.getAniNormal()[csNetworkManager.TCPclient.getMyID()].aniSet != 3)
            csNetworkManager.TCPclient.AnimationSendFunc(3);
    }

    void doBackWalk()
    {
        anim.SetInteger("aniStep", 4);
        if (csNetworkManager.TCPclient != null && csNetworkManager.TCPclient.getAniNormal()[csNetworkManager.TCPclient.getMyID()].aniSet != 4)
            csNetworkManager.TCPclient.AnimationSendFunc(4);
    }
}
