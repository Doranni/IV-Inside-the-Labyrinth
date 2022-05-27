using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationAndMovementController : MonoBehaviour
{
    [SerializeField] private float movementSpeed;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float jumpingHight;
    [SerializeField] private float jumpingAsideForce;
    [SerializeField] private float addGravity;

    private CharacterController characterController;
    private Animator animator;

    private float rotationInput;
    private float verticalInput;
    private bool isOnDash = false;
    private float acceleration = 1f;
    private float maxAcceleration = 2f;
    private float gravity;
    private Vector3 moveDirection;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        gravity = Physics.gravity.y;
    }

    void Update()
    {
        rotationInput = Input.GetAxis("Mouse X");
        verticalInput = Input.GetAxis("Vertical");

        HandleRotation();
        SetAcceleration();
        HandleAnimation();
    }

    private void FixedUpdate()
    {
        SetMoveDirection();
        characterController.Move(moveDirection * Time.fixedDeltaTime);
    }

    private void HandleRotation()
    {
        if (characterController.isGrounded)
        {
            transform.Rotate(Vector3.up * rotationInput * rotationSpeed * Time.deltaTime);
        }
    }

    private void SetMoveDirection()
    {
        if (characterController.isGrounded)
        {
            moveDirection = transform.forward * verticalInput * movementSpeed * acceleration;

            // Jumping 
            if (Input.GetButtonDown("Jump"))
            {
                moveDirection.y = jumpingHight;
            }
            // Jumping left
            else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            {
                moveDirection = moveDirection.normalized + transform.right * -1;
                moveDirection *= jumpingAsideForce;
                moveDirection.y = jumpingHight;
            }
            // Jumping right
            else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                moveDirection = moveDirection.normalized + transform.right;
                moveDirection *= jumpingAsideForce;
                moveDirection.y = jumpingHight;
            }
            else
            {
                moveDirection.y = gravity;
            }
        }
        // Falling
        else
        {
            moveDirection.y += addGravity * Time.fixedDeltaTime;
        }
    }

    private void HandleAnimation()
    {
        if (characterController.isGrounded)
        {
            animator.SetFloat("Velocity", verticalInput * acceleration);
        }
        else
        {
            animator.SetFloat("Velocity", 0);
        }
    }

    private void SetAcceleration()
    {
        // Dashing
        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
        {
            isOnDash = true;
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift) || Input.GetKeyUp(KeyCode.RightShift))
        {
            isOnDash = false;
        }
        if (isOnDash && characterController.isGrounded && acceleration < maxAcceleration)
        {
            acceleration += Time.deltaTime;
        }
        else if (!isOnDash && characterController.isGrounded && acceleration > 1f)
        {
            acceleration -= Time.deltaTime;
        }
        else if (isOnDash && acceleration > maxAcceleration)
        {
            acceleration = maxAcceleration;
        }
        else if (!isOnDash && acceleration < 1f)
        {
            acceleration = 1f;
        }
    }
}