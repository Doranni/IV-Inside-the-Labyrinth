using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationAndMovementController : MonoBehaviour
{
    [SerializeField] private float movementSpeed, rotationSpeed;
    [SerializeField] private float jumpUpForce, jumpAsideForce;
    [SerializeField] private LayerMask floorLM;

    private CharacterController characterController;
    private Animator animator;
    private GameInput gameInput;

    private float rotationInput, verticalInput;
    private bool accelerationInput;

    private Vector3 moveDirection, jumpDirection;

    private float acceleration = 1f;
    private float maxAcceleration = 2f;

    private bool isMovementLocked = false;
    private bool isFalling = false;
    private bool isJumpingUp = false;

    int animHashVelocity, animHashJump, animHashFalling, animHashLanding;

    private void Awake()
    {
        gameInput = new GameInput();

        gameInput.Player.Move.started += context =>
        {
            verticalInput = context.ReadValue<float>();
        };
        gameInput.Player.Move.performed += context =>
        {
            verticalInput = context.ReadValue<float>();
        };
        gameInput.Player.Move.canceled += context =>
        {
            verticalInput = context.ReadValue<float>();
        };

        gameInput.Player.Rotate.started += context =>
        {
            rotationInput = context.ReadValue<float>();
        };
        gameInput.Player.Rotate.performed += context =>
        {
            rotationInput = context.ReadValue<float>();
        };
        gameInput.Player.Rotate.canceled += context =>
        {
            rotationInput = context.ReadValue<float>();
        };

        gameInput.Player.Acceleration.started += context =>
        {
            accelerationInput = context.ReadValueAsButton();
        };
        gameInput.Player.Acceleration.canceled += context =>
        {
            accelerationInput = context.ReadValueAsButton();
        };

        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        animHashVelocity = Animator.StringToHash("Velocity");
        animHashJump = Animator.StringToHash("Jump");
        animHashFalling = Animator.StringToHash("Falling");
        animHashLanding = Animator.StringToHash("Landing");
    }

    void Start()
    {
        characterController.Move(Vector3.down);
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
            characterController.Move(jumpDirection * Time.deltaTime);
        }
        else if (!isMovementLocked)
        {
            if (!characterController.isGrounded)
            {
                Vector3 p1 = transform.position + characterController.center
                    - new Vector3(0, characterController.height * 0.5f - 0.5f, 0);
                Vector3 p2 = p1 + new Vector3(0, characterController.height - 1, 0);
                if (!Physics.CapsuleCast(p1, p2, characterController.radius,
                    Vector3.down, 2f, floorLM))
                {
                    isFalling = true;
                    isMovementLocked = true;
                    animator.SetTrigger(animHashFalling);
                }
            }
            else
            {
                HandleRotation();
                SetAcceleration();
                HandleMovement();
            }
        }

        // Debug
        if (Input.GetKeyDown(KeyCode.G))
        {
            Debug.Log("Player: isGrounded - " + characterController.isGrounded);
        }
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
        if (gameInput.Player.JumpUp.WasPressedThisFrame())
        {
            isMovementLocked = true;
            jumpDirection = transform.forward * verticalInput * movementSpeed
                * acceleration + Vector3.up * jumpUpForce;
            animator.SetTrigger(animHashJump);
        }
        // Jumping left
        else if (gameInput.Player.JumpLeft.WasPressedThisFrame())
        {

        }
        // Jumping right
        else if (gameInput.Player.JumpRight.WasPressedThisFrame())
        {

        }
        else
        {
            moveDirection = transform.forward * verticalInput * movementSpeed * acceleration;
            characterController.SimpleMove(moveDirection);
            animator.SetFloat(animHashVelocity, verticalInput * acceleration);
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
        Debug.Log($"Player is Jumping");
    }

    public void JumpUpFinish()
    {
        isJumpingUp = false;
        isFalling = true;
        Debug.Log($"Player is Jumping: velocity - {animator.GetFloat(animHashVelocity)}, high - {transform.position.y}");
    }

    public void UnlockMovement()
    {
        isMovementLocked = false;
    }

    private void OnEnable()
    {
        gameInput.Enable();
    }

    private void OnDisable()
    {
        gameInput.Disable();
    }
}