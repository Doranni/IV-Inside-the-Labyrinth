using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AnimationAndMovementController : MonoBehaviour
{
    [SerializeField] private float movementSpeed;
    [SerializeField] private float jumpUpForce, jumpForwardForce;
    [SerializeField] private LayerMask floorLM;

    private CharacterController characterController;
    private Animator animator;
    private InputManager inputManager;
    public float playerRadius => characterController.radius;

    private float movementForwardInput;
    private float rotationMouseInput, rotationKeyboardInput;
    private bool accelerationInput;

    private Vector3 moveDirectionGlobal, moveDirectionLocal;
    [SerializeField] private float moveGravity;

    [SerializeField] private float maxAcceleration = 2f,
        maxAccelerationTime = 1f;
    private float acceleration = 1f, accelerationStep;
    private float currentRotationSpeed;

    private bool isMovementLocked = false;
    private bool isFalling = false;
    private bool isJumpingUp = false;

    private EffectsListController effectsListController;
    Dictionary<int, Effect> effects = new Dictionary<int, Effect>();
    private Dictionary<int, Coroutine> effectCoroutines = new Dictionary<int, Coroutine>();
    private float speedEffectMultiplier = 1;

    private int animHashVecticalVelocity, animHashHorizontalVelocity, animHashAcceleration,
        animHashFallingVelocity, animHashAngle,
        animHashJump, animHashFalling, animHashLanding;

    [SerializeField] private AudioClip runningAudioClip;
    private AudioSource audioSource;
    [SerializeField] private float audioPitchMax = 1f, audioPitchMin = 0.55f;
    private float audioPitchStep;

    // For debug
    public bool isGrounded;

    private void Awake()
    {
        inputManager = InputManager.instance;

        inputManager.OnMoveForward_performed += context =>
        {
            movementForwardInput = context.ReadValue<float>();
        };
        inputManager.OnRotateMouse_performed += context =>
        {
            rotationMouseInput = context.ReadValue<float>();
        };
        inputManager.OnRotateKeyboard_performed += context =>
        {
            rotationKeyboardInput = context.ReadValue<float>();
        };

        inputManager.OnAcceleration_started += context =>
        {
            accelerationInput = context.ReadValueAsButton();
        };
        inputManager.OnAcceleration_canceled += context =>
        {
            accelerationInput = context.ReadValueAsButton();
        };

        inputManager.OnMouseRightClick_started += MouseRightClick_started;
        inputManager.OnMouseRightClick_canceled += MouseRightClick_canceled;

        inputManager.OnJumpUp_performed += JumpUp_performed;

        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        animHashVecticalVelocity = Animator.StringToHash("Vertical Velocity");
        animHashHorizontalVelocity = Animator.StringToHash("Horizontal Velocity");
        animHashAcceleration = Animator.StringToHash("Acceleration");
        animHashFallingVelocity = Animator.StringToHash("Falling Velocity");
        animHashAngle = Animator.StringToHash("Angle");
        animHashJump = Animator.StringToHash("Jump");
        animHashFalling = Animator.StringToHash("Falling");
        animHashLanding = Animator.StringToHash("Landing");
    }

    private void MouseRightClick_canceled(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        currentRotationSpeed = Preferences.plRotSpeed;
    }

    private void MouseRightClick_started(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if (Preferences.camRotStyle == Preferences.CameraRotationStyle.withRightClickMouse 
            && Preferences.plRotStyle == Preferences.PlayerRotationStyle.withMouse)
        {
            currentRotationSpeed = 0;
        } 
    }

    private void JumpUp_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if (!isMovementLocked && characterController.isGrounded)
        {
            SetMoveDirection();
            isMovementLocked = true;
            float rotAngle = 0;
            if (moveDirectionLocal.magnitude > 0)
            {
                moveDirectionGlobal = moveDirectionGlobal * jumpForwardForce + Vector3.up * jumpUpForce;

                // Jumping Forward
                if (moveDirectionLocal.z >= 0)
                {
                    if (Preferences.plRotStyle == Preferences.PlayerRotationStyle.withMouse)
                    {
                        rotAngle = Vector3.SignedAngle(Vector3.forward, moveDirectionLocal, Vector3.up);
                    }
                    animator.SetFloat(animHashFallingVelocity, 1);
                }
                // Jumping Backward
                else
                {
                    if (Preferences.plRotStyle == Preferences.PlayerRotationStyle.withMouse)
                    {
                        rotAngle = Vector3.SignedAngle(Vector3.back, moveDirectionLocal, Vector3.up);
                    }
                    animator.SetFloat(animHashFallingVelocity, -1);
                }
            }
            // Jumping Up
            else
            {
                moveDirectionGlobal = Vector3.up * jumpUpForce;
                animator.SetFloat(animHashFallingVelocity, 0);
            }
            animator.SetFloat(animHashAngle, rotAngle);
            animator.SetTrigger(animHashJump);
            if (audioSource.isPlaying)
            {
                audioSource.Stop();
            }
        }
    }

    void Start()
    {
        effectsListController = GetComponent<EffectsListController>();
        audioSource = GetComponent<AudioSource>();
        UpdatePlayerRotation();
        if (moveGravity >= 0)
        {
            moveGravity = Physics.gravity.y;
        }
        accelerationStep = (maxAcceleration - 1) / maxAccelerationTime;
        audioPitchStep = (audioPitchMax - audioPitchMin) / maxAccelerationTime;
        audioSource.pitch = audioPitchMin;
    }

    void Update()
    {
        isGrounded = characterController.isGrounded;
        if (isFalling)
        {
            if (characterController.isGrounded)
            {
                isFalling = false;
                animator.SetTrigger(animHashLanding);
                Debug.Log("Player: Landing");
            }
            else
            {
                HandleFalling();
            }
        }
        else if (isJumpingUp)
        {
            HandleJumpingUp();
        }
        else if (!isMovementLocked)
        {
            if (!characterController.isGrounded)
            {
                (Vector3 p1, Vector3 p2) capsulePoints = GetColliderPoints();

                // Falling with animation (from high distance)
                if (!Physics.CapsuleCast(capsulePoints.p1, capsulePoints.p2, characterController.radius,
                    Vector3.down, 1.5f, floorLM))
                {
                    StartFalling();
                }
                else
                {
                    HandleFalling();
                }
                if (audioSource.isPlaying)
                {
                    audioSource.Stop();
                }
            }
            else
            {
                HandleRotation();
                SetAcceleration();
                SetMoveDirection();
                HandleMovement();
            }
        }
    }

    private (Vector3 point1, Vector3 point2) GetColliderPoints()
    {
        Vector3 p1 = transform.position + characterController.center
                    - new Vector3(0, characterController.height * 0.5f - 0.5f, 0);
        Vector3 p2 = p1 + new Vector3(0, characterController.height - 1, 0);
        return (p1, p2);
    }

    private void HandleJumpingUp()
    {
        characterController.Move(moveDirectionGlobal * speedEffectMultiplier * Time.deltaTime);
        moveDirectionGlobal.y += moveGravity * Time.deltaTime;
    }

    private void HandleFalling()
    {
        characterController.Move(moveDirectionGlobal * Time.deltaTime);
        moveDirectionGlobal.y += moveGravity * Time.deltaTime;
    }

    private void StartFalling()
    {
        moveDirectionGlobal.y = moveGravity;
        isFalling = true;
        isMovementLocked = true;
        float rotAngle = 0;
        if (moveDirectionLocal.magnitude > 0)
        {
            // Falling Forward
            if (moveDirectionLocal.z >= 0)
            {
                if (Preferences.plRotStyle == Preferences.PlayerRotationStyle.withMouse)
                {
                    rotAngle = Vector3.SignedAngle(Vector3.forward, moveDirectionLocal, Vector3.up);
                }
                animator.SetFloat(animHashFallingVelocity, 1);
            }
            // Falling Backward
            else
            {
                if (Preferences.plRotStyle == Preferences.PlayerRotationStyle.withMouse)
                {
                    rotAngle = Vector3.SignedAngle(Vector3.back, moveDirectionLocal, Vector3.up);
                }
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
            switch (Preferences.plRotStyle)
            {
                case Preferences.PlayerRotationStyle.withMouse:
                    {
                        transform.Rotate(Vector3.up, Mathf.Lerp(0, rotationMouseInput * currentRotationSpeed,
                Time.deltaTime), Space.Self);
                        break;
                    }
                case Preferences.PlayerRotationStyle.withKeyboard:
                    {
                        transform.Rotate(Vector3.up, Mathf.Lerp(0, rotationKeyboardInput * currentRotationSpeed,
                Time.deltaTime), Space.Self);
                        break;
                    }
            }
            
        }
    }

    private void HandleMovement()
    {
        moveDirectionGlobal.y = moveGravity;
        moveDirectionGlobal *= movementSpeed * acceleration * speedEffectMultiplier * Time.deltaTime;
        characterController.Move(moveDirectionGlobal);

        animator.SetFloat(animHashVecticalVelocity, moveDirectionLocal.z);
        animator.SetFloat(animHashHorizontalVelocity, moveDirectionLocal.x);

        if (moveDirectionLocal.magnitude > 0)
        {
            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }
        }
        else if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }

    private void SetMoveDirection()
    {
        switch (Preferences.plRotStyle)
        {
            case Preferences.PlayerRotationStyle.withMouse:
                {
                    moveDirectionLocal = new Vector3(rotationKeyboardInput, 0, movementForwardInput).normalized;
                    break;
                }
            case Preferences.PlayerRotationStyle.withKeyboard:
                {
                    moveDirectionLocal = new Vector3(0, 0, movementForwardInput);
                    break;
                }
        }
        moveDirectionGlobal = transform.TransformDirection(moveDirectionLocal);
    }

    private void SetAcceleration()
    {
        // Dashing
        if (accelerationInput && characterController.isGrounded && acceleration < maxAcceleration)
        {
            acceleration += accelerationStep * Time.deltaTime;
            audioSource.pitch += audioPitchStep * Time.deltaTime;
        }
        else if (!accelerationInput && characterController.isGrounded && acceleration > 1f)
        {
            acceleration -= accelerationStep * Time.deltaTime;
            audioSource.pitch -= audioPitchStep * Time.deltaTime;
        }
        else if (accelerationInput && acceleration > maxAcceleration)
        {
            acceleration = maxAcceleration;
            audioSource.pitch = audioPitchMax;
        }
        else if (!accelerationInput && acceleration < 1f)
        {
            acceleration = 1f;
            audioSource.pitch = audioPitchMin;
        }
        animator.SetFloat(animHashAcceleration, (acceleration - 1) / (maxAcceleration - 1));
    }

    public void JumpUpStart()
    {
        isJumpingUp = true;
    }

    public void JumpUpFinish()
    {
        isJumpingUp = false;
        StartFalling();
    }

    public void RunningAudioStart()
    {
        if (!audioSource.isPlaying)
        {
            audioSource.time = 0.198f;
            audioSource.Play();
        }
    }

    public void RunningAudioFinish()
    {
        audioSource.time = 0f;
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }

    private void UpdateSpeedEffectMultiplier()
    {
        float res = 1;
        foreach (KeyValuePair<int, Effect> effect in effects)
        {
            res *= effect.Value.effectValue;
        }
        speedEffectMultiplier = res;
    }

    public void UpdatePlayerRotation()
    {
        currentRotationSpeed = Preferences.plRotSpeed;
    }

    public void UnlockMovement()
    {
        isMovementLocked = false;
    }

    public bool AddEffect(Effect effect)
    {
        Effect newEffect = new Effect(effect);
        if (effect.type != Effect.EffectType.movement)
        {
            return false;
        }
        int id = GameManager.NewId();
        id = effectsListController.Add(newEffect, id);
        effects.Add(id, newEffect);
        UpdateSpeedEffectMultiplier();
        effectCoroutines.Add(id, StartCoroutine(EffectRoutine(id)));
        return true;
    }

    private IEnumerator EffectRoutine(int id)
    {
        while (effects[id].TimeLeft > 0)
        {
            yield return new WaitForSeconds(1);
            effects[id].ChangeTimeLeft(-1);
            effectsListController.UpdateEffects();
        }
        effectsListController.RemoveEffect(id);
        speedEffectMultiplier = 1;
        effects.Remove(id);
        UpdateSpeedEffectMultiplier();
        StopCoroutine(effectCoroutines[id]);
        effectCoroutines.Remove(id);
    }
}