using System;
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

    private List<TMP_Dropdown.OptionData> playerRotList, cameraRotList;

    // Sounds
    [Header("Sounds")]
    [SerializeField] private TMP_InputField backgroundMusicInputField;
    [SerializeField] private TMP_InputField stepsInputField, damageEffectInputField;
    [SerializeField] private Slider backgroundMusicSlider, stepsSlider, damageEffectSlider;
    [Header("Reset")]
    [SerializeField] private Button resetButton_Sounds;

    private void Start()
    {
        pauseToggle.onValueChanged.AddListener(SetPauseBehavior);
        Preferences.OnPauseBehaviorChanged += UpdatePauseUI;
        UpdatePauseUI();

        // Movement and Camera
        playerRotList = new List<TMP_Dropdown.OptionData>();
        cameraRotList = new List<TMP_Dropdown.OptionData>();
        plRotStyleDropdown.onValueChanged.AddListener(ChangePlayerRotationStyle);
        camRotStyleDropdown.onValueChanged.AddListener(ChangeCameraRotationStyle);
        Preferences.OnPlRotStyleChanged += UpdatePlayerRotationStyle;
        Preferences.OnCamRotStyleChanged += UpdateCameraRotationStyle;
        UpdatePlayerRotationStyle();

        SetSliderRange(plRotSpeedSlider, Preferences.plRotSpeedMin, Preferences.plRotSpeedMax);
        plRotSpeedSlider.onValueChanged.AddListener(ChangePlayerRotSpeed_Float);
        plRotSpeedInputField.onValueChanged.AddListener(ChangePlayerRotSpeed_String);
        Preferences.OnPlRotSpeedChanged += UpdatePlayerRotSpeed;
        UpdatePlayerRotSpeed();

        SetSliderRange(camRotSpeed_HorizontalSlider, Preferences.camRotHorizontal_SpeedMin, Preferences.camRot_HorizontalSpeedMax);
        camRotSpeed_HorizontalSlider.onValueChanged.AddListener(ChangeCamRotSpeed_Horizontal_Float);
        camRotSpeed_HorizontalInputField.onValueChanged.AddListener(ChangeCamRotSpeed_Horizontal_String);
        Preferences.OnCamRotSpeed_HorizontalChanged += UpdateCamRotSpeed_Horizontal;
        UpdateCamRotSpeed_Horizontal();

        SetSliderRange(camRotSpeed_VerticalSlider, Preferences.camRotVertical_SpeedMin, Preferences.camRot_VerticalSpeedMax);
        camRotSpeed_VerticalSlider.onValueChanged.AddListener(ChangeCamRotSpeed_Vertical_Float);
        camRotSpeed_VerticalInputField.onValueChanged.AddListener(ChangeCamRotSpeed_Vertical_String);
        Preferences.OnCamRotSpeed_VerticalChanged += UpdateCamRotSpeed_Vertical;
        UpdateCamRotSpeed_Vertical();

        SetSliderRange(camFollowDamping_XSlider, Preferences.camDampingMin, Preferences.camDampingMax);
        camFollowDamping_XSlider.onValueChanged.AddListener(ChangeCamFollowDamping_X_Float);
        camFollowDamping_XInputField.onValueChanged.AddListener(ChangeCamFollowDamping_X_String);
        Preferences.OnCamFollowDamping_XChanged += UpdateCamFollowDamping_X;
        UpdateCamFollowDamping_X();

        SetSliderRange(camFollowDamping_YSlider, Preferences.camDampingMin, Preferences.camDampingMax);
        camFollowDamping_YSlider.onValueChanged.AddListener(ChangeCamFollowDamping_Y_Float);
        camFollowDamping_YInputField.onValueChanged.AddListener(ChangeCamFollowDamping_Y_String);
        Preferences.OnCamFollowDamping_YChanged += UpdateCamFollowDamping_Y;
        UpdateCamFollowDamping_Y();

        SetSliderRange(camFollowDamping_ZSlider, Preferences.camDampingMin, Preferences.camDampingMax);
        camFollowDamping_ZSlider.onValueChanged.AddListener(ChangeCamFollowDamping_Z_Float);
        camFollowDamping_ZInputField.onValueChanged.AddListener(ChangeCamFollowDamping_Z_String);
        Preferences.OnCamFollowDamping_ZChanged += UpdateCamFollowDamping_Z;
        UpdateCamFollowDamping_Z();

        SetSliderRange(camFollowDamping_YawSlider, Preferences.camDampingMin, Preferences.camDampingMax);
        camFollowDamping_YawSlider.onValueChanged.AddListener(ChangeCamFollowDamping_Yaw_Float);
        camFollowDamping_YawInputField.onValueChanged.AddListener(ChangeCamFollowDamping_Yaw_String);
        Preferences.OnCamFollowDamping_YawChanged += UpdateCamFollowDamping_Yaw;
        UpdateCamFollowDamping_Yaw();

        SetSliderRange(camRotDamping_HorizontalSlider, Preferences.camDampingMin, Preferences.camDampingMax);
        camRotDamping_HorizontalSlider.onValueChanged.AddListener(ChangeCamRotDamping_Horizontal_Float);
        camRotDamping_HorizontalInputField.onValueChanged.AddListener(ChangeCamRotDamping_Horizontal_String);
        Preferences.OnCamRotDamping_HorizontalChanged += UpdateCamRotDamping_Horizontal;
        UpdateCamRotDamping_Horizontal();

        SetSliderRange(camRotDamping_VerticalSlider, Preferences.camDampingMin, Preferences.camDampingMax);
        camRotDamping_VerticalSlider.onValueChanged.AddListener(ChangeCamRotDamping_Vertical_Float);
        camRotDamping_VerticalInputField.onValueChanged.AddListener(ChangeCamRotDamping_Vertical_String);
        Preferences.OnCamRotDamping_VerticalChanged += UpdateCamRotDamping_Vertical;
        UpdateCamRotDamping_Vertical();

        resetButton_MovementAndCamera.onClick.AddListener(ResetPrefs_MovementAndCamera);

        // Sounds
        SetSliderRange(backgroundMusicSlider, 0, 100);
        backgroundMusicSlider.onValueChanged.AddListener(ChangeBackgroundMusicVolume_Float);
        backgroundMusicInputField.onValueChanged.AddListener(ChangeBackgroundMusicVolume_String);
        Preferences.OnBackgroundMusicVolumeChanged += UpdateBackgroundMusicVolume;
        UpdateBackgroundMusicVolume();

        SetSliderRange(stepsSlider, 0, 100);
        stepsSlider.onValueChanged.AddListener(ChangeStepsVolume_Float);
        stepsInputField.onValueChanged.AddListener(ChangeStepsVolume_String);
        Preferences.OnStepsVolumeChanged += UpdateStepsVolume;
        UpdateStepsVolume();

        SetSliderRange(damageEffectSlider, 0, 100);
        damageEffectSlider.onValueChanged.AddListener(ChangeDamageEffectVolume_Float);
        damageEffectInputField.onValueChanged.AddListener(ChangeDamageEffectVolume_String);
        Preferences.OnDamageEffectVolumeChanged += UpdateDamageEffectVolume;
        UpdateDamageEffectVolume();

        resetButton_Sounds.onClick.AddListener(ResetPrefs_Sounds);
    }

    private void UpdatePauseUI()
    {
        pauseToggle.isOn = Preferences.isPausedWhileInMenu;
    }

    private void SetSliderRange(Slider slider, float minValue, float maxValue)
    {
        slider.maxValue = maxValue;
        slider.minValue = minValue;
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
        foreach (KeyValuePair<Preferences.PlayerRotationStyle, string> style in Preferences.plRotStyles_info)
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
                        style in Preferences.camRotStyles_info)
                    {
                        if (style.Value.isAvailableWithPlRot_Mouse)
                            cameraRotList.Add(new TMP_Dropdown.OptionData(style.Value.name));
                    }
                    break;
                }
            case Preferences.PlayerRotationStyle.withKeyboard:
                {
                    foreach (KeyValuePair<Preferences.CameraRotationStyle, (string name, bool, bool isAvailableWithPlRot_Keyboard)> 
                        style in Preferences.camRotStyles_info)
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
        foreach (KeyValuePair<Preferences.PlayerRotationStyle, string> style in Preferences.plRotStyles_info)
        {
            if (plRotStyleDropdown.options[dropdownValue].text.Equals(style.Value))
            {
                Preferences.SetPlayerRotationStyle((int)style.Key);
                break;
            }
        }
    }
    private void UpdatePlayerRotationStyle()
    {
        PopulatePlayerRotationDropdown();
        PopulateCameraRotationDropdown();
    }

    private void ChangeCameraRotationStyle(int dropdownValue)
    {
        foreach (KeyValuePair<Preferences.CameraRotationStyle, (string name, bool, bool)> 
            style in Preferences.camRotStyles_info)
        {
            if (camRotStyleDropdown.options[dropdownValue].text.Equals(style.Value.name))
            {
                Preferences.SetCameraRotationStyle((int)style.Key);
                break;
            }
        }
    }
    private void UpdateCameraRotationStyle()
    {
        PopulateCameraRotationDropdown();
    }

    private void ChangePlayerRotSpeed_Float(float value)
    {
        Preferences.SetPlayerRotationSpeed(value);
    }
    private void ChangePlayerRotSpeed_String(string value)
    {
        ChangePlayerRotSpeed_Float(float.Parse(value));
    }
    private void UpdatePlayerRotSpeed()
    {
        plRotSpeedInputField.text = Mathf.Round(Preferences.plRotSpeed).ToString();
        plRotSpeedSlider.value = Preferences.plRotSpeed;
    }

    private void ChangeCamRotSpeed_Horizontal_Float(float value)
    {
        Preferences.SetCameraRotationSpeed_Horizontal(value);
    }
    private void ChangeCamRotSpeed_Horizontal_String(string value)
    {
        ChangeCamRotSpeed_Horizontal_Float(float.Parse(value));
    }
    private void UpdateCamRotSpeed_Horizontal()
    {
        camRotSpeed_HorizontalInputField.text = Mathf.Round(Preferences.camRot_HorizontalSpeed).ToString();
        camRotSpeed_HorizontalSlider.value = Preferences.camRot_HorizontalSpeed;
    }

    private void ChangeCamRotSpeed_Vertical_Float(float value)
    {
        Preferences.SetCameraRotationSpeed_Vertical(value);
    }
    private void ChangeCamRotSpeed_Vertical_String(string value)
    {
        ChangeCamRotSpeed_Vertical_Float(float.Parse(value));
    }
    private void UpdateCamRotSpeed_Vertical()
    {
        camRotSpeed_VerticalInputField.text = Mathf.Round(Preferences.camRot_VerticalSpeed).ToString();
        camRotSpeed_VerticalSlider.value = Preferences.camRot_VerticalSpeed;
    }

    private void ChangeCamFollowDamping_X_Float(float value)
    {
        Preferences.SetCameraFollowingDamping_X(value);
    }
    private void ChangeCamFollowDamping_X_String(string value)
    {
        ChangeCamFollowDamping_X_Float(float.Parse(value));
    }
    private void UpdateCamFollowDamping_X()
    {
        camFollowDamping_XInputField.text = Mathf.Round(Preferences.camFollowDamping_X).ToString();
        camFollowDamping_XSlider.value = Preferences.camFollowDamping_X;
    }

    private void ChangeCamFollowDamping_Y_Float(float value)
    {
        Preferences.SetCameraFollowingDamping_Y(value); 
    }
    private void ChangeCamFollowDamping_Y_String(string value)
    {
        ChangeCamFollowDamping_Y_Float(float.Parse(value));
    }
    private void UpdateCamFollowDamping_Y()
    {
        camFollowDamping_YInputField.text = Mathf.Round(Preferences.camFollowDamping_Y).ToString();
        camFollowDamping_YSlider.value = Preferences.camFollowDamping_Y;
    }

    private void ChangeCamFollowDamping_Z_Float(float value)
    {
        Preferences.SetCameraFollowingDamping_Z(value);
    }
    private void ChangeCamFollowDamping_Z_String(string value)
    {
        ChangeCamFollowDamping_Z_Float(float.Parse(value));
    }
    private void UpdateCamFollowDamping_Z()
    {
        camFollowDamping_ZInputField.text = Mathf.Round(Preferences.camFollowDamping_Z).ToString();
        camFollowDamping_ZSlider.value = Preferences.camFollowDamping_Z;
    }

    private void ChangeCamFollowDamping_Yaw_Float(float value)
    {
        Preferences.SetCameraFollowingDamping_Yaw(value);
    }
    private void ChangeCamFollowDamping_Yaw_String(string value)
    {
        ChangeCamFollowDamping_Yaw_Float(float.Parse(value));
    }
    private void UpdateCamFollowDamping_Yaw()
    {
        camFollowDamping_YawInputField.text = Mathf.Round(Preferences.camFollowDamping_Yaw).ToString();
        camFollowDamping_YawSlider.value = Preferences.camFollowDamping_Yaw;
    }

    private void ChangeCamRotDamping_Horizontal_Float(float value)
    {
        Preferences.SetCameraRotatingDamping_Horizontal(value);
    }
    private void ChangeCamRotDamping_Horizontal_String(string value)
    {
        ChangeCamRotDamping_Horizontal_Float(float.Parse(value));
    }
    private void UpdateCamRotDamping_Horizontal()
    {
        camRotDamping_HorizontalInputField.text = Mathf.Round(Preferences.camRotDamping_Horizontal).ToString();
        camRotDamping_HorizontalSlider.value = Preferences.camRotDamping_Horizontal;
    }

    private void ChangeCamRotDamping_Vertical_Float(float value)
    {
        Preferences.SetCameraRotatingDamping_Vertical(value);
    }
    private void ChangeCamRotDamping_Vertical_String(string value)
    {
        ChangeCamRotDamping_Vertical_Float(float.Parse(value));
    }
    private void UpdateCamRotDamping_Vertical()
    {
        camRotDamping_VerticalInputField.text = Mathf.Round(Preferences.camRotDamping_Vertical).ToString();
        camRotDamping_VerticalSlider.value = Preferences.camRotDamping_Vertical;
    }

    private void ChangeBackgroundMusicVolume_Float(float value)
    {
        Preferences.SetBackgroundMusicVolume(value);
    }
    private void ChangeBackgroundMusicVolume_String(string value)
    {
        ChangeBackgroundMusicVolume_Float(float.Parse(value));
    }
    private void UpdateBackgroundMusicVolume()
    {
        backgroundMusicInputField.text = Mathf.Round(Preferences.backMusicVolume).ToString();
        backgroundMusicSlider.value = Preferences.backMusicVolume;
    }

    private void ChangeStepsVolume_Float(float value)
    {
        Preferences.SetStepsVolume(value);
    }
    private void ChangeStepsVolume_String(string value)
    {
        ChangeStepsVolume_Float(float.Parse(value));
    }
    private void UpdateStepsVolume()
    {
        stepsInputField.text = Mathf.Round(Preferences.stepsVolume).ToString();
        stepsSlider.value = Preferences.stepsVolume;
    }

    private void ChangeDamageEffectVolume_Float(float value)
    {
        Preferences.SetDamageEffectVolume(value);
    }
    private void ChangeDamageEffectVolume_String(string value)
    {
        ChangeDamageEffectVolume_Float(float.Parse(value));
    }
    private void UpdateDamageEffectVolume()
    {
        damageEffectInputField.text = Mathf.Round(Preferences.damageEffectVolume).ToString();
        damageEffectSlider.value = Preferences.damageEffectVolume;
    }

    private void OnDestroy()
    {
        Preferences.OnPauseBehaviorChanged -= UpdatePauseUI;

        //Movement and Camera
        Preferences.OnPlRotStyleChanged -= UpdatePlayerRotationStyle;
        Preferences.OnCamRotStyleChanged -= UpdateCameraRotationStyle;

        Preferences.OnPlRotSpeedChanged -= UpdatePlayerRotSpeed;
        Preferences.OnCamRotSpeed_HorizontalChanged -= UpdateCamRotSpeed_Horizontal;
        Preferences.OnCamRotSpeed_VerticalChanged -= UpdateCamRotSpeed_Vertical;

        Preferences.OnCamFollowDamping_XChanged -= UpdateCamFollowDamping_X;
        Preferences.OnCamFollowDamping_YChanged -= UpdateCamFollowDamping_Y;
        Preferences.OnCamFollowDamping_ZChanged -= UpdateCamFollowDamping_Z;
        Preferences.OnCamFollowDamping_YawChanged -= UpdateCamFollowDamping_Yaw;

        Preferences.OnCamRotDamping_HorizontalChanged -= UpdateCamRotDamping_Horizontal;
        Preferences.OnCamRotDamping_VerticalChanged -= UpdateCamRotDamping_Vertical;

        //Sounds
        Preferences.OnBackgroundMusicVolumeChanged -= UpdateBackgroundMusicVolume;
        Preferences.OnStepsVolumeChanged -= UpdateStepsVolume;
        Preferences.OnDamageEffectVolumeChanged -= UpdateDamageEffectVolume;
    }
}
