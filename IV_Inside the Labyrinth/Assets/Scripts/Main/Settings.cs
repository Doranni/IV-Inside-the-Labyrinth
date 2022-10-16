using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Settings : MonoBehaviour
{
    [SerializeField] private Toggle pauseToggle;

    // Movement and Camera
    [Header("Player's Movement")]
    [SerializeField] private TMP_Dropdown plRotStyleDropdown;
    [SerializeField] private TMP_InputField plRotSpeedInputField;
    [SerializeField] private Slider plRotSpeedSlider;
    [Header("Camera's Rotation")]
    [SerializeField] private TMP_Dropdown camRotStyleDropdown;
    [SerializeField] private TMP_InputField camRotSpeed_HorizontalInputField, camRotSpeed_VerticalInputField;
    [SerializeField] private Slider camRotSpeed_HorizontalSlider, camRotSpeed_VerticalSlider;
    [Header("Camera's Follow damping")]
    [SerializeField] private TMP_InputField camFollowDamping_XInputField, camFollowDamping_YInputField, 
        camFollowDamping_ZInputField, camFollowDamping_YawInputField;
    [SerializeField] private Slider camFollowDamping_XSlider, camFollowDamping_YSlider, 
        camFollowDamping_ZSlider, camFollowDamping_YawSlider;
    [Header("Camera's Rotation damping")]
    [SerializeField] private TMP_InputField camRotDamping_HorizontalInputField, camRotDamping_VerticalInputField;
    [SerializeField] private Slider camRotDamping_HorizontalSlider, camRotDamping_VerticalSlider;
    [Header("Reset")]
    [SerializeField] private Button resetButton_MovementAndCamera;

    private AnimationAndMovementController movementController;
    private CameraController cameraController;

    List<TMP_Dropdown.OptionData> playerRotList, cameraRotList;

    // Sounds
    [Header("Sounds")]
    [SerializeField] private TMP_InputField backgroundMusicInputField;
    [SerializeField] private TMP_InputField stepsInputField, damageEffectInputField;
    [SerializeField] private Slider backgroundMusicSlider, stepsSlider, damageEffectSlider;
    [Header("Reset")]
    [SerializeField] private Button resetButton_Sounds;

    private void Start()
    {
        // Movement and Camera
        movementController = GameObject.FindGameObjectWithTag(GameManager.playerTag).GetComponent<AnimationAndMovementController>();
        cameraController = GameObject.Find("MainCamera").GetComponent<CameraController>();

        pauseToggle.isOn = Preferences.isPausedWhileInMenu;
        pauseToggle.onValueChanged.AddListener(SetPauseBehavior);

        playerRotList = new List<TMP_Dropdown.OptionData>();
        cameraRotList = new List<TMP_Dropdown.OptionData>();
        PopulatePlayerRotationDropdown();
        PopulateCameraRotationDropdown();
        plRotStyleDropdown.onValueChanged.AddListener(ChangePlayerRotationStyle);
        camRotStyleDropdown.onValueChanged.AddListener(ChangeCameraRotationStyle);

        SetSliderAndInputFieldValue(plRotSpeedInputField, plRotSpeedSlider,
            Preferences.plRotSpeedMin, Preferences.plRotSpeedMax, Preferences.plRotSpeed);
        plRotSpeedSlider.onValueChanged.AddListener(ChangePlayerRotSpeed_Float);
        plRotSpeedInputField.onValueChanged.AddListener(ChangePlayerRotSpeed_String);

        SetSliderAndInputFieldValue(camRotSpeed_HorizontalInputField, camRotSpeed_HorizontalSlider,
            Preferences.camRotHorizontal_SpeedMin, Preferences.camRot_HorizontalSpeedMax, Preferences.camRot_HorizontalSpeed);
        camRotSpeed_HorizontalSlider.onValueChanged.AddListener(ChangeCamRotSpeed_Horizontal_Float);
        camRotSpeed_HorizontalInputField.onValueChanged.AddListener(ChangeCamRotSpeed_Horizontal_String);

        SetSliderAndInputFieldValue(camRotSpeed_VerticalInputField, camRotSpeed_VerticalSlider,
            Preferences.camRotVertical_SpeedMin, Preferences.camRot_VerticalSpeedMax, Preferences.camRot_VerticalSpeed);
        camRotSpeed_VerticalSlider.onValueChanged.AddListener(ChangeCamRotSpeed_Vertical_Float);
        camRotSpeed_VerticalInputField.onValueChanged.AddListener(ChangeCamRotSpeed_Vertical_String);

        SetSliderAndInputFieldValue(camFollowDamping_XInputField, camFollowDamping_XSlider, 
            Preferences.camDampingMin, Preferences.camDampingMax, Preferences.camFollowDamping_X);
        camFollowDamping_XSlider.onValueChanged.AddListener(ChangeCamFollowDamping_X_Float);
        camFollowDamping_XInputField.onValueChanged.AddListener(ChangeCamFollowDamping_X_String);

        SetSliderAndInputFieldValue(camFollowDamping_YInputField, camFollowDamping_YSlider,
            Preferences.camDampingMin, Preferences.camDampingMax, Preferences.camFollowDamping_Y);
        camFollowDamping_YSlider.onValueChanged.AddListener(ChangeCamFollowDamping_Y_Float);
        camFollowDamping_YInputField.onValueChanged.AddListener(ChangeCamFollowDamping_Y_String);

        SetSliderAndInputFieldValue(camFollowDamping_ZInputField, camFollowDamping_ZSlider,
            Preferences.camDampingMin, Preferences.camDampingMax, Preferences.camFollowDamping_Z);
        camFollowDamping_ZSlider.onValueChanged.AddListener(ChangeCamFollowDamping_Z_Float);
        camFollowDamping_ZInputField.onValueChanged.AddListener(ChangeCamFollowDamping_Z_String);

        SetSliderAndInputFieldValue(camFollowDamping_YawInputField, camFollowDamping_YawSlider,
            Preferences.camDampingMin, Preferences.camDampingMax, Preferences.camFollowDamping_Yaw);
        camFollowDamping_YawSlider.onValueChanged.AddListener(ChangeCamFollowDamping_Yaw_Float);
        camFollowDamping_YawInputField.onValueChanged.AddListener(ChangeCamFollowDamping_Yaw_String);

        SetSliderAndInputFieldValue(camRotDamping_HorizontalInputField, camRotDamping_HorizontalSlider,
            Preferences.camDampingMin, Preferences.camDampingMax, Preferences.camRotDamping_Horizontal);
        camRotDamping_HorizontalSlider.onValueChanged.AddListener(ChangeCamRotDamping_Horizontal_Float);
        camRotDamping_HorizontalInputField.onValueChanged.AddListener(ChangeCamRotDamping_Horizontal_String);

        SetSliderAndInputFieldValue(camRotDamping_VerticalInputField, camRotDamping_VerticalSlider,
            Preferences.camDampingMin, Preferences.camDampingMax, Preferences.camRotDamping_Vertical);
        camRotDamping_VerticalSlider.onValueChanged.AddListener(ChangeCamRotDamping_Vertical_Float);
        camRotDamping_VerticalInputField.onValueChanged.AddListener(ChangeCamRotDamping_Vertical_String);

        resetButton_MovementAndCamera.onClick.AddListener(ResetPrefs_MovementAndCamera);

        // Sounds
        SetSliderAndInputFieldValue(backgroundMusicInputField, backgroundMusicSlider, 
            0, 100, Preferences.backMusicVolume);
        backgroundMusicSlider.onValueChanged.AddListener(ChangeBackgroundMusicVolume_Float);
        backgroundMusicInputField.onValueChanged.AddListener(ChangeBackgroundMusicVolume_String);

        SetSliderAndInputFieldValue(stepsInputField, stepsSlider, 
            0, 100, Preferences.stepsVolume);
        stepsSlider.onValueChanged.AddListener(ChangeStepsVolume_Float);
        stepsInputField.onValueChanged.AddListener(ChangeStepsVolume_String);

        SetSliderAndInputFieldValue(damageEffectInputField, damageEffectSlider, 
            0, 100, Preferences.damageEffectVolume);
        damageEffectSlider.onValueChanged.AddListener(ChangeDamageEffectVolume_Float);
        damageEffectInputField.onValueChanged.AddListener(ChangeDamageEffectVolume_String);

        resetButton_Sounds.onClick.AddListener(ResetPrefs_Sounds);
    }

    private void SetSliderAndInputFieldValue(TMP_InputField inputField, Slider slider, float minValue, float maxValue, float value)
    {
        slider.maxValue = maxValue;
        slider.minValue = minValue;
        slider.value = value;
        inputField.text = value.ToString();
    }

    private void ResetPrefs_MovementAndCamera()
    {
        ChangePlayerRotSpeed_Float(Preferences.plRotSpeed_def);
        ChangeCamRotSpeed_Horizontal_Float(Preferences.camRotSpeed_Horizontal_def);
        ChangeCamRotSpeed_Vertical_Float(Preferences.camRotSpeed_Vertical_def);
        ChangeCamFollowDamping_X_Float(Preferences.camFollowDamping_X_def);
        ChangeCamFollowDamping_Y_Float(Preferences.camFollowDamping_Y_def);
        ChangeCamFollowDamping_Z_Float(Preferences.camFollowDamping_Z_def);
        ChangeCamFollowDamping_Yaw_Float(Preferences.camFollowDamping_Yaw_def);
        ChangeCamRotDamping_Horizontal_Float(Preferences.camRotDamping_Horizontal_def);
        ChangeCamRotDamping_Vertical_Float(Preferences.camRotDamping_Vertical_def);
    }

    private void ResetPrefs_Sounds()
    {
        ChangeBackgroundMusicVolume_Float(Preferences.backMusicVolume_def);
        ChangeStepsVolume_Float(Preferences.stepsVolume_def);
        ChangeDamageEffectVolume_Float(Preferences.damageEffectVolume_def);
    }

    private void SetPauseBehavior(bool value)
    {
        Preferences.isPausedWhileInMenu = value;
        if (value)
        {
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
        }
    }

    private void PopulatePlayerRotationDropdown()
    {
        playerRotList.Clear();
        foreach (KeyValuePair<Preferences.PlayerRotationStyle, string> style in Preferences.plRotStyles)
        {
            playerRotList.Add(new TMP_Dropdown.OptionData(style.Value));
        }
        plRotStyleDropdown.options = playerRotList;
        for (int i = 0; i < playerRotList.Count; i++)
        {
            if (playerRotList[i].text.Equals(Preferences.GetPlayerRotationStyleName()))
            {
                plRotStyleDropdown.value = i;
            }
        }
    }

    private void PopulateCameraRotationDropdown()
    {
        cameraRotList.Clear();
        switch (Preferences.plRotStyle)
        {
            case Preferences.PlayerRotationStyle.withMouse:
                {
                    foreach (KeyValuePair<Preferences.CameraRotationStyle, (string name, bool isAvailableWithPlRot_Mouse, bool)> 
                        style in Preferences.camRotStyles)
                    {
                        if (style.Value.isAvailableWithPlRot_Mouse)
                            cameraRotList.Add(new TMP_Dropdown.OptionData(style.Value.name));
                    }
                    break;
                }
            case Preferences.PlayerRotationStyle.withKeyboard:
                {
                    foreach (KeyValuePair<Preferences.CameraRotationStyle, (string name, bool, bool isAvailableWithPlRot_Keyboard)> 
                        style in Preferences.camRotStyles)
                    {
                        if (style.Value.isAvailableWithPlRot_Keyboard)
                            cameraRotList.Add(new TMP_Dropdown.OptionData(style.Value.name));
                    }
                    break;
                }
        }
        camRotStyleDropdown.options = cameraRotList;
        for (int i = 0; i < cameraRotList.Count; i++)
        {
            if (cameraRotList[i].text.Equals(Preferences.GetCameraRotationStyleName()))
            {
                camRotStyleDropdown.value = i;
            }
        }
    }

    private void ChangePlayerRotationStyle(int dropdownValue)
    {
        foreach (KeyValuePair<Preferences.PlayerRotationStyle, string> style in Preferences.plRotStyles)
        {
            if (plRotStyleDropdown.options[dropdownValue].text.Equals(style.Value))
            {
                Preferences.SetPlayerRotationStyle((int)style.Key);
                break;
            }
        }
        movementController.UpdatePlayerRotation();
        cameraController.UpdateViewMode();
        PopulateCameraRotationDropdown();
    }

    private void ChangeCameraRotationStyle(int dropdownValue)
    {
        foreach (KeyValuePair<Preferences.CameraRotationStyle, (string name, bool, bool)> 
            style in Preferences.camRotStyles)
        {
            if (camRotStyleDropdown.options[dropdownValue].text.Equals(style.Value.name))
            {
                Preferences.SetCameraRotationStyle((int)style.Key);
                break;
            }
        }
        movementController.UpdatePlayerRotation();
        cameraController.UpdateViewMode();
    }

    private void ChangePlayerRotSpeed_Float(float value)
    {
        Preferences.SetPlayerRotationSpeed(value);
        plRotSpeedInputField.text = Preferences.plRotSpeed.ToString();
        plRotSpeedSlider.value = Preferences.plRotSpeed;
        movementController.UpdatePlayerRotation();
    }

    private void ChangePlayerRotSpeed_String(string value)
    {
        ChangePlayerRotSpeed_Float(float.Parse(value));
    }

    private void ChangeCamRotSpeed_Horizontal_Float(float value)
    {
        Preferences.SetCameraRotationSpeed_Horizontal(value);
        camRotSpeed_HorizontalInputField.text = Preferences.camRot_HorizontalSpeed.ToString();
        camRotSpeed_HorizontalSlider.value = Preferences.camRot_HorizontalSpeed;
        cameraController.UpdateRotation();
    }

    private void ChangeCamRotSpeed_Horizontal_String(string value)
    {
        ChangeCamRotSpeed_Horizontal_Float(float.Parse(value));
    }

    private void ChangeCamRotSpeed_Vertical_Float(float value)
    {
        Preferences.SetCameraRotationSpeed_Vertical(value);
        camRotSpeed_VerticalInputField.text = Preferences.camRot_VerticalSpeed.ToString();
        camRotSpeed_VerticalSlider.value = Preferences.camRot_VerticalSpeed;
        cameraController.UpdateRotation();
    }

    private void ChangeCamRotSpeed_Vertical_String(string value)
    {
        ChangeCamRotSpeed_Vertical_Float(float.Parse(value));
    }

    private void ChangeCamFollowDamping_X_Float(float value)
    {
        Preferences.SetCameraFollowingDamping_X(value);
        camFollowDamping_XInputField.text = Preferences.camFollowDamping_X.ToString();
        camFollowDamping_XSlider.value = Preferences.camFollowDamping_X;
        cameraController.UpdateDamping();
    }

    private void ChangeCamFollowDamping_X_String(string value)
    {
        ChangeCamFollowDamping_X_Float(float.Parse(value));
    }

    private void ChangeCamFollowDamping_Y_Float(float value)
    {
        Preferences.SetCameraFollowingDamping_Y(value);
        camFollowDamping_YInputField.text = Preferences.camFollowDamping_Y.ToString();
        camFollowDamping_YSlider.value = Preferences.camFollowDamping_Y;
        cameraController.UpdateDamping();
    }

    private void ChangeCamFollowDamping_Y_String(string value)
    {
        ChangeCamFollowDamping_Y_Float(float.Parse(value));
    }

    private void ChangeCamFollowDamping_Z_Float(float value)
    {
        Preferences.SetCameraFollowingDamping_Z(value);
        camFollowDamping_ZInputField.text = Preferences.camFollowDamping_Z.ToString();
        camFollowDamping_ZSlider.value = Preferences.camFollowDamping_Z;
        cameraController.UpdateDamping();
    }

    private void ChangeCamFollowDamping_Z_String(string value)
    {
        ChangeCamFollowDamping_Z_Float(float.Parse(value));
    }

    private void ChangeCamFollowDamping_Yaw_Float(float value)
    {
        Preferences.SetCameraFollowingDamping_Yaw(value);
        camFollowDamping_YawInputField.text = Preferences.camFollowDamping_Yaw.ToString();
        camFollowDamping_YawSlider.value = Preferences.camFollowDamping_Yaw;
        cameraController.UpdateDamping();
    }

    private void ChangeCamFollowDamping_Yaw_String(string value)
    {
        ChangeCamFollowDamping_Yaw_Float(float.Parse(value));
    }

    private void ChangeCamRotDamping_Horizontal_Float(float value)
    {
        Preferences.SetCameraRotatingDamping_Horizontal(value);
        camRotDamping_HorizontalInputField.text = Preferences.camRotDamping_Horizontal.ToString();
        camRotDamping_HorizontalSlider.value = Preferences.camRotDamping_Horizontal;
        cameraController.UpdateDamping();
    }

    private void ChangeCamRotDamping_Horizontal_String(string value)
    {
        ChangeCamRotDamping_Horizontal_Float(float.Parse(value));
    }

    private void ChangeCamRotDamping_Vertical_Float(float value)
    {
        Preferences.SetCameraRotatingDamping_Vertical(value);
        camRotDamping_VerticalInputField.text = Preferences.camRotDamping_Vertical.ToString();
        camRotDamping_VerticalSlider.value = Preferences.camRotDamping_Vertical;
        cameraController.UpdateDamping();
    }

    private void ChangeCamRotDamping_Vertical_String(string value)
    {
        ChangeCamRotDamping_Vertical_Float(float.Parse(value));
    }

    private void ChangeBackgroundMusicVolume_Float(float value)
    {
        Preferences.SetBackgroundMusicVolume(value);
        backgroundMusicInputField.text = Preferences.backMusicVolume.ToString();
        backgroundMusicSlider.value = Preferences.backMusicVolume;
        SoundsManager.UpdateBackgroundMusicVolume(value);
    }

    private void ChangeBackgroundMusicVolume_String(string value)
    {
        ChangeBackgroundMusicVolume_Float(float.Parse(value));
    }

    private void ChangeStepsVolume_Float(float value)
    {
        Preferences.SetStepsVolume(value);
        stepsInputField.text = Preferences.stepsVolume.ToString();
        stepsSlider.value = Preferences.stepsVolume;
        SoundsManager.UpdateStepsVolume(value);
    }

    private void ChangeStepsVolume_String(string value)
    {
        ChangeStepsVolume_Float(float.Parse(value));
    }

    private void ChangeDamageEffectVolume_Float(float value)
    {
        Preferences.SetDamageEffectVolume(value);
        damageEffectInputField.text = Preferences.damageEffectVolume.ToString();
        damageEffectSlider.value = Preferences.damageEffectVolume;
        SoundsManager.UpdateDamageEffectVolume(value);
    }

    private void ChangeDamageEffectVolume_String(string value)
    {
        ChangeDamageEffectVolume_Float(float.Parse(value));
    }
}
