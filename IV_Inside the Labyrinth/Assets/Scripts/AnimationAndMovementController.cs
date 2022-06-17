using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AnimationAndMovementController : MonoBehaviour
{
    [SerializeField] private float movementSpeed, rotationSpeed;
    [SerializeField] private float jumpUpForce, jumpForwardForce;
    [SerializeField] private LayerMask floorLM;

    [SerializeField] private TMP_Text mouseDeltaValue, mousePositionValue;

    private CharacterController characterController;
    private Animator animator;
    private PlayerInput playerInput;

    private Vector2 movementInput;
    private float rotationInput;
    private bool accelerationInput;

    private Vector3 moveDirection;
    private float moveGravity;

    private float acceleration = 1f;
    private float maxAcceleration = 2f;

    private bool isMovementLocked = false;
    private bool isFalling = false;
    private bool isJumpingUp = false;

    private int animHashVecticalVelocity, animHashHorizontalVelocity,
        animHashFallingVelocity, animHashAngle,
        animHashJump, animHashFalling, animHashLanding;

    private void Awake()
    {
        playerInput = new PlayerInput();

        playerInput.Player.Move.started += context =>
        {
            movementInput = context.ReadValue<Vector2>();
        };
        playerInput.Player.Move.performed += context =>
        {
            movementInput = context.ReadValue<Vector2>();
        };
        playerInput.Player.Move.canceled += context =>
        {
            movementInput = context.ReadValue<Vector2>();
        };

        playerInput.Player.Rotate.started += context =>
        {
            rotationInput = context.ReadValue<float>();
            mouseDeltaValue.text = rotationInput.ToString();
        };
        playerInput.Player.Rotate.performed += context =>
        {
            rotationInput = context.ReadValue<float>();
            mouseDeltaValue.text = rotationInput.ToString();
        };
        playerInput.Player.Rotate.canceled += context =>
        {
            rotationInput = context.ReadValue<float>();
            mouseDeltaValue.text = rotationInput.ToString();
        };

        playerInput.Player.Acceleration.started += context =>
        {
            accelerationInput = context.ReadValueAsButton();
        };
        playerInput.Player.Acceleration.canceled += context =>
        {
            accelerationInput = context.ReadValueAsButton();
        };

        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        animHashVecticalVelocity = Animator.StringToHash("Vertical Velocity");
        animHashHorizontalVelocity = Animator.StringToHash("Horizontal Velocity");
        animHashFallingVelocity = Animator.StringToHash("Falling Velocity");
        animHashAngle = Animator.StringToHash("Angle");
        animHashJump = Animator.StringToHash("Jump");
        animHashFalling = Animator.StringToHash("Falling");
        animHashLanding = Animator.StringToHash("Landing");
    }

    void Start()
    {
        moveGravity = Physics.gravity.y;
        characterController.Move(Vector3.down);
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if (isFalling)
        {
            if (characterController.isGrounded)
            {
                isFalling = false;
                animator.SetTrigger(animHashLanding);
            }
        }
        else if (isJumpingUp)
        {
            HandleJumping();
        }
        else if (!isMovementLocked)
        {
            if (!characterController.isGrounded)
            {
                Vector3 p1 = transform.position + characterController.center
                    - new Vector3(0, characterController.height * 0.5f - 0.5f, 0);
                Vector3 p2 = p1 + new Vector3(0, characterController.height - 1, 0);

                // Falling
                if (!Physics.CapsuleCast(p1, p2, characterController.radius,
                    Vector3.down, 2f, floorLM))
                {
                    HandleFalling();
                }
            }
            else
            {
                HandleRotation();
                SetAcceleration();
                HandleMovement();
            }
        }
    }

    private void HandleJumping()
    {
        characterController.Move(moveDirection * Time.deltaTime);
    }

    private void HandleFalling()
    {
        isFalling = true;
        isMovementLocked = true;
        float rotAngle = 0;
        if (movementInput.magnitude > 0)
        {
            Vector3 movementInputVec3 = new Vector3(movementInput.x, 0, movementInput.y);

            // Falling Forward
            if (movementInput.y >= 0)
            {
                rotAngle = Vector3.SignedAngle(Vector3.forward, movementInputVec3, Vector3.up);
                animator.SetFloat(animHashFallingVelocity, 1);
            }
            // Falling Backward
            else
            {
                rotAngle = Vector3.SignedAngle(Vector3.back, movementInputVec3, Vector3.up);
                animator.SetFloat(animHashFallingVelocity, -1);
            }
        }
        // Falling Up
        else
        {
            animator.SetFloat(animHashFallingVelocity, 0);
        }
        animator.SetFloat(animHashAngle, rotAngle);
        animator.SetTrigger(animHashFalling);
    }

    private void HandleRotation()
    {
        if (characterController.isGrounded)
        {
            transform.Rotate(Vector3.up, Mathf.Lerp(0, rotationInput * rotationSpeed,
                Time.deltaTime), Space.Self);
        }
    }

    private void HandleMovement()
    {
        Vector3 movementInputVec3 = new Vector3(movementInput.x, 0, movementInput.y) * acceleration;
        moveDirection = transform.TransformDirection(movementInputVec3);

        // Jumping
        if (playerInput.Player.JumpUp.WasPressedThisFrame())
        {
            isMovementLocked = true;
            float rotAngle = 0;
            if (movementInput.magnitude > 0)
            {
                moveDirection = moveDirection * jumpForwardForce + Vector3.up * jumpUpForce;

                // Jumping Forward
                if (movementInput.y >= 0)
                {
                    rotAngle = Vector3.SignedAngle(Vector3.forward, movementInputVec3, Vector3.up);
                    animator.SetFloat(animHashFallingVelocity, 1);
                }
                // Jumping Backward
                else
                {
                    rotAngle = Vector3.SignedAngle(Vector3.back, movementInputVec3, Vector3.up);
                    animator.SetFloat(animHashFallingVelocity, -1);
                }
            }
            // Jumping Up
            else
            {
                moveDirection = Vector3.up * jumpUpForce;
                animator.SetFloat(animHashFallingVelocity, 0);
            }
            animator.SetFloat(animHashAngle, rotAngle);
            animator.SetTrigger(animHashJump);
        }
        // Running
        else
        {
            moveDirection.y = moveGravity;
            moveDirection *= movementSpeed * Time.deltaTime;
            characterController.Move(moveDirection);

            animator.SetFloat(animHashVecticalVelocity, movementInputVec3.z);
            animator.SetFloat(animHashHorizontalVelocity, movementInputVec3.x);
        }
    }

    private void SetAcceleration()
    {
        // Dashing
        if (accelerationInput && characterController.isGrounded && acceleration < maxAcceleration)
        {
            acceleration += Time.deltaTime;
        }
        else if (!accelerationInput && characterController.isGrounded && acceleration > 1f)
        {
            acceleration -= Time.deltaTime;
        }
        else if (accelerationInput && acceleration > maxAcceleration)
        {
            acceleration = maxAcceleration;
        }
        else if (!accelerationInput && acceleration < 1f)
        {
            acceleration = 1f;
        }
    }

    public void JumpUpStart()
    {
        isJumpingUp = true;
    }

    public void JumpUpFinish()
    {
        isJumpingUp = false;
        isFalling = true;

        // Debug
        Debug.Log($"Player is Jumping: velocity - {animator.GetFloat(animHashVecticalVelocity)}, high - {transform.position.y}");
    }

    public void UnlockMovement()
    {
        isMovementLocked = false;
    }

    private void OnEnable()
    {
        playerInput.Enable();
    }

    private void OnDisable()
    {
        playerInput.Disable();
    }
}