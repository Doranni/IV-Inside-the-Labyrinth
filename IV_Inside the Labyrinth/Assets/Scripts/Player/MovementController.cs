using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController : MonoBehaviour
{
    public MovementStateMachine StateMachine { get; private set; }

    [SerializeField] private LayerMask floorLM;
    public LayerMask FloorLM => floorLM;

    [SerializeField] private CameraController cameraController;
    private CharacterController characterController;

    [SerializeField] private float movementSpeed, jumpUpForce, jumpForwardForce, moveGravity,
        maxAcceleration = 2f, maxRotationAcceleration = 1f, maxAccelerationTime = 1f;

    private float acceleration = 1f, accelerationStep,
        rotationAcceleration = 1f, rotationAccelerationStep;
    private float currentRotationSpeed;

    public float MovementSpeed => movementSpeed;
    public float JumpUpForce => jumpUpForce;
    public float JumpForwardForce => jumpForwardForce;
    public float MoveGravity => moveGravity;
    public float Acceleration => acceleration;
    public float RotationAcceleration => rotationAcceleration;
    public float CurrentRotationSpeed => currentRotationSpeed;

    public Vector3 moveDirectionGlobal;
    public Vector3 moveDirectionLocal;
    
    // Min agle between player's forward and camera's forward to start player's rotation 
    // for PlayerRotationStyle - withMouse and CameraRotationStyle - followPlayer
    // (actuallt it's player follows camera)
    [SerializeField] private float plCamMinAngleForRotation = 25;

    public float PlCamMinAngleForRotation => plCamMinAngleForRotation;

    // Input
    private float movementForwardInput;
    private float rotationMouseInput, rotationKeyboardInput;
    private bool accelerationInput;
    public float MovementForwardInput => movementForwardInput;
    public float RotationMouseInput => rotationMouseInput;
    public float RotationKeyboardInput => rotationKeyboardInput;
    public bool AccelerationInput => accelerationInput;

    // Movement effects
    private EffectsListController effectsListController;
    readonly Dictionary<int, Effect> effects = new Dictionary<int, Effect>();
    private readonly Dictionary<int, Coroutine> effectCoroutines = new Dictionary<int, Coroutine>();
    private float speedEffectMultiplier = 1;
    public float SpeedEffectMultiplier => speedEffectMultiplier;

    // Animation
    private Animator animator;
    private int animHash_VecticalVelocity_Float, animHash_HorizontalVelocity_Float,
        animHash_Acceleration_Float,
        animHash_FallingVelocity_Float, animHash_Angle_Float,
        animHash_Jump_Trigger, animHash_Falling_Trigger, animHash_Landing_Trigger;
    public int AnimHash_VecticalVelocity_Float => animHash_VecticalVelocity_Float;
    public int AnimHash_HorizontalVelocity_Float => animHash_HorizontalVelocity_Float;
    public int AnimHash_Acceleration_Float => animHash_Acceleration_Float;
    public int AnimHash_FallingVelocity_Float => animHash_FallingVelocity_Float;
    public int AnimHash_Angle_Float => animHash_Angle_Float;
    public int AnimHash_Jump_Trigger => animHash_Jump_Trigger;
    public int AnimHash_Falling_Trigger => animHash_Falling_Trigger;
    public int AnimHash_Landing_Trigger => animHash_Landing_Trigger;

    // Audio
    [SerializeField] private AudioClip runningAudioClip;
    private AudioSource audioSource;
    [SerializeField] private float audioPitchMax = 1f, audioPitchMin = 0.55f;
    private float audioPitchStep;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        StateMachine = new MovementStateMachine(this, characterController, animator, cameraController);
        StateMachine.Initialize(StateMachine.stillState);

        InputManager.instance.OnMoveForward_performed += MoveForward_performed; 
        InputManager.instance.OnMoveForward_started += StateMachine.MoveForward_started;
        InputManager.instance.OnMoveForward_canceled += StateMachine.MoveForward_canceled;
        InputManager.instance.OnMoveAside_started += StateMachine.MoveAside_started;
        InputManager.instance.OnMoveAside_canceled += StateMachine.MoveAside_canceled;

        InputManager.instance.OnRotateMouse_performed += RotateMouse_performed;
        InputManager.instance.OnRotateKeyboard_performed += RotateKeyboard_performed;

        InputManager.instance.OnAcceleration_started += Acceleration_started;
        InputManager.instance.OnAcceleration_canceled += Acceleration_canceled;

        InputManager.instance.OnMouseRightClick_started += MouseRightClick_started;
        InputManager.instance.OnMouseRightClick_canceled += MouseRightClick_canceled;
        InputManager.instance.OnJumpUp_performed += JumpUp_performed;

        Preferences.OnPlRotStyleChanged += UpdatePlayerRotation;
        Preferences.OnCamRotStyleChanged += UpdatePlayerRotation;
        Preferences.OnPlRotSpeedChanged += UpdatePlayerRotation;

        animHash_VecticalVelocity_Float = Animator.StringToHash("Vertical Velocity");
        animHash_HorizontalVelocity_Float = Animator.StringToHash("Horizontal Velocity");
        animHash_Acceleration_Float = Animator.StringToHash("Acceleration");
        animHash_FallingVelocity_Float = Animator.StringToHash("Falling Velocity");
        animHash_Angle_Float = Animator.StringToHash("Angle");
        animHash_Jump_Trigger = Animator.StringToHash("Jump");
        animHash_Falling_Trigger = Animator.StringToHash("Falling");
        animHash_Landing_Trigger = Animator.StringToHash("Landing");
    }

    private void Acceleration_canceled(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        accelerationInput = context.ReadValueAsButton();
    }

    private void Acceleration_started(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        accelerationInput = context.ReadValueAsButton();
    }

    private void RotateKeyboard_performed(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        rotationKeyboardInput = context.ReadValue<float>();
    }

    private void RotateMouse_performed(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        rotationMouseInput = context.ReadValue<float>();
    }

    private void MoveForward_performed(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        movementForwardInput = context.ReadValue<float>();
    }

    private void MouseRightClick_canceled(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        currentRotationSpeed = Preferences.plRotSpeed;
    }

    private void MouseRightClick_started(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        if (Preferences.CamRotStyle == Preferences.CameraRotationStyle.withRightClickMouse
            && Preferences.PlRotStyle == Preferences.PlayerRotationStyle.withMouse)
        {
            currentRotationSpeed = 0;
        }
    }

    private void JumpUp_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        StateMachine.JumpUp_pressed();
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
        rotationAccelerationStep = (maxRotationAcceleration - 1) / maxAccelerationTime;
        audioPitchStep = (audioPitchMax - audioPitchMin) / maxAccelerationTime;
        audioSource.pitch = audioPitchMin;
    }

    void Update()
    {
        StateMachine.Update();
    }

    public (Vector3 point1, Vector3 point2) GetColliderPoints()
    {
        Vector3 p1 = transform.position + characterController.center
                    - new Vector3(0, characterController.height * 0.5f - 0.5f, 0);
        Vector3 p2 = p1 + new Vector3(0, characterController.height - 1, 0);
        return (p1, p2);
    }

    public void SetMoveDirection()
    {
        switch (Preferences.PlRotStyle)
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
        animator.SetFloat(AnimHash_VecticalVelocity_Float, moveDirectionLocal.z);
        animator.SetFloat(AnimHash_HorizontalVelocity_Float, moveDirectionLocal.x);
    }

    public void SetAcceleration()
    {
        // Dashing
        if (accelerationInput && StateMachine.CurrentState.Equals(StateMachine.movingState) 
            && acceleration < maxAcceleration)
        {
            acceleration += accelerationStep * Time.deltaTime;
            rotationAcceleration += rotationAccelerationStep * Time.deltaTime;
            audioSource.pitch += audioPitchStep * Time.deltaTime;
        }
        else if ((!accelerationInput || !StateMachine.CurrentState.Equals(StateMachine.movingState)) 
            && acceleration > 1f)
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

    public void Anim_JumpUpStart()
    {
        StateMachine.TransitionTo(StateMachine.jumpingUpState);
    }

    public void Anim_JumpUpFinish()
    {
        StateMachine.TransitionTo(StateMachine.fallingState);
    }

    public void Anim_UnlockMovement()
    {
        if (movementForwardInput == 0 && rotationKeyboardInput == 0)
        {
            StateMachine.TransitionTo(StateMachine.stillState);
        }
        else
        {
            StateMachine.TransitionTo(StateMachine.movingState);
        }
    }

    public void Anim_RunningAudioStart()
    {
        if (!audioSource.isPlaying)
        {
            audioSource.time = 0.198f;
            audioSource.Play();
        }
    }

    public void Anim_RunningAudioFinish()
    {
        audioSource.time = 0f;
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }

    public void RunningAudioStart()
    {
        if (!audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }

    public void RunningAudioFinish()
    {
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
        InputManager.instance.OnMoveForward_performed -= MoveForward_performed;
        InputManager.instance.OnMoveForward_started -= StateMachine.MoveForward_started;
        InputManager.instance.OnMoveForward_canceled -= StateMachine.MoveForward_canceled;
        InputManager.instance.OnMoveAside_started -= StateMachine.MoveAside_started;
        InputManager.instance.OnMoveAside_canceled -= StateMachine.MoveAside_canceled;

        InputManager.instance.OnRotateMouse_performed -= RotateMouse_performed;
        InputManager.instance.OnRotateKeyboard_performed -= RotateKeyboard_performed;

        InputManager.instance.OnAcceleration_started -= Acceleration_started;
        InputManager.instance.OnAcceleration_canceled -= Acceleration_canceled;

        InputManager.instance.OnMouseRightClick_started -= MouseRightClick_started;
        InputManager.instance.OnMouseRightClick_canceled -= MouseRightClick_canceled;
        InputManager.instance.OnJumpUp_performed -= JumpUp_performed;

        // TODO: to check to i need it
        Preferences.OnPlRotStyleChanged -= UpdatePlayerRotation;
        Preferences.OnCamRotStyleChanged -= UpdatePlayerRotation;
        Preferences.OnPlRotSpeedChanged -= UpdatePlayerRotation;
    }
}