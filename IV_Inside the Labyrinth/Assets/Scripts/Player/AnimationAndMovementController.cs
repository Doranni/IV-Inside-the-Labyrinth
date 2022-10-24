using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AnimationAndMovementController : MonoBehaviour
{
    [SerializeField] private float movementSpeed;
    [SerializeField] private float jumpUpForce, jumpForwardForce;
    [SerializeField] private LayerMask floorLM;
    [SerializeField] private CameraController cameraController;
    private CharacterController characterController;

    private Vector3 moveDirectionGlobal, moveDirectionLocal;
    [SerializeField] private float moveGravity;

    [SerializeField] private float maxAcceleration = 2f,
        maxAccelerationTime = 1f;
    private float acceleration = 1f, accelerationStep;
    [SerializeField] private float maxRotationAcceleration = 1f;
    private float rotationAcceleration = 1f, rotationAccelerationStep;
    private float currentRotationSpeed;
    // Min agle between player's forward and camera's forward to start player's rotation 
    // for PlayerRotationStyle - withMouse and CameraRotationStyle - followPlayer
    // (actuallt it's player follows camera)
    [SerializeField] private float plCamMinAngleForRotation = 25;
    private Transform mainCamera;
    // For debug
    [SerializeField] private Vector3 camerasForward, playersForward;
    [SerializeField] private float plCamAngle;
    [SerializeField] private bool isGrounded;

    public enum MovementState
    {
        still,
        moving,
        falling,
        jumpingUp,
        underAnimation
    }

    [SerializeField] private MovementState moveState = MovementState.still;
    public MovementState MoveState => moveState;

    // Input
    private float movementForwardInput;
    private float rotationMouseInput, rotationKeyboardInput;
    private bool accelerationInput;

    // Movement effects
    private EffectsListController effectsListController;
    Dictionary<int, Effect> effects = new Dictionary<int, Effect>();
    private Dictionary<int, Coroutine> effectCoroutines = new Dictionary<int, Coroutine>();
    private float speedEffectMultiplier = 1;

    // Animation
    private Animator animator;
    private int animHash_VecticalVelocity_Float, animHash_HorizontalVelocity_Float,
        animHash_Acceleration_Float,
        animHash_FallingVelocity_Float, animHash_Angle_Float,
        animHash_Jump_Trigger, animHash_Falling_Trigger, animHash_Landing_Trigger;

    // Audio
    [SerializeField] private AudioClip runningAudioClip;
    private AudioSource audioSource;
    [SerializeField] private float audioPitchMax = 1f, audioPitchMin = 0.55f;
    private float audioPitchStep;

    private void Awake()
    {
        InputManager.instance.OnMoveForward_performed += context =>
        {
            movementForwardInput = context.ReadValue<float>();
        };
        InputManager.instance.OnMoveForward_started += context =>
        {
            switch (Preferences.plRotStyle)
            {
                case Preferences.PlayerRotationStyle.withMouse:
                    {
                        if (moveState == MovementState.still)
                        {
                            moveState = MovementState.moving;
                            if (Preferences.camRotStyle == Preferences.CameraRotationStyle.withRightClickMouse)
                            {
                                cameraController.UpdateViewMode();
                            }
                        }
                        break;
                    }
                case Preferences.PlayerRotationStyle.withKeyboard:
                    {
                        if (moveState == MovementState.still)
                        {
                            moveState = MovementState.moving;
                        }
                        cameraController.UpdateViewMode();
                        break;
                    }
            }
        };
        InputManager.instance.OnMoveForward_canceled += context =>
        {
            switch (Preferences.plRotStyle)
            {
                case Preferences.PlayerRotationStyle.withMouse:
                    {
                        if (moveState == MovementState.moving && rotationKeyboardInput == 0)
                        {
                            moveState = MovementState.still;
                            if (Preferences.camRotStyle == Preferences.CameraRotationStyle.withRightClickMouse)
                            {
                                cameraController.UpdateViewMode();
                            }
                        }
                        break;
                    }
                case Preferences.PlayerRotationStyle.withKeyboard:
                    {
                        if (moveState == MovementState.moving)
                        {
                            moveState = MovementState.still;
                        }
                        cameraController.UpdateViewMode();
                        break;
                    }
            }
        };
        InputManager.instance.OnMoveAside_started += context =>
        {
            switch (Preferences.plRotStyle)
            {
                case Preferences.PlayerRotationStyle.withMouse:
                    {
                        if (moveState == MovementState.still)
                        {
                            moveState = MovementState.moving;
                            if (Preferences.camRotStyle == Preferences.CameraRotationStyle.withRightClickMouse)
                            {
                                cameraController.UpdateViewMode();
                            }
                        }
                        break;
                    }
                case Preferences.PlayerRotationStyle.withKeyboard:
                    {
                        break;
                    }
            }
        };
        InputManager.instance.OnMoveAside_canceled += context =>
        {
            switch (Preferences.plRotStyle)
            {
                case Preferences.PlayerRotationStyle.withMouse:
                    {
                        if (moveState == MovementState.moving && movementForwardInput == 0)
                        {
                            moveState = MovementState.still;
                            if (Preferences.camRotStyle == Preferences.CameraRotationStyle.withRightClickMouse)
                            {
                                cameraController.UpdateViewMode();
                            }
                        }
                        break;
                    }
                case Preferences.PlayerRotationStyle.withKeyboard:
                    {
                        break;
                    }
            }
        };

        InputManager.instance.OnRotateMouse_performed += context =>
        {
            rotationMouseInput = context.ReadValue<float>();
        };
        InputManager.instance.OnRotateKeyboard_performed += context =>
        {
            rotationKeyboardInput = context.ReadValue<float>();
        };

        InputManager.instance.OnAcceleration_started += context =>
        {
            accelerationInput = context.ReadValueAsButton();
        };
        InputManager.instance.OnAcceleration_canceled += context =>
        {
            accelerationInput = context.ReadValueAsButton();
        };

        InputManager.instance.OnMouseRightClick_started += MouseRightClick_started;
        InputManager.instance.OnMouseRightClick_canceled += MouseRightClick_canceled;

        InputManager.instance.OnJumpUp_performed += JumpUp_performed;

        // TODO: to check to i need it
        Preferences.OnPlRotStyleChanged += UpdatePlayerRotation;
        Preferences.OnCamRotStyleChanged += UpdatePlayerRotation;
        Preferences.OnPlRotSpeedChanged += UpdatePlayerRotation;

        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        animHash_VecticalVelocity_Float = Animator.StringToHash("Vertical Velocity");
        animHash_HorizontalVelocity_Float = Animator.StringToHash("Horizontal Velocity");
        animHash_Acceleration_Float = Animator.StringToHash("Acceleration");
        animHash_FallingVelocity_Float = Animator.StringToHash("Falling Velocity");
        animHash_Angle_Float = Animator.StringToHash("Angle");
        animHash_Jump_Trigger = Animator.StringToHash("Jump");
        animHash_Falling_Trigger = Animator.StringToHash("Falling");
        animHash_Landing_Trigger = Animator.StringToHash("Landing");
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
        if ((moveState == MovementState.still || moveState == MovementState.moving)
            && characterController.isGrounded)
        {
            SetMoveDirection();
            moveState = MovementState.underAnimation;
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
                    animator.SetFloat(animHash_FallingVelocity_Float, 1);
                }
                // Jumping Backward
                else
                {
                    if (Preferences.plRotStyle == Preferences.PlayerRotationStyle.withMouse)
                    {
                        rotAngle = Vector3.SignedAngle(Vector3.back, moveDirectionLocal, Vector3.up);
                    }
                    animator.SetFloat(animHash_FallingVelocity_Float, -1);
                }
            }
            // Jumping Up
            else
            {
                moveDirectionGlobal = Vector3.up * jumpUpForce;
                animator.SetFloat(animHash_FallingVelocity_Float, 0);
            }
            animator.SetFloat(animHash_Angle_Float, rotAngle);
            animator.SetTrigger(animHash_Jump_Trigger);
            if (audioSource.isPlaying)
            {
                audioSource.Stop();
            }
        }
    }

    void Start()
    {
        mainCamera = Camera.main.transform;
        effectsListController = GetComponent<EffectsListController>();
        audioSource = GetComponent<AudioSource>();
        UpdatePlayerRotation();
        if (moveGravity >= 0)
        {
            moveGravity = Physics.gravity.y;
        }
        accelerationStep = (maxAcceleration - 1) / maxAccelerationTime;
        rotationAccelerationStep = (maxRotationAcceleration - 1) / maxAccelerationTime;
        audioPitchStep = (audioPitchMax - audioPitchMin) / maxAccelerationTime;
        audioSource.pitch = audioPitchMin;
    }

    void Update()
    {
        // For debug
        isGrounded = characterController.isGrounded;
        camerasForward = mainCamera.forward;
        playersForward = transform.forward;
        plCamAngle = Vector3.SignedAngle(playersForward, camerasForward, Vector3.up);

        if (moveState == MovementState.falling)
        {
            if (characterController.isGrounded)
            {
                moveState = MovementState.underAnimation;
                animator.SetTrigger(animHash_Landing_Trigger);
            }
            else
            {
                HandleFalling();
            }
        }
        else if (moveState == MovementState.jumpingUp)
        {
            HandleJumpingUp();
        }
        else if (moveState != MovementState.underAnimation)
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
        moveState = MovementState.falling;
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
                animator.SetFloat(animHash_FallingVelocity_Float, 1);
            }
            // Falling Backward
            else
            {
                if (Preferences.plRotStyle == Preferences.PlayerRotationStyle.withMouse)
                {
                    rotAngle = Vector3.SignedAngle(Vector3.back, moveDirectionLocal, Vector3.up);
                }
                animator.SetFloat(animHash_FallingVelocity_Float, -1);
            }
        }
        // Falling Up
        else
        {
            animator.SetFloat(animHash_FallingVelocity_Float, 0);
        }
        animator.SetFloat(animHash_Angle_Float, rotAngle);
        animator.SetTrigger(animHash_Falling_Trigger);
    }

    private void HandleRotation()
    {
        if (characterController.isGrounded && 
            (moveState == MovementState.moving || moveState == MovementState.still))
        {
            switch (Preferences.plRotStyle)
            {
                case Preferences.PlayerRotationStyle.withMouse:
                    {
                        if (moveState == MovementState.moving)
                        {
                            if (Preferences.camRotStyle == Preferences.CameraRotationStyle.followPlayer
                                && cameraController.VMode == CameraController.ViewMode.thirdViewMode)
                            {
                                float angleClamped = Mathf.Clamp(plCamAngle, -plCamMinAngleForRotation, plCamMinAngleForRotation);
                                if (angleClamped == -plCamMinAngleForRotation || angleClamped == plCamMinAngleForRotation)
                                {
                                    transform.Rotate(Vector3.up, Mathf.Lerp(0,
                                    angleClamped / plCamMinAngleForRotation * currentRotationSpeed * rotationAcceleration,
                                    Time.deltaTime), Space.Self);
                                }
                            }
                            else
                            {
                                transform.Rotate(Vector3.up, Mathf.Lerp(0,
                                    rotationMouseInput * currentRotationSpeed * rotationAcceleration,
                                    Time.deltaTime), Space.Self);
                            }
                        }
                        break;
                    }
                case Preferences.PlayerRotationStyle.withKeyboard:
                    {
                        transform.Rotate(Vector3.up, Mathf.Lerp(0,
                            rotationKeyboardInput * currentRotationSpeed * rotationAcceleration,
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

        animator.SetFloat(animHash_VecticalVelocity_Float, moveDirectionLocal.z);
        animator.SetFloat(animHash_HorizontalVelocity_Float, moveDirectionLocal.x);

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
                    moveDirectionLocal = new Vector3(0, 0, movementForwardInput).normalized;
                    break;
                }
        }
        moveDirectionGlobal = transform.TransformDirection(moveDirectionLocal);
    }

    private void SetAcceleration()
    {
        // Dashing
        if (accelerationInput && moveState == MovementState.moving && acceleration < maxAcceleration)
        {
            acceleration += accelerationStep * Time.deltaTime;
            rotationAcceleration += rotationAccelerationStep * Time.deltaTime;
            audioSource.pitch += audioPitchStep * Time.deltaTime;
        }
        else if ((!accelerationInput || moveState != MovementState.moving) && acceleration > 1f)
        {
            acceleration -= accelerationStep * Time.deltaTime;
            rotationAcceleration -= rotationAccelerationStep * Time.deltaTime;
            audioSource.pitch -= audioPitchStep * Time.deltaTime;
        }
        else if (accelerationInput && acceleration > maxAcceleration)
        {
            acceleration = maxAcceleration;
            rotationAcceleration = maxRotationAcceleration;
            audioSource.pitch = audioPitchMax;
        }
        else if (!accelerationInput && acceleration < 1f)
        {
            acceleration = 1f;
            rotationAcceleration = 1f;
            audioSource.pitch = audioPitchMin;
        }
        animator.SetFloat(animHash_Acceleration_Float, (acceleration - 1) / (maxAcceleration - 1));
    }

    public void JumpUpStart()
    {
        moveState = MovementState.jumpingUp;
    }

    public void JumpUpFinish()
    {
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
        if (movementForwardInput == 0 && rotationKeyboardInput == 0)
        {
            moveState = MovementState.still;
        }
        else
        {
            moveState = MovementState.moving;
        }
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

    private void OnDestroy()
    {
        Preferences.OnPlRotStyleChanged -= UpdatePlayerRotation;
        Preferences.OnPlRotSpeedChanged -= UpdatePlayerRotation;
        Preferences.OnCamRotStyleChanged -= UpdatePlayerRotation;
    }
}