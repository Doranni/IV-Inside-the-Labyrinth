using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationAndMovementControllerRM : MonoBehaviour
{
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float jumpingUpForce;
    [SerializeField] private float jumpingAsideForce;

    private CharacterController characterController;
    private Animator animator;

    private float rotationInput;
    private float verticalInput;
    private bool isDoubleJumpAvailable = false;
    private bool isOnDash;
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
        SetMoveDirection();
        //HandleGravity();
        HandleAnimation();
    }

    private void HandleGravity()
    {
        if (characterController.isGrounded)
        {
            moveDirection.y = -0.05f;
        }
        else
        {
            moveDirection.y = gravity;
        }
        characterController.Move(moveDirection * Time.deltaTime);
    }

    private void HandleRotation()
    {
        transform.Rotate(Vector3.up * rotationInput * rotationSpeed * Time.deltaTime);
    }

    private void SetMoveDirection()
    {
        moveDirection.z = verticalInput;
    }

    private void HandleAnimation()
    {
        animator.SetFloat("Velocity", moveDirection.z);
        if (Input.GetKeyDown(KeyCode.Space))
        {
            animator.SetTrigger("Jump");
        }
    }
}
