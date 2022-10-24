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

    public static PlayerRotationStyle plRotStyle { get; private set; }
    public static CameraRotationStyle camRotStyle { get; private set; }

    public static Dictionary<PlayerRotationStyle, string> plRotStyles_info { get; private set; }
    public static Dictionary<CameraRotationStyle, (string name, bool isAvailableWithPlRot_Mouse, 
        bool isAvailableWithPlRot_Keyboard)>
        camRotStyles_info { get; private set; }

    public static float plRotSpeed, camRot_HorizontalSpeed, camRot_VerticalSpeed;
    public static readonly float plRotSpeedMin = 100, plRotSpeedMax = 300,
        camRotHorizontal_SpeedMin = 100, camRot_HorizontalSpeedMax = 300,
        camRotVertical_SpeedMin = 10, camRot_VerticalSpeedMax = 100;

    public static float camFollowDamping_X, camFollowDamping_Y, camFollowDamping_Z, camFollowDamping_Yaw,
        camRotDamping_Horizontal, camRotDamping_Vertical;
    public static readonly float camDampingMin = 0, camDampingMax = 20;

    private static bool _isPausedWhileInMenu;

    public static bool isPausedWhileInMenu
    {
        get
        {
            return _isPausedWhileInMenu;
        }
        set
        {
            _isPausedWhileInMenu = value;
            PlayerPrefs.SetString(isPausedWhileInMenuKey, _isPausedWhileInMenu.ToString());
            OnPauseBehaviorChanged?.Invoke();
        }
    }

    private static string plRotStyleKey = "player_rot_style", plRotSpeedKey = "player_rot_speed",
        camRotStyleKey = "camera_rot_style", 
        camRotSpeed_HorizontalKey = "camera_rot_speed_horizontal", camRotSpeed_VerticalKey = "camera_rot_speed_vertical",
        camFollowDamping_XKey = "camera_follow_damping_x", camFollowDamping_YKey = "camera_follow_damping_y",
        camFollowDamping_ZKey = "camera_follow_damping_z", camFollowDamping_YawKey = "camera_follow_damping_yaw",
        camRotDamping_HorizontalKey = "camera_rot_damping_horizontal", camRotDamping_VerticalKey = "camera_rot_damping_vertical",
        isPausedWhileInMenuKey = "is_paused_in_menu";
       

    public static readonly float plRotSpeed_def = 150, 
        camRotSpeed_Horizontal_def = 150, camRotSpeed_Vertical_def = 70,
        camFollowDamping_X_def = 3, camFollowDamping_Y_def = 0, camFollowDamping_Z_def = 0,
        camFollowDamping_Yaw_def = 3, camRotDamping_Horizontal_def = 0.5f, camRotDamping_Vertical_def = 0.5f;

    public static event Action OnPauseBehaviorChanged,
        OnPlRotStyleChanged, OnCamRotStyleChanged,
        OnPlRotSpeedChanged, OnCamRotSpeed_HorizontalChanged, OnCamRotSpeed_VerticalChanged,
        OnCamFollowDamping_XChanged, OnCamFollowDamping_YChanged, OnCamFollowDamping_ZChanged, OnCamFollowDamping_YawChanged,
        OnCamRotDamping_HorizontalChanged, OnCamRotDamping_VerticalChanged;

    // Sounds
    private static string backMusicVolumeKey = "background_music_volume", stepsVolumeKey = "steps_volume",
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
        plRotStyles_info = new Dictionary<PlayerRotationStyle, string>();
        plRotStyles_info.Add(PlayerRotationStyle.withMouse, "Mouse");
        plRotStyles_info.Add(PlayerRotationStyle.withKeyboard, "AD keys or arrow keys");

        camRotStyles_info = new Dictionary<CameraRotationStyle, (string, bool, bool)>();
        camRotStyles_info.Add(CameraRotationStyle.followPlayer, ("Follow player", true, true));
        camRotStyles_info.Add(CameraRotationStyle.withMouse, ("Mouse", false, true));
        camRotStyles_info.Add(CameraRotationStyle.withRightClickMouse, ("Right click mouse", true, true));

        _isPausedWhileInMenu = bool.Parse(PlayerPrefs.GetString(isPausedWhileInMenuKey, "true"));
        SetPlayerRotationStyle(PlayerPrefs.GetInt(plRotStyleKey, 0));
        SetPlayerRotationSpeed(PlayerPrefs.GetFloat(plRotSpeedKey, plRotSpeed_def));
        SetCameraRotationSpeed_Horizontal(PlayerPrefs.GetFloat(camRotSpeed_HorizontalKey, camRotSpeed_Horizontal_def));
        SetCameraRotationSpeed_Vertical(PlayerPrefs.GetFloat(camRotSpeed_VerticalKey, camRotSpeed_Vertical_def));
        SetCameraFollowingDamping_X(PlayerPrefs.GetFloat(camFollowDamping_XKey, camFollowDamping_X_def));
        SetCameraFollowingDamping_Y(PlayerPrefs.GetFloat(camFollowDamping_YKey, camFollowDamping_Y_def));
        SetCameraFollowingDamping_Z(PlayerPrefs.GetFloat(camFollowDamping_ZKey, camFollowDamping_Z_def));
        SetCameraFollowingDamping_Yaw(PlayerPrefs.GetFloat(camFollowDamping_YawKey, camFollowDamping_Yaw_def));
        SetCameraRotatingDamping_Horizontal(PlayerPrefs.GetFloat(camRotDamping_HorizontalKey, camRotDamping_Horizontal_def));
        SetCameraRotatingDamping_Vertical(PlayerPrefs.GetFloat(camRotDamping_VerticalKey, camRotDamping_Vertical_def));

        // Sounds
        SetBackgroundMusicVolume(PlayerPrefs.GetFloat(backMusicVolumeKey, backMusicVolume_def));
        SetStepsVolume(PlayerPrefs.GetFloat(stepsVolumeKey, stepsVolume_def));
        SetDamageEffectVolume(PlayerPrefs.GetFloat(damageEffectVolumeKey, damageEffectVolume_def));
    }

    public static string GetPlayerRotationStyleName()
    {
        return plRotStyles_info[plRotStyle];
    }

    public static string GetCameraRotationStyleName()
    {
        return camRotStyles_info[camRotStyle].name;
    }

    public static void SetPlayerRotationStyle(int key)
    {
        if (!plRotStyles_info.ContainsKey((PlayerRotationStyle)key))
        {
            plRotStyle = PlayerRotationStyle.withMouse;
        }
        else
        {
            plRotStyle = (PlayerRotationStyle)key;
        }
        SetCameraRotationStyle(PlayerPrefs.GetInt(camRotStyleKey, (int)camRotStyle));
        PlayerPrefs.SetInt(plRotStyleKey, (int)plRotStyle);
        OnPlRotStyleChanged?.Invoke();
    }

    public static void SetCameraRotationStyle(int index)
    {
        switch (plRotStyle)
        {
            case PlayerRotationStyle.withMouse:
                {
                    if(index == 2)
                    {
                        camRotStyle = CameraRotationStyle.withRightClickMouse;
                    }
                    else
                    {
                        camRotStyle = CameraRotationStyle.followPlayer;
                    }
                    break;
                }
            case PlayerRotationStyle.withKeyboard:
                {
                    if (index > 2 || index < 0)
                    {
                        camRotStyle = CameraRotationStyle.followPlayer;
                    }
                    else
                    {
                        camRotStyle = (CameraRotationStyle)index;
                    }
                    break;
                }
        }
        PlayerPrefs.SetInt(camRotStyleKey, (int)camRotStyle);
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

    public static void SetCameraFollowingDamping_X(float damping)
    {
        camFollowDamping_X = Mathf.Clamp(damping, camDampingMin, camDampingMax);
        PlayerPrefs.SetFloat(camFollowDamping_XKey, camFollowDamping_X);
        OnCamFollowDamping_XChanged?.Invoke();
    }

    public static void SetCameraFollowingDamping_Y(float damping)
    {
        camFollowDamping_Y = Mathf.Clamp(damping, camDampingMin, camDampingMax);
        PlayerPrefs.SetFloat(camFollowDamping_YKey, camFollowDamping_Y);
        OnCamFollowDamping_YChanged?.Invoke();
    }

    public static void SetCameraFollowingDamping_Z(float damping)
    {
        camFollowDamping_Z = Mathf.Clamp(damping, camDampingMin, camDampingMax);
        PlayerPrefs.SetFloat(camFollowDamping_ZKey, camFollowDamping_Z);
        OnCamFollowDamping_ZChanged?.Invoke();
    }

    public static void SetCameraFollowingDamping_Yaw(float damping)
    {
        camFollowDamping_Yaw = Mathf.Clamp(damping, camDampingMin, camDampingMax);
        PlayerPrefs.SetFloat(camFollowDamping_YawKey, camFollowDamping_Yaw);
        OnCamFollowDamping_YawChanged?.Invoke();
    }

    public static void SetCameraRotatingDamping_Horizontal(float damping)
    {
        camRotDamping_Horizontal = Mathf.Clamp(damping, camDampingMin, camDampingMax);
        PlayerPrefs.SetFloat(camRotDamping_HorizontalKey, camRotDamping_Horizontal);
        OnCamRotDamping_HorizontalChanged?.Invoke();
    }

    public static void SetCameraRotatingDamping_Vertical(float damping)
    {
        camRotDamping_Vertical = Mathf.Clamp(damping, camDampingMin, camDampingMax);
        PlayerPrefs.SetFloat(camRotDamping_VerticalKey, camRotDamping_Vertical);
        OnCamRotDamping_VerticalChanged?.Invoke();
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
