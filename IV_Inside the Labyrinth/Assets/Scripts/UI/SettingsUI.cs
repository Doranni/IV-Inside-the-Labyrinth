using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(UIDocument))]
public class SettingsUI : MonoBehaviour
{
    private VisualElement settingsScreen;
    const string k_settingsScreen = "SettingsScreen";

    // Movement and Camera
    private DropdownField dropdown_plRotStyle, dropdown_camRotStyle;
    private Label label_plRotStyle, label_camRotStyle, 
        label_camDamping_bodyX, label_camDamping_bodyY, label_camDamping_bodyZ, label_camDamping_bodyAim;
    private Slider slider_plRotSpeed, slider_camRotSpeed_horizontal, slider_camRotSpeed_vertical,
        slider_camDamping_bodyX, slider_camDamping_bodyY, slider_camDamping_bodyZ, slider_camDamping_aim;
    private Button button_reset_movementAndCamera;

    const string k_dropdown_plRotStyle = "DropdownField_PlRotStyle";
    const string k_dropdown_camRotStyle = "DropdownField_CamRotStyle";

    const string k_label_plRotStyle = "Label_PlRotStyle";
    const string k_label_camRotStyle = "Label_CamRotStyle";
    const string k_label_camDamping_bodyX = "Label_CamDamping_BodyX";
    const string k_label_camDamping_bodyY = "Label_CamDamping_BodyY";
    const string k_label_camDamping_bodyZ = "Label_CamDamping_BodyZ";
    const string k_label_camDamping_bodyAim = "Label_CamDamping_Aim";


    const string k_slider_plRotSpeed = "Slider_PlRotSpeed";
    const string k_slider_camRotSpeed_horizontal = "Slider_CamRotSpeed_Horizontal";
    const string k_slider_camRotSpeed_vertical = "Slider_CamRotSpeed_Vertical";

    const string k_slider_camDamping_bodyX = "Slider_CamDamping_BodyX";
    const string k_slider_camDamping_bodyY = "Slider_CamDamping_BodyY";
    const string k_slider_camDamping_bodyZ = "Slider_CamDamping_BodyZ";
    const string k_slider_camDamping_aim = "Slider_CamDamping_Aim";

    const string k_button_reset_movementAndCamera = "Button_Reset_MovementAndCamera";

    private readonly List<string> plRotStylesChoices = new();
    private readonly List<string> camRotStylesChoices = new();

    // Sounds
    private Slider slider_backgroundMusic, slider_steps, slider_damageEffect;
    private Button button_reset_sounds;

    const string k_slider_backgroundMusic = "Slider_BackgroundMusicVolume";
    const string k_slider_steps = "Slider_StepsVolume";
    const string k_slider_damageEffect = "Slider_DamageEffectVolume";

    const string k_button_reset_sounds = "Button_Reset_Sounds";

    private void Awake()
    {
        VisualElement rootElement = GetComponent<UIDocument>().rootVisualElement;
        settingsScreen = rootElement.Q(k_settingsScreen);

        // Movement and Camera
        dropdown_plRotStyle = rootElement.Q<DropdownField>(k_dropdown_plRotStyle);
        dropdown_camRotStyle = rootElement.Q<DropdownField>(k_dropdown_camRotStyle);

        label_plRotStyle = rootElement.Q<Label>(k_label_plRotStyle);
        label_camRotStyle = rootElement.Q<Label>(k_label_camRotStyle);
        label_camDamping_bodyX = rootElement.Q<Label>(k_label_camDamping_bodyX);
        label_camDamping_bodyY = rootElement.Q<Label>(k_label_camDamping_bodyY);
        label_camDamping_bodyZ = rootElement.Q<Label>(k_label_camDamping_bodyZ);
        label_camDamping_bodyAim = rootElement.Q<Label>(k_label_camDamping_bodyAim);

        label_plRotStyle.AddManipulator(new TooltipManipulator(rootElement));
        label_camRotStyle.AddManipulator(new TooltipManipulator(rootElement));
        label_camDamping_bodyX.AddManipulator(new TooltipManipulator(rootElement));
        label_camDamping_bodyY.AddManipulator(new TooltipManipulator(rootElement));
        label_camDamping_bodyZ.AddManipulator(new TooltipManipulator(rootElement));
        label_camDamping_bodyAim.AddManipulator(new TooltipManipulator(rootElement));

        slider_plRotSpeed = rootElement.Q<Slider>(k_slider_plRotSpeed);
        slider_camRotSpeed_horizontal = rootElement.Q<Slider>(k_slider_camRotSpeed_horizontal);
        slider_camRotSpeed_vertical = rootElement.Q<Slider>(k_slider_camRotSpeed_vertical);

        slider_camDamping_bodyX = rootElement.Q<Slider>(k_slider_camDamping_bodyX);
        slider_camDamping_bodyY = rootElement.Q<Slider>(k_slider_camDamping_bodyY);
        slider_camDamping_bodyZ = rootElement.Q<Slider>(k_slider_camDamping_bodyZ);
        slider_camDamping_aim = rootElement.Q<Slider>(k_slider_camDamping_aim);

        button_reset_movementAndCamera = rootElement.Q<Button>(k_button_reset_movementAndCamera);

        // Sounds
        slider_backgroundMusic = rootElement.Q<Slider>(k_slider_backgroundMusic);
        slider_steps = rootElement.Q<Slider>(k_slider_steps);
        slider_damageEffect = rootElement.Q<Slider>(k_slider_damageEffect);

        button_reset_sounds = rootElement.Q<Button>(k_button_reset_sounds);
    }

    private void Start()
    {
        GameManager.instance.StateMachine.OnStateChanged += UpdateVisibility;
        UpdateVisibility();

        // Movement and Camera
        PopulatePlayerRotationDropdown();
        PopulateCameraRotationDropdown();
        dropdown_plRotStyle.RegisterValueChangedCallback(ChangePlayerRotationStyle);
        dropdown_camRotStyle.RegisterValueChangedCallback(ChangeCameraRotationStyle);
        Preferences.OnPlRotStyleChanged += UpdatePlayerRotationStyle;
        Preferences.OnCamRotStyleChanged += UpdateCameraRotationStyle;

        SetSliderRange(slider_plRotSpeed, Preferences.plRotSpeedMin, Preferences.plRotSpeedMax);
        slider_plRotSpeed.RegisterValueChangedCallback(ChangePlayerRotSpeed);
        Preferences.OnPlRotSpeedChanged += UpdatePlayerRotSpeed;
        UpdatePlayerRotSpeed();

        SetSliderRange(slider_camRotSpeed_horizontal, Preferences.camRotHorizontal_SpeedMin, Preferences.camRot_HorizontalSpeedMax);
        slider_camRotSpeed_horizontal.RegisterValueChangedCallback(ChangeCamRotSpeed_Horizontal);
        Preferences.OnCamRotSpeed_HorizontalChanged += UpdateCamRotSpeed_Horizontal;
        UpdateCamRotSpeed_Horizontal();

        SetSliderRange(slider_camRotSpeed_vertical, Preferences.camRotVertical_SpeedMin, Preferences.camRot_VerticalSpeedMax);
        slider_camRotSpeed_vertical.RegisterValueChangedCallback(ChangeCamRotSpeed_Vertical);
        Preferences.OnCamRotSpeed_VerticalChanged += UpdateCamRotSpeed_Vertical;
        UpdateCamRotSpeed_Vertical();

        SetSliderRange(slider_camDamping_bodyX, Preferences.camDampingMin, Preferences.camDampingMax);
        slider_camDamping_bodyX.RegisterValueChangedCallback(ChangeCamDamping_BodyX);
        Preferences.OnCamDamping_BodyXChanged += UpdateCamDamping_BodyX;
        UpdateCamDamping_BodyX();

        SetSliderRange(slider_camDamping_bodyY, Preferences.camDampingMin, Preferences.camDampingMax);
        slider_camDamping_bodyY.RegisterValueChangedCallback(ChangeCamDamping_BodyY);
        Preferences.OnCamDamping_BodyYChanged += UpdateCamDamping_BodyY;
        UpdateCamDamping_BodyY();

        SetSliderRange(slider_camDamping_bodyZ, Preferences.camDampingMin, Preferences.camDampingMax);
        slider_camDamping_bodyZ.RegisterValueChangedCallback(ChangeCamDamping_BodyZ);
        Preferences.OnCamDamping_BodyZChanged += UpdateCamDamping_BodyZ;
        UpdateCamDamping_BodyZ();

        SetSliderRange(slider_camDamping_aim, Preferences.camDampingMin, Preferences.camDampingMax);
        slider_camDamping_aim.RegisterValueChangedCallback(ChangeCamDamping_Aim);
        Preferences.OnCamDamping_AimChanged += UpdateCamDamping_Aim;
        UpdateCamDamping_Aim();

        button_reset_movementAndCamera.RegisterCallback<ClickEvent>((_) => ResetPrefs_MovementAndCamera());

        // Sounds
        SetSliderRange(slider_backgroundMusic, 0, 100);
        slider_backgroundMusic.RegisterValueChangedCallback(ChangeBackgroundMusicVolume);
        Preferences.OnBackgroundMusicVolumeChanged += UpdateBackgroundMusicVolume;
        UpdateBackgroundMusicVolume();

        SetSliderRange(slider_steps, 0, 100);
        slider_steps.RegisterValueChangedCallback(ChangeStepsVolume);
        Preferences.OnStepsVolumeChanged += UpdateStepsVolume;
        UpdateStepsVolume();

        SetSliderRange(slider_damageEffect, 0, 100);
        slider_damageEffect.RegisterValueChangedCallback(ChangeDamageEffectVolume);
        Preferences.OnDamageEffectVolumeChanged += UpdateDamageEffectVolume;
        UpdateDamageEffectVolume();

        button_reset_sounds.RegisterCallback<ClickEvent>((_) => ResetPrefs_Sounds());
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
        dropdown_plRotStyle.choices = plRotStylesChoices;
        dropdown_plRotStyle.value = Preferences.GetPlayerRotationStyleName();
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
        dropdown_camRotStyle.choices = camRotStylesChoices;
        dropdown_camRotStyle.value = Preferences.GetCameraRotationStyleName();
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
        dropdown_plRotStyle.value = Preferences.GetPlayerRotationStyleName();
        PopulateCameraRotationDropdown();
    }
    
    private void UpdateCameraRotationStyle()
    {
        dropdown_camRotStyle.value = Preferences.GetCameraRotationStyleName();
    }

    private void ChangePlayerRotSpeed(ChangeEvent<float> evt)
    {
        Preferences.SetPlayerRotationSpeed(evt.newValue);
    }
    private void UpdatePlayerRotSpeed()
    {
        slider_plRotSpeed.value = Preferences.plRotSpeed;
        slider_plRotSpeed.label = Mathf.Round(Preferences.plRotSpeed).ToString();
    }

    private void ChangeCamRotSpeed_Horizontal(ChangeEvent<float> evt)
    {
        Preferences.SetCameraRotationSpeed_Horizontal(evt.newValue);
    }
    private void UpdateCamRotSpeed_Horizontal()
    {
        slider_camRotSpeed_horizontal.value = Preferences.camRot_HorizontalSpeed;
        slider_camRotSpeed_horizontal.label = Mathf.Round(Preferences.camRot_HorizontalSpeed).ToString();
    }

    private void ChangeCamRotSpeed_Vertical(ChangeEvent<float> evt)
    {
        Preferences.SetCameraRotationSpeed_Vertical(evt.newValue);
    }
    private void UpdateCamRotSpeed_Vertical()
    {
        slider_camRotSpeed_vertical.value = Preferences.camRot_VerticalSpeed;
        slider_camRotSpeed_vertical.label = Mathf.Round(Preferences.camRot_VerticalSpeed).ToString();
    }

    private void ChangeCamDamping_BodyX(ChangeEvent<float> evt)
    {
        Preferences.SetCameraDamping_BodyX(evt.newValue);
    }
    private void UpdateCamDamping_BodyX()
    {
        slider_camDamping_bodyX.value = Preferences.camDamping_BodyX;
        slider_camDamping_bodyX.label = Mathf.Round(Preferences.camDamping_BodyX).ToString();
    }

    private void ChangeCamDamping_BodyY(ChangeEvent<float> evt)
    {
        Preferences.SetCameraDamping_BodyY(evt.newValue); 
    }
    private void UpdateCamDamping_BodyY()
    {
        slider_camDamping_bodyY.value = Preferences.camDamping_BodyY;
        slider_camDamping_bodyY.label = Mathf.Round(Preferences.camDamping_BodyY).ToString();
    }

    private void ChangeCamDamping_BodyZ(ChangeEvent<float> evt)
    {
        Preferences.SetCameraDamping_BodyZ(evt.newValue);
    }
    private void UpdateCamDamping_BodyZ()
    {
        slider_camDamping_bodyZ.value = Preferences.camDamping_BodyZ;
        slider_camDamping_bodyZ.label = Mathf.Round(Preferences.camDamping_BodyZ).ToString();
    }

    private void ChangeCamDamping_Aim(ChangeEvent<float> evt)
    {
        Preferences.SetCameraDamping_Aim(evt.newValue);
    }
    private void UpdateCamDamping_Aim()
    {
        slider_camDamping_aim.value = Preferences.camDamping_Aim;
        slider_camDamping_aim.label = Mathf.Round(Preferences.camDamping_Aim).ToString();
    }

    private void ChangeBackgroundMusicVolume(ChangeEvent<float> evt)
    {
        Preferences.SetBackgroundMusicVolume(evt.newValue);
    }
    private void UpdateBackgroundMusicVolume()
    {
        slider_backgroundMusic.value = Preferences.backMusicVolume;
        slider_backgroundMusic.label = Mathf.Round(Preferences.backMusicVolume).ToString();
    }

    private void ChangeStepsVolume(ChangeEvent<float> evt)
    {
        Preferences.SetStepsVolume(evt.newValue);
    }
    private void UpdateStepsVolume()
    {
        slider_steps.value = Preferences.stepsVolume;
        slider_steps.label = Mathf.Round(Preferences.stepsVolume).ToString();
    }

    private void ChangeDamageEffectVolume(ChangeEvent<float> evt)
    {
        Preferences.SetDamageEffectVolume(evt.newValue);
    }
    private void UpdateDamageEffectVolume()
    {
        slider_damageEffect.value = Preferences.damageEffectVolume;
        slider_damageEffect.label = Mathf.Round(Preferences.damageEffectVolume).ToString();
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
