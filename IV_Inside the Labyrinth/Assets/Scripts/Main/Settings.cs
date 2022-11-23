using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Settings : MonoBehaviour
{
    // Movement and Camera
    [Header("Player's Movement")]
    [SerializeField] private TMP_Dropdown plRotStyleDropdown;
    [SerializeField] private TMP_InputField plRotSpeedInputField;
    [SerializeField] private Slider plRotSpeedSlider;
    [Header("Camera's Rotation")]
    [SerializeField] private TMP_Dropdown camRotStyleDropdown;
    [SerializeField] private TMP_InputField camRotSpeed_HorizontalInputField, camRotSpeed_VerticalInputField;
    [SerializeField] private Slider camRotSpeed_HorizontalSlider, camRotSpeed_VerticalSlider;
    [Header("Camera's Damping")]
    [SerializeField] private TMP_InputField camDamping_BodyXInputField;
    [SerializeField] private TMP_InputField camDamping_BodyYInputField, 
        camDamping_BodyZInputField, camDamping_AimInputField;
    [SerializeField] private Slider camDamping_BodyXSlider, camDamping_BodyYSlider, 
        camDamping_BodyZSlider, camDamping_AimSlider;
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

        SetSliderRange(camDamping_BodyXSlider, Preferences.camDampingMin, Preferences.camDampingMax);
        camDamping_BodyXSlider.onValueChanged.AddListener(ChangeCamDamping_BodyX_Float);
        camDamping_BodyXInputField.onValueChanged.AddListener(ChangeCamDamping_BodyX_String);
        Preferences.OnCamDamping_BodyXChanged += UpdateCamDamping_BodyX;
        UpdateCamDamping_BodyX();

        SetSliderRange(camDamping_BodyYSlider, Preferences.camDampingMin, Preferences.camDampingMax);
        camDamping_BodyYSlider.onValueChanged.AddListener(ChangeCamDamping_BodyY_Float);
        camDamping_BodyYInputField.onValueChanged.AddListener(ChangeCamDamping_BodyY_String);
        Preferences.OnCamDamping_BodyYChanged += UpdateCamDamping_BodyY;
        UpdateCamDamping_BodyY();

        SetSliderRange(camDamping_BodyZSlider, Preferences.camDampingMin, Preferences.camDampingMax);
        camDamping_BodyZSlider.onValueChanged.AddListener(ChangeCamDamping_BodyZ_Float);
        camDamping_BodyZInputField.onValueChanged.AddListener(ChangeCamDamping_BodyZ_String);
        Preferences.OnCamDamping_BodyZChanged += UpdateCamDamping_BodyZ;
        UpdateCamDamping_BodyZ();

        SetSliderRange(camDamping_AimSlider, Preferences.camDampingMin, Preferences.camDampingMax);
        camDamping_AimSlider.onValueChanged.AddListener(ChangeCamDamping_Aim_Float);
        camDamping_AimInputField.onValueChanged.AddListener(ChangeCamDamping_Aim_String);
        Preferences.OnCamDamping_AimChanged += UpdateCamDamping_Aim;
        UpdateCamDamping_Aim();

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
        ChangeCamDamping_BodyX_Float(Preferences.camDamping_BodyX_def);
        ChangeCamDamping_BodyY_Float(Preferences.camDamping_BodyY_def);
        ChangeCamDamping_BodyZ_Float(Preferences.camDamping_BodyZ_def);
        ChangeCamDamping_Aim_Float(Preferences.camDamping_Aim_def);
    }

    private void ResetPrefs_Sounds()
    {
        ChangeBackgroundMusicVolume_Float(Preferences.backMusicVolume_def);
        ChangeStepsVolume_Float(Preferences.stepsVolume_def);
        ChangeDamageEffectVolume_Float(Preferences.damageEffectVolume_def);
    }

    private void PopulatePlayerRotationDropdown()
    {
        playerRotList.Clear();
        foreach (KeyValuePair<Preferences.PlayerRotationStyle, string> style in Preferences.PlRotStyles_info)
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
        switch (Preferences.PlRotStyle)
        {
            case Preferences.PlayerRotationStyle.withMouse:
                {
                    foreach (KeyValuePair<Preferences.CameraRotationStyle, (string name, bool isAvailableWithPlRot_Mouse, bool)> 
                        style in Preferences.CamRotStyles_info)
                    {
                        if (style.Value.isAvailableWithPlRot_Mouse)
                            cameraRotList.Add(new TMP_Dropdown.OptionData(style.Value.name));
                    }
                    break;
                }
            case Preferences.PlayerRotationStyle.withKeyboard:
                {
                    foreach (KeyValuePair<Preferences.CameraRotationStyle, (string name, bool, bool isAvailableWithPlRot_Keyboard)> 
                        style in Preferences.CamRotStyles_info)
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
        foreach (KeyValuePair<Preferences.PlayerRotationStyle, string> style in Preferences.PlRotStyles_info)
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
            style in Preferences.CamRotStyles_info)
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

    private void ChangeCamDamping_BodyX_Float(float value)
    {
        Preferences.SetCameraDamping_BodyX(value);
    }
    private void ChangeCamDamping_BodyX_String(string value)
    {
        ChangeCamDamping_BodyX_Float(float.Parse(value));
    }
    private void UpdateCamDamping_BodyX()
    {
        camDamping_BodyXInputField.text = Mathf.Round(Preferences.camDamping_BodyX).ToString();
        camDamping_BodyXSlider.value = Preferences.camDamping_BodyX;
    }

    private void ChangeCamDamping_BodyY_Float(float value)
    {
        Preferences.SetCameraDamping_BodyY(value); 
    }
    private void ChangeCamDamping_BodyY_String(string value)
    {
        ChangeCamDamping_BodyY_Float(float.Parse(value));
    }
    private void UpdateCamDamping_BodyY()
    {
        camDamping_BodyYInputField.text = Mathf.Round(Preferences.camDamping_BodyY).ToString();
        camDamping_BodyYSlider.value = Preferences.camDamping_BodyY;
    }

    private void ChangeCamDamping_BodyZ_Float(float value)
    {
        Preferences.SetCameraDamping_BodyZ(value);
    }
    private void ChangeCamDamping_BodyZ_String(string value)
    {
        ChangeCamDamping_BodyZ_Float(float.Parse(value));
    }
    private void UpdateCamDamping_BodyZ()
    {
        camDamping_BodyZInputField.text = Mathf.Round(Preferences.camDamping_BodyZ).ToString();
        camDamping_BodyZSlider.value = Preferences.camDamping_BodyZ;
    }

    private void ChangeCamDamping_Aim_Float(float value)
    {
        Preferences.SetCameraDamping_Aim(value);
    }
    private void ChangeCamDamping_Aim_String(string value)
    {
        ChangeCamDamping_Aim_Float(float.Parse(value));
    }
    private void UpdateCamDamping_Aim()
    {
        camDamping_AimInputField.text = Mathf.Round(Preferences.camDamping_Aim).ToString();
        camDamping_AimSlider.value = Preferences.camDamping_Aim;
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
        //Movement and Camera
        Preferences.OnPlRotStyleChanged -= UpdatePlayerRotationStyle;
        Preferences.OnCamRotStyleChanged -= UpdateCameraRotationStyle;

        Preferences.OnPlRotSpeedChanged -= UpdatePlayerRotSpeed;
        Preferences.OnCamRotSpeed_HorizontalChanged -= UpdateCamRotSpeed_Horizontal;
        Preferences.OnCamRotSpeed_VerticalChanged -= UpdateCamRotSpeed_Vertical;

        Preferences.OnCamDamping_BodyXChanged -= UpdateCamDamping_BodyX;
        Preferences.OnCamDamping_BodyYChanged -= UpdateCamDamping_BodyY;
        Preferences.OnCamDamping_BodyZChanged -= UpdateCamDamping_BodyZ;
        Preferences.OnCamDamping_AimChanged -= UpdateCamDamping_Aim;

        //Sounds
        Preferences.OnBackgroundMusicVolumeChanged -= UpdateBackgroundMusicVolume;
        Preferences.OnStepsVolumeChanged -= UpdateStepsVolume;
        Preferences.OnDamageEffectVolumeChanged -= UpdateDamageEffectVolume;
    }
}
