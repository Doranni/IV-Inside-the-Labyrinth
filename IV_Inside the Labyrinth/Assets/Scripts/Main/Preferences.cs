using System;
using System.Collections.Generic;
using UnityEngine;

public class Preferences : Singleton<Preferences>
{
    // Movement and Camera
    public enum PlayerRotationStyle
    {
        withMouse,
        withKeyboard
    }
    public enum CameraRotationStyle
    {
        followPlayer,
        withMouse,
        withRightClickMouse
    }

    public static PlayerRotationStyle PlRotStyle { get; private set; }
    public static CameraRotationStyle CamRotStyle { get; private set; }

    public static Dictionary<PlayerRotationStyle, string> PlRotStyles_info { get; private set; }
    public static Dictionary<CameraRotationStyle, (string name, bool isAvailableWithPlRot_Mouse, 
        bool isAvailableWithPlRot_Keyboard)>
        CamRotStyles_info { get; private set; }

    public static float plRotSpeed, camRot_HorizontalSpeed, camRot_VerticalSpeed;
    public static readonly float plRotSpeedMin = 100, plRotSpeedMax = 300,
        camRotHorizontal_SpeedMin = 100, camRot_HorizontalSpeedMax = 300,
        camRotVertical_SpeedMin = 10, camRot_VerticalSpeedMax = 100;

    public static float camDamping_BodyX, camDamping_BodyY, camDamping_BodyZ, camDamping_Aim;
    public static readonly float camDampingMin = 0, camDampingMax = 10;

    private static bool isPausedWhileInMenu;

    public static bool IsPausedWhileInMenu
    {
        get
        {
            return isPausedWhileInMenu;
        }
        set
        {
            isPausedWhileInMenu = value;
            PlayerPrefs.SetString(isPausedWhileInMenuKey, isPausedWhileInMenu.ToString());
            OnPauseBehaviorChanged?.Invoke();
        }
    }

    private static readonly string plRotStyleKey = "player_rot_style", plRotSpeedKey = "player_rot_speed",
        camRotStyleKey = "camera_rot_style", 
        camRotSpeed_HorizontalKey = "camera_rot_speed_horizontal", camRotSpeed_VerticalKey = "camera_rot_speed_vertical",
        camDamping_BodyXKey = "camera_damping_body_x", camDamping_BodyYKey = "camera_damping_body_y",
        camDamping_BodyZKey = "camera_damping_body_z", camDamping_AimKey = "camera_damping_aim",
        isPausedWhileInMenuKey = "is_paused_in_menu";
       

    public static readonly float plRotSpeed_def = 150, 
        camRotSpeed_Horizontal_def = 150, camRotSpeed_Vertical_def = 70,
        camDamping_BodyX_def = 1, camDamping_BodyY_def = 1, camDamping_BodyZ_def = 1,
        camDamping_Aim_def = 2;

    public static event Action OnPauseBehaviorChanged,
        OnPlRotStyleChanged, OnCamRotStyleChanged,
        OnPlRotSpeedChanged, OnCamRotSpeed_HorizontalChanged, OnCamRotSpeed_VerticalChanged,
        OnCamDamping_BodyXChanged, OnCamDamping_BodyYChanged, OnCamDamping_BodyZChanged, OnCamDamping_AimChanged;

    // Sounds
    private static readonly string backMusicVolumeKey = "background_music_volume", stepsVolumeKey = "steps_volume",
        damageEffectVolumeKey = "damage_effect_volume";

    public static float backMusicVolume, stepsVolume, damageEffectVolume;

    public static readonly float backMusicVolume_def = 30, stepsVolume_def = 100, damageEffectVolume_def = 100;

    public static event Action OnBackgroundMusicVolumeChanged, OnStepsVolumeChanged, OnDamageEffectVolumeChanged;

    public override void Awake()
    {
        base.Awake();
        SetPrefetences();
    }

    private static void SetPrefetences()
    {
        // Movement and Camera
        PlRotStyles_info = new Dictionary<PlayerRotationStyle, string>();
        PlRotStyles_info.Add(PlayerRotationStyle.withMouse, "Mouse");
        PlRotStyles_info.Add(PlayerRotationStyle.withKeyboard, "AD keys or arrow keys");

        CamRotStyles_info = new Dictionary<CameraRotationStyle, (string, bool, bool)>();
        CamRotStyles_info.Add(CameraRotationStyle.followPlayer, ("Follow player", true, true));
        CamRotStyles_info.Add(CameraRotationStyle.withMouse, ("Mouse", false, true));
        CamRotStyles_info.Add(CameraRotationStyle.withRightClickMouse, ("Right click mouse", true, true));

        isPausedWhileInMenu = bool.Parse(PlayerPrefs.GetString(isPausedWhileInMenuKey, "true"));
        SetPlayerRotationStyle(PlayerPrefs.GetInt(plRotStyleKey, 0));
        SetPlayerRotationSpeed(PlayerPrefs.GetFloat(plRotSpeedKey, plRotSpeed_def));
        SetCameraRotationSpeed_Horizontal(PlayerPrefs.GetFloat(camRotSpeed_HorizontalKey, camRotSpeed_Horizontal_def));
        SetCameraRotationSpeed_Vertical(PlayerPrefs.GetFloat(camRotSpeed_VerticalKey, camRotSpeed_Vertical_def));
        SetCameraDamping_BodyX(PlayerPrefs.GetFloat(camDamping_BodyXKey, camDamping_BodyX_def));
        SetCameraDamping_BodyY(PlayerPrefs.GetFloat(camDamping_BodyYKey, camDamping_BodyY_def));
        SetCameraDamping_BodyZ(PlayerPrefs.GetFloat(camDamping_BodyZKey, camDamping_BodyZ_def));
        SetCameraDamping_Aim(PlayerPrefs.GetFloat(camDamping_AimKey, camDamping_Aim_def));

        // Sounds
        SetBackgroundMusicVolume(PlayerPrefs.GetFloat(backMusicVolumeKey, backMusicVolume_def));
        SetStepsVolume(PlayerPrefs.GetFloat(stepsVolumeKey, stepsVolume_def));
        SetDamageEffectVolume(PlayerPrefs.GetFloat(damageEffectVolumeKey, damageEffectVolume_def));
    }

    public static string GetPlayerRotationStyleName()
    {
        return PlRotStyles_info[PlRotStyle];
    }

    public static string GetCameraRotationStyleName()
    {
        return CamRotStyles_info[CamRotStyle].name;
    }

    public static void SetPlayerRotationStyle(int key)
    {
        if (!PlRotStyles_info.ContainsKey((PlayerRotationStyle)key))
        {
            PlRotStyle = PlayerRotationStyle.withMouse;
        }
        else
        {
            PlRotStyle = (PlayerRotationStyle)key;
        }
        SetCameraRotationStyle(PlayerPrefs.GetInt(camRotStyleKey, (int)CamRotStyle));
        PlayerPrefs.SetInt(plRotStyleKey, (int)PlRotStyle);
        OnPlRotStyleChanged?.Invoke();
    }

    public static void SetCameraRotationStyle(int index)
    {
        switch (PlRotStyle)
        {
            case PlayerRotationStyle.withMouse:
                {
                    if(index == 2)
                    {
                        CamRotStyle = CameraRotationStyle.withRightClickMouse;
                    }
                    else
                    {
                        CamRotStyle = CameraRotationStyle.followPlayer;
                    }
                    break;
                }
            case PlayerRotationStyle.withKeyboard:
                {
                    if (index > 2 || index < 0)
                    {
                        CamRotStyle = CameraRotationStyle.followPlayer;
                    }
                    else
                    {
                        CamRotStyle = (CameraRotationStyle)index;
                    }
                    break;
                }
        }
        PlayerPrefs.SetInt(camRotStyleKey, (int)CamRotStyle);
        OnCamRotStyleChanged?.Invoke();
    }

    public static void SetPlayerRotationSpeed(float speed)
    {
        plRotSpeed = Mathf.Clamp(speed, plRotSpeedMin, plRotSpeedMax);
        PlayerPrefs.SetFloat(plRotSpeedKey, plRotSpeed);
        OnPlRotSpeedChanged?.Invoke();
    }

    public static void SetCameraRotationSpeed_Horizontal(float speed)
    {
        camRot_HorizontalSpeed = Mathf.Clamp(speed, camRotHorizontal_SpeedMin, camRot_HorizontalSpeedMax);
        PlayerPrefs.SetFloat(camRotSpeed_HorizontalKey, camRot_HorizontalSpeed);
        OnCamRotSpeed_HorizontalChanged?.Invoke();
    }

    public static void SetCameraRotationSpeed_Vertical(float speed)
    {
        camRot_VerticalSpeed = Mathf.Clamp(speed, camRotVertical_SpeedMin, camRot_VerticalSpeedMax);
        PlayerPrefs.SetFloat(camRotSpeed_VerticalKey, camRot_VerticalSpeed);
        OnCamRotSpeed_VerticalChanged?.Invoke();
    }

    public static void SetCameraDamping_BodyX(float damping)
    {
        camDamping_BodyX = Mathf.Clamp(damping, camDampingMin, camDampingMax);
        PlayerPrefs.SetFloat(camDamping_BodyXKey, camDamping_BodyX);
        OnCamDamping_BodyXChanged?.Invoke();
    }

    public static void SetCameraDamping_BodyY(float damping)
    {
        camDamping_BodyY = Mathf.Clamp(damping, camDampingMin, camDampingMax);
        PlayerPrefs.SetFloat(camDamping_BodyYKey, camDamping_BodyY);
        OnCamDamping_BodyYChanged?.Invoke();
    }

    public static void SetCameraDamping_BodyZ(float damping)
    {
        camDamping_BodyZ = Mathf.Clamp(damping, camDampingMin, camDampingMax);
        PlayerPrefs.SetFloat(camDamping_BodyZKey, camDamping_BodyZ);
        OnCamDamping_BodyZChanged?.Invoke();
    }

    public static void SetCameraDamping_Aim(float damping)
    {
        camDamping_Aim = Mathf.Clamp(damping, camDampingMin, camDampingMax);
        PlayerPrefs.SetFloat(camDamping_AimKey, camDamping_Aim);
        OnCamDamping_AimChanged?.Invoke();
    }

    public static void SetBackgroundMusicVolume(float volume)
    {
        backMusicVolume = Mathf.Clamp(volume, 0, 100);
        PlayerPrefs.SetFloat(backMusicVolumeKey, backMusicVolume);
        OnBackgroundMusicVolumeChanged?.Invoke();
    }

    public static void SetStepsVolume(float volume)
    {
        stepsVolume = Mathf.Clamp(volume, 0, 100);
        PlayerPrefs.SetFloat(stepsVolumeKey, stepsVolume);
        OnStepsVolumeChanged?.Invoke();
    }

    public static void SetDamageEffectVolume(float volume)
    {
        damageEffectVolume = Mathf.Clamp(volume, 0, 100);
        PlayerPrefs.SetFloat(damageEffectVolumeKey, damageEffectVolume);
        OnDamageEffectVolumeChanged?.Invoke();
    }
}
