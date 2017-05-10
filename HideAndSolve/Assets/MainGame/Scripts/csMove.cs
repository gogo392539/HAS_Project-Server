using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Client;

public class csMove : MonoBehaviour {

    public Slider staminaBarSlider;

    public float movSpeed = 5.0f;
    public float rotSpeed = 120.0f;

    public static CharacterController controller;
    Vector3 moveDirection;

    float jumpSpeed = 10.0f;
    float gravity = 20.0f;

    // Use this for initialization
    private void Start () {
        controller = GetComponent<CharacterController>();
    }
	
	// Update is called once per frame
	private void Update () {
        float amtRot = rotSpeed * Time.deltaTime;
        float ang = Input.GetAxis("Horizontal");

        if (controller != null)
        {
            transform.Rotate(Vector3.up * ang * amtRot);
            if (controller.isGrounded)
            {
                float ver = Input.GetAxis("Vertical");

                if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.UpArrow) && staminaBarSlider.value > 0)
                    movSpeed = 10.0f;
                else
                    movSpeed = 5.0f;

                moveDirection = new Vector3(0, 0, ver * movSpeed);
                moveDirection = transform.TransformDirection(moveDirection);

                if (Input.GetKey(KeyCode.LeftAlt))
                    moveDirection.y = jumpSpeed;

                if (Input.GetKey(KeyCode.UpArrow) && movSpeed == 10.0f && staminaBarSlider.value > 0)
                    staminaBarSlider.value -= 1;
                else if (staminaBarSlider.value <= 100 && !Input.GetKey(KeyCode.LeftShift))
                    staminaBarSlider.value += .3f;
            }

            moveDirection.y -= gravity * Time.deltaTime;

            controller.Move(moveDirection * Time.deltaTime);
        }
    }

    private void LateUpdate()
    {
        csNetworkManager.UDPclient.setPos(this.transform.position.x, this.transform.position.y, this.transform.position.z,
            this.transform.rotation.eulerAngles.x, this.transform.rotation.eulerAngles.y, this.transform.rotation.eulerAngles.z);
    }

}
