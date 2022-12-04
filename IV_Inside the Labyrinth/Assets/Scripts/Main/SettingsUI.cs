using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class SettingsUI : MonoBehaviour
{
    private VisualElement settingsScreen;
    const string settingsScreen_Name = "SettingsScreen";

    // Movement and Camera
    private DropdownField plRotStyleDropdown, camRotStyleDropdown;
    private Slider plRotSpeedSlider, camRotSpeed_HorizontalSlider, camRotSpeed_VerticalSlider;
    private Slider camDamping_BodyXSlider, camDamping_BodyYSlider, 
        camDamping_BodyZSlider, camDamping_AimSlider;
    private Button movementAndCameraResetButton;

    const string plRotStyleDropdown_Name = "DropdownField_PlRotStyle";
    const string camRotStyleDropdown_Name = "DropdownField_CamRotStyle";

    const string plRotSpeedSlider_Name = "Slider_PlRotSpeed";
    const string camRotSpeed_HorizontalSlider_Name = "Slider_CamRotSpeed_Horizontal";
    const string camRotSpeed_VerticalSlider_Name = "Slider_CamRotSpeed_Vertical";

    const string camDamping_BodyXSlider_Name = "Slider_CamDamping_BodyX";
    const string camDamping_BodyYSlider_Name = "Slider_CamDamping_BodyY";
    const string camDamping_BodyZSlider_Name = "Slider_CamDamping_BodyZ";
    const string camDamping_AimSlider_Name = "Slider_CamDamping_Aim";

    const string movementAndCameraResetButton_Name = "Button_Reset_MovementAndCamera";

    private List<string> plRotStylesChoices = new();
    private List<string> camRotStylesChoices = new();

    // Sounds
    private Slider backgroundMusicSlider, stepsSlider, damageEffectSlider;
    private Button soundsResetButton;

    const string backgroundMusicSlider_Name = "Slider_BackgroundMusicVolume";
    const string stepsSlider_Name = "Slider_StepsVolume";
    const string damageEffectSlider_Name = "Slider_DamageEffectVolume";

    const string soundsResetButton_Name = "Button_Reset_Sounds";

    UIDocument gameScreen;

    private void Awake()
    {
        gameScreen = GetComponent<UIDocument>();
        VisualElement rootElement = gameScreen.rootVisualElement;
        settingsScreen = rootElement.Q(settingsScreen_Name);

        // Movement and Camera
        plRotStyleDropdown = rootElement.Q<DropdownField>(plRotStyleDropdown_Name);
        camRotStyleDropdown = rootElement.Q<DropdownField>(camRotStyleDropdown_Name);

        plRotSpeedSlider = rootElement.Q<Slider>(plRotSpeedSlider_Name);
        camRotSpeed_HorizontalSlider = rootElement.Q<Slider>(camRotSpeed_HorizontalSlider_Name);
        camRotSpeed_VerticalSlider = rootElement.Q<Slider>(camRotSpeed_VerticalSlider_Name);

        camDamping_BodyXSlider = rootElement.Q<Slider>(camDamping_BodyXSlider_Name);
        camDamping_BodyYSlider = rootElement.Q<Slider>(camDamping_BodyYSlider_Name);
        camDamping_BodyZSlider = rootElement.Q<Slider>(camDamping_BodyZSlider_Name);
        camDamping_AimSlider = rootElement.Q<Slider>(camDamping_AimSlider_Name);

        movementAndCameraResetButton = rootElement.Q<Button>(movementAndCameraResetButton_Name);

        // Sounds
        backgroundMusicSlider = rootElement.Q<Slider>(backgroundMusicSlider_Name);
        stepsSlider = rootElement.Q<Slider>(stepsSlider_Name);
        damageEffectSlider = rootElement.Q<Slider>(damageEffectSlider_Name);

        soundsResetButton = rootElement.Q<Button>(soundsResetButton_Name);
    }

    private void Start()
    {
        GameManager.instance.StateMachine.OnStateChanged += UpdateVisibility;
        UpdateVisibility();

        // Movement and Camera
        PopulatePlayerRotationDropdown();
        PopulateCameraRotationDropdown();
        plRotStyleDropdown.RegisterValueChangedCallback(ChangePlayerRotationStyle);
        camRotStyleDropdown.RegisterValueChangedCallback(ChangeCameraRotationStyle);
        Preferences.OnPlRotStyleChanged += UpdatePlayerRotationStyle;
        Preferences.OnCamRotStyleChanged += UpdateCameraRotationStyle;

        SetSliderRange(plRotSpeedSlider, Preferences.plRotSpeedMin, Preferences.plRotSpeedMax);
        plRotSpeedSlider.RegisterValueChangedCallback(ChangePlayerRotSpeed);
        Preferences.OnPlRotSpeedChanged += UpdatePlayerRotSpeed;
        UpdatePlayerRotSpeed();

        SetSliderRange(camRotSpeed_HorizontalSlider, Preferences.camRotHorizontal_SpeedMin, Preferences.camRot_HorizontalSpeedMax);
        camRotSpeed_HorizontalSlider.RegisterValueChangedCallback(ChangeCamRotSpeed_Horizontal);
        Preferences.OnCamRotSpeed_HorizontalChanged += UpdateCamRotSpeed_Horizontal;
        UpdateCamRotSpeed_Horizontal();

        SetSliderRange(camRotSpeed_VerticalSlider, Preferences.camRotVertical_SpeedMin, Preferences.camRot_VerticalSpeedMax);
        camRotSpeed_VerticalSlider.RegisterValueChangedCallback(ChangeCamRotSpeed_Vertical);
        Preferences.OnCamRotSpeed_VerticalChanged += UpdateCamRotSpeed_Vertical;
        UpdateCamRotSpeed_Vertical();

        SetSliderRange(camDamping_BodyXSlider, Preferences.camDampingMin, Preferences.camDampingMax);
        camDamping_BodyXSlider.RegisterValueChangedCallback(ChangeCamDamping_BodyX);
        Preferences.OnCamDamping_BodyXChanged += UpdateCamDamping_BodyX;
        UpdateCamDamping_BodyX();

        SetSliderRange(camDamping_BodyYSlider, Preferences.camDampingMin, Preferences.camDampingMax);
        camDamping_BodyYSlider.RegisterValueChangedCallback(ChangeCamDamping_BodyY);
        Preferences.OnCamDamping_BodyYChanged += UpdateCamDamping_BodyY;
        UpdateCamDamping_BodyY();

        SetSliderRange(camDamping_BodyZSlider, Preferences.camDampingMin, Preferences.camDampingMax);
        camDamping_BodyZSlider.RegisterValueChangedCallback(ChangeCamDamping_BodyZ);
        Preferences.OnCamDamping_BodyZChanged += UpdateCamDamping_BodyZ;
        UpdateCamDamping_BodyZ();

        SetSliderRange(camDamping_AimSlider, Preferences.camDampingMin, Preferences.camDampingMax);
        camDamping_AimSlider.RegisterValueChangedCallback(ChangeCamDamping_Aim);
        Preferences.OnCamDamping_AimChanged += UpdateCamDamping_Aim;
        UpdateCamDamping_Aim();

        movementAndCameraResetButton.RegisterCallback<ClickEvent>((_) => ResetPrefs_MovementAndCamera());

        // Sounds
        SetSliderRange(backgroundMusicSlider, 0, 100);
        backgroundMusicSlider.RegisterValueChangedCallback(ChangeBackgroundMusicVolume);
        Preferences.OnBackgroundMusicVolumeChanged += UpdateBackgroundMusicVolume;
        UpdateBackgroundMusicVolume();

        SetSliderRange(stepsSlider, 0, 100);
        stepsSlider.RegisterValueChangedCallback(ChangeStepsVolume);
        Preferences.OnStepsVolumeChanged += UpdateStepsVolume;
        UpdateStepsVolume();

        SetSliderRange(damageEffectSlider, 0, 100);
        damageEffectSlider.RegisterValueChangedCallback(ChangeDamageEffectVolume);
        Preferences.OnDamageEffectVolumeChanged += UpdateDamageEffectVolume;
        UpdateDamageEffectVolume();

        soundsResetButton.RegisterCallback<ClickEvent>((_) => ResetPrefs_Sounds());
    }

    private void UpdateVisibility()
    {
        if (GameManager.instance.StateMachine.CurrentState.Equals(GameManager.instance.StateMachine.settingsState))
        {
            settingsScreen.style.display = DisplayStyle.Flex;
        }
        else
        {
            settingsScreen.style.display = DisplayStyle.None;
        }
    }

    private void SetSliderRange(Slider slider, float minValue, float maxValue)
    {
        slider.highValue = maxValue;
        slider.lowValue = minValue;
    }

    private void ResetPrefs_MovementAndCamera()
    {
        Preferences.ResetPlayerRotationSpeed();
        Preferences.ResetCameraRotationSpeed_Horizontal();
        Preferences.ResetCameraRotationSpeed_Vertical();
        Preferences.ResetCameraDamping_BodyX();
        Preferences.ResetCameraDamping_BodyY();
        Preferences.ResetCameraDamping_BodyZ();
        Preferences.ResetCameraDamping_Aim();
    }

    private void ResetPrefs_Sounds()
    {
        Preferences.ResetBackgroundMusicVolume();
        Preferences.ResetStepsVolume();
        Preferences.ResetDamageEffectVolume();
    }

    private void PopulatePlayerRotationDropdown()
    {
        foreach (KeyValuePair<Preferences.PlayerRotationStyle, string> style in Preferences.PlRotStyles_info)
        {
            plRotStylesChoices.Add(style.Value);
        }
        plRotStyleDropdown.choices = plRotStylesChoices;
        plRotStyleDropdown.value = Preferences.GetPlayerRotationStyleName();
    }

    private void PopulateCameraRotationDropdown()
    {
        camRotStylesChoices.Clear();
        switch (Preferences.PlRotStyle)
        {
            case Preferences.PlayerRotationStyle.withMouse:
                {
                    foreach (KeyValuePair<Preferences.CameraRotationStyle, 
                        (string name, bool isAvailableWithPlRot_Mouse, bool)> style in Preferences.CamRotStyles_info)
                    {
                        if (style.Value.isAvailableWithPlRot_Mouse)
                        {
                            camRotStylesChoices.Add(style.Value.name);
                        }
                    }
                    break;
                }
            case Preferences.PlayerRotationStyle.withKeyboard:
                {
                    foreach (KeyValuePair<Preferences.CameraRotationStyle, 
                        (string name, bool, bool isAvailableWithPlRot_Keyboard)> style in Preferences.CamRotStyles_info)
                    {
                        if (style.Value.isAvailableWithPlRot_Keyboard)
                        {
                            camRotStylesChoices.Add(style.Value.name);
                        } 
                    }
                    break;
                }
        }
        camRotStyleDropdown.choices = camRotStylesChoices;
        camRotStyleDropdown.value = Preferences.GetCameraRotationStyleName();
    }

    private void ChangePlayerRotationStyle(ChangeEvent<string> evt)
    {
        Preferences.SetPlayerRotationStyle(evt.newValue);
    }

    private void ChangeCameraRotationStyle(ChangeEvent<string> evt)
    {
        Preferences.SetCameraRotationStyle(evt.newValue);
    }

    private void UpdatePlayerRotationStyle()
    {
        plRotStyleDropdown.value = Preferences.GetPlayerRotationStyleName();
        PopulateCameraRotationDropdown();
    }
    
    private void UpdateCameraRotationStyle()
    {
        camRotStyleDropdown.value = Preferences.GetCameraRotationStyleName();
    }

    private void ChangePlayerRotSpeed(ChangeEvent<float> evt)
    {
        Preferences.SetPlayerRotationSpeed(evt.newValue);
    }
    private void UpdatePlayerRotSpeed()
    {
        plRotSpeedSlider.value = Preferences.plRotSpeed;
        plRotSpeedSlider.label = Mathf.Round(Preferences.plRotSpeed).ToString();
    }

    private void ChangeCamRotSpeed_Horizontal(ChangeEvent<float> evt)
    {
        Preferences.SetCameraRotationSpeed_Horizontal(evt.newValue);
    }
    private void UpdateCamRotSpeed_Horizontal()
    {
        camRotSpeed_HorizontalSlider.value = Preferences.camRot_HorizontalSpeed;
        camRotSpeed_HorizontalSlider.label = Mathf.Round(Preferences.camRot_HorizontalSpeed).ToString();
    }

    private void ChangeCamRotSpeed_Vertical(ChangeEvent<float> evt)
    {
        Preferences.SetCameraRotationSpeed_Vertical(evt.newValue);
    }
    private void UpdateCamRotSpeed_Vertical()
    {
        camRotSpeed_VerticalSlider.value = Preferences.camRot_VerticalSpeed;
        camRotSpeed_VerticalSlider.label = Mathf.Round(Preferences.camRot_VerticalSpeed).ToString();
    }

    private void ChangeCamDamping_BodyX(ChangeEvent<float> evt)
    {
        Preferences.SetCameraDamping_BodyX(evt.newValue);
    }
    private void UpdateCamDamping_BodyX()
    {
        camDamping_BodyXSlider.value = Preferences.camDamping_BodyX;
        camDamping_BodyXSlider.label = Mathf.Round(Preferences.camDamping_BodyX).ToString();
    }

    private void ChangeCamDamping_BodyY(ChangeEvent<float> evt)
    {
        Preferences.SetCameraDamping_BodyY(evt.newValue); 
    }
    private void UpdateCamDamping_BodyY()
    {
        camDamping_BodyYSlider.value = Preferences.camDamping_BodyY;
        camDamping_BodyYSlider.label = Mathf.Round(Preferences.camDamping_BodyY).ToString();
    }

    private void ChangeCamDamping_BodyZ(ChangeEvent<float> evt)
    {
        Preferences.SetCameraDamping_BodyZ(evt.newValue);
    }
    private void UpdateCamDamping_BodyZ()
    {
        camDamping_BodyZSlider.value = Preferences.camDamping_BodyZ;
        camDamping_BodyZSlider.label = Mathf.Round(Preferences.camDamping_BodyZ).ToString();
    }

    private void ChangeCamDamping_Aim(ChangeEvent<float> evt)
    {
        Preferences.SetCameraDamping_Aim(evt.newValue);
    }
    private void UpdateCamDamping_Aim()
    {
        camDamping_AimSlider.value = Preferences.camDamping_Aim;
        camDamping_AimSlider.label = Mathf.Round(Preferences.camDamping_Aim).ToString();
    }

    private void ChangeBackgroundMusicVolume(ChangeEvent<float> evt)
    {
        Preferences.SetBackgroundMusicVolume(evt.newValue);
    }
    private void UpdateBackgroundMusicVolume()
    {
        backgroundMusicSlider.value = Preferences.backMusicVolume;
        backgroundMusicSlider.label = Mathf.Round(Preferences.backMusicVolume).ToString();
    }

    private void ChangeStepsVolume(ChangeEvent<float> evt)
    {
        Preferences.SetStepsVolume(evt.newValue);
    }
    private void UpdateStepsVolume()
    {
        stepsSlider.value = Preferences.stepsVolume;
        stepsSlider.label = Mathf.Round(Preferences.stepsVolume).ToString();
    }

    private void ChangeDamageEffectVolume(ChangeEvent<float> evt)
    {
        Preferences.SetDamageEffectVolume(evt.newValue);
    }
    private void UpdateDamageEffectVolume()
    {
        damageEffectSlider.value = Preferences.damageEffectVolume;
        damageEffectSlider.label = Mathf.Round(Preferences.damageEffectVolume).ToString();
    }

    private void OnDestroy()
    {
        GameManager.instance.StateMachine.OnStateChanged -= UpdateVisibility;

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
