using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [Header("Speeds and heights")]
    [SerializeField] float walkSpeed = 3.5f;
    [SerializeField] float crouchSpeed = 1.5f;
    [SerializeField] float jumpHeight = 1;
    [SerializeField] float sprintSpeed = 5;


    [Header("Character Heights")]
    [SerializeField] float normalheight = 2;
    [SerializeField] float crouchedHeight = 1.5f;


    [Header("Gravity and ground check")]
    [SerializeField] float gravity = -9.81f;
    [SerializeField] float groundDistance = 0.2f;
    [SerializeField] Transform groundCheck;
    [SerializeField] LayerMask groundLayers;

    [Header("Slopes")]
    [SerializeField] bool isOnSlope;


    CharacterController controller;

    Vector3 velocity;
    bool isGrounded;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        Gravity();
        Move();
        Jump();
        ExecuteMovement();
    }

    void Gravity()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundLayers);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -1;
        }

    }

    void Move()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");


        Vector3 move = transform.right * x + transform.forward * z;
        if (Input.GetButton("Crouch"))
        {
            controller.Move(move * crouchSpeed * Time.deltaTime);
            controller.height = crouchedHeight;

        }

        else
        {
            controller.Move(move * walkSpeed * Time.deltaTime);
            controller.height = normalheight;
        }

    }

    void Jump()
    {
        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2 * gravity);
            Debug.Log("Jump");
        }


    }

    void ExecuteMovement()
    {
        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
    }

    bool IsOnSlope()
    {
        if (isGrounded && Physics.Raycast(transform.position, Vector3.down, out RaycastHit slopeHit, 2f))
        {
            Vector3 hitPointNormal = slopeHit.normal;
            Debug.Log(true);

            if (Vector3.Angle(hitPointNormal, Vector3.up) < 1)
            {
                return false;
            }
            else
            {
                return true; ;
            }

        }

        else
        {
            if (isGrounded)
            {
                return true;
            }

            else
            {
                return false;
            }

        }


    }


}
