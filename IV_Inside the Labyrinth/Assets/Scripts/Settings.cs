using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Settings : MonoBehaviour
{
    [SerializeField] private Toggle pauseToggle;
    [SerializeField] private TMP_Dropdown plRotStyleDropdown, camRotStyleDropdown;
    [SerializeField] private TMP_InputField plRotSpeedInputField, camRotSpeedInputField;
    [SerializeField] private Slider plRotSpeedSlider, camRotSpeedSlider;
    [SerializeField] private TMP_InputField camFollowDamping_XInputField, camFollowDamping_YInputField, 
        camFollowDamping_ZInputField, camFollowDamping_YawInputField;
    [SerializeField] private Slider camFollowDamping_XSlider, camFollowDamping_YSlider, 
        camFollowDamping_ZSlider, camFollowDamping_YawSlider;
    [SerializeField] private TMP_InputField camRotDamping_HorizontalInputField, camRotDamping_VerticalInputField;
    [SerializeField] private Slider camRotDamping_HorizontalSlider, camRotDamping_VerticalSlider;

    private AnimationAndMovementController movementController;
    private CameraController cameraController;

    private delegate void Func(float value);
    private delegate void UpdateFunc();

    private void Awake()
    {
        movementController = GameObject.Find("Player").GetComponent<AnimationAndMovementController>();
        cameraController = GameObject.Find("MainCamera").GetComponent<CameraController>();

        pauseToggle.isOn = Preferences.isPausedWhileInMenu;
        pauseToggle.onValueChanged.AddListener(SetPauseBehavior);

        PopulatePlayerRotationDropdown();
        PopulateCameraRotationDropdown();
        plRotStyleDropdown.onValueChanged.AddListener(ChangePlayerRotationStyle);
        camRotStyleDropdown.onValueChanged.AddListener(ChangeCameraRotationStyle);

        SetValue(plRotSpeedInputField, plRotSpeedSlider,
            Preferences.plRotSpeedMin, Preferences.plRotSpeedMax, Preferences.plRotSpeed);
        plRotSpeedSlider.onValueChanged.AddListener(ChangePlayerRotSpeed_Float);
        plRotSpeedInputField.onValueChanged.AddListener(ChangePlayerRotSpeed_String);

        SetValue(camRotSpeedInputField, camRotSpeedSlider,
            Preferences.camRotSpeedMin, Preferences.camRotSpeedMax, Preferences.camRotSpeed);
        camRotSpeedSlider.onValueChanged.AddListener(ChangeCamRotSpeed_Float);
        camRotSpeedInputField.onValueChanged.AddListener(ChangeCamRotSpeed_String);

        SetValue(camFollowDamping_XInputField, camFollowDamping_XSlider, 
            Preferences.camDampingMin, Preferences.camDampingMax, Preferences.camFollowDamping_X);
        camFollowDamping_XSlider.onValueChanged.AddListener(ChangeCamFollowDamping_X_Float);
        camFollowDamping_XInputField.onValueChanged.AddListener(ChangeCamFollowDamping_X_String);

        SetValue(camFollowDamping_YInputField, camFollowDamping_YSlider,
            Preferences.camDampingMin, Preferences.camDampingMax, Preferences.camFollowDamping_Y);
        camFollowDamping_YSlider.onValueChanged.AddListener(ChangeCamFollowDamping_Y_Float);
        camFollowDamping_YInputField.onValueChanged.AddListener(ChangeCamFollowDamping_Y_String);

        SetValue(camFollowDamping_ZInputField, camFollowDamping_ZSlider,
            Preferences.camDampingMin, Preferences.camDampingMax, Preferences.camFollowDamping_Z);
        camFollowDamping_ZSlider.onValueChanged.AddListener(ChangeCamFollowDamping_Z_Float);
        camFollowDamping_ZInputField.onValueChanged.AddListener(ChangeCamFollowDamping_Z_String);

        SetValue(camFollowDamping_YawInputField, camFollowDamping_YawSlider,
            Preferences.camDampingMin, Preferences.camDampingMax, Preferences.camFollowDamping_Yaw);
        camFollowDamping_YawSlider.onValueChanged.AddListener(ChangeCamFollowDamping_Yaw_Float);
        camFollowDamping_YawInputField.onValueChanged.AddListener(ChangeCamFollowDamping_Yaw_String);

        SetValue(camRotDamping_HorizontalInputField, camRotDamping_HorizontalSlider,
            Preferences.camDampingMin, Preferences.camDampingMax, Preferences.camRotDamping_Horizontal);
        camRotDamping_HorizontalSlider.onValueChanged.AddListener(ChangeCamRotDamping_Horizontal_Float);
        camRotDamping_HorizontalInputField.onValueChanged.AddListener(ChangeCamRotDamping_Horizontal_String);

        SetValue(camRotDamping_VerticalInputField, camRotDamping_VerticalSlider,
            Preferences.camDampingMin, Preferences.camDampingMax, Preferences.camRotDamping_Vertical);
        camRotDamping_VerticalSlider.onValueChanged.AddListener(ChangeCamRotDamping_Vertical_Float);
        camRotDamping_VerticalInputField.onValueChanged.AddListener(ChangeCamRotDamping_Vertical_String);
    }

    private void SetValue(TMP_InputField inputField, Slider slider, float minValue, float maxValue, float value)
    {
        slider.maxValue = maxValue;
        slider.minValue = minValue;
        slider.value = value;
        inputField.text = value.ToString();
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
        List<TMP_Dropdown.OptionData> playerRotList = new List<TMP_Dropdown.OptionData>();
        foreach ((Preferences.PlayerRotationStyle, string name) style in Preferences.plRotStyles)
        {
            playerRotList.Add(new TMP_Dropdown.OptionData(style.name));
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
        List<TMP_Dropdown.OptionData> cameraRotList = new List<TMP_Dropdown.OptionData>();
        switch (Preferences.plRotStyle)
        {
            case Preferences.PlayerRotationStyle.withMouse:
                {
                    foreach ((Preferences.CameraRotationStyle style, string name, bool isAvailableWithPlRot_Mouse, bool) style in Preferences.camRotStyles)
                    {
                        if (style.isAvailableWithPlRot_Mouse)
                            cameraRotList.Add(new TMP_Dropdown.OptionData(style.name));
                    }
                    break;
                }
            case Preferences.PlayerRotationStyle.withKeyboard:
                {
                    foreach ((Preferences.CameraRotationStyle style, string name, bool, bool isAvailableWithPlRot_Keyboard) style in Preferences.camRotStyles)
                    {
                        if (style.isAvailableWithPlRot_Keyboard)
                            cameraRotList.Add(new TMP_Dropdown.OptionData(style.name));
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
        for (int i = 0; i < Preferences.plRotStyles.Count; i++)
        {
            if (plRotStyleDropdown.options[dropdownValue].text.Equals(Preferences.plRotStyles[i].name))
            {
                Preferences.SetPlayerRotationStyle((int)Preferences.plRotStyles[i].style);
                break;
            }
        }
        movementController.UpdatePlayerRotation();
        cameraController.UpdateRotation();
        PopulateCameraRotationDropdown();
    }

    private void ChangeCameraRotationStyle(int dropdownValue)
    {
        for (int i = 0; i < Preferences.camRotStyles.Count; i++)
        {
            if (camRotStyleDropdown.options[dropdownValue].text.Equals(Preferences.camRotStyles[i].name))
            {
                Preferences.SetCameraRotationStyle((int)Preferences.camRotStyles[i].style);
                break;
            }
        }
        movementController.UpdatePlayerRotation();
        cameraController.UpdateRotation();
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

    private void ChangeCamRotSpeed_Float(float value)
    {
        Preferences.SetCameraRotationSpeed(value);
        camRotSpeedInputField.text = Preferences.camRotSpeed.ToString();
        camRotSpeedSlider.value = Preferences.camRotSpeed;
        cameraController.UpdateRotation();
    }

    private void ChangeCamRotSpeed_String(string value)
    {
        ChangeCamRotSpeed_Float(float.Parse(value));
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
}
