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
            PlayerPrefs.SetString(key_isPausedWhileInMenu, isPausedWhileInMenu.ToString());
            OnPauseBehaviorChanged?.Invoke();
        }
    }

    private static readonly string key_plRotStyle = "player_rot_style", key_plRotSpeed = "player_rot_speed",
        key_camRotStyle = "camera_rot_style", 
        key_camRotSpeed_Horizontal = "camera_rot_speed_horizontal", key_camRotSpeed_Vertical = "camera_rot_speed_vertical",
        key_camDamping_BodyX = "camera_damping_body_x", key_camDamping_BodyY = "camera_damping_body_y",
        key_camDamping_BodyZ = "camera_damping_body_z", key_camDamping_Aim = "camera_damping_aim",
        key_isPausedWhileInMenu = "is_paused_in_menu";
       

    public static readonly float def_plRotSpeed = 150, 
        def_camRotSpeed_Horizontal = 150, def_camRotSpeed_Vertical = 70,
        def_camDamping_BodyX = 1, def_camDamping_BodyY = 1, def_camDamping_BodyZ = 1,
        def_camDamping_Aim = 2;

    public static event Action OnPauseBehaviorChanged,
        OnPlRotStyleChanged, OnCamRotStyleChanged,
        OnPlRotSpeedChanged, OnCamRotSpeed_HorizontalChanged, OnCamRotSpeed_VerticalChanged,
        OnCamDamping_BodyXChanged, OnCamDamping_BodyYChanged, OnCamDamping_BodyZChanged, OnCamDamping_AimChanged;

    // Sounds
    private static readonly string key_backMusicVolume = "background_music_volume", key_stepsVolume = "steps_volume",
        key_damageEffectVolume = "damage_effect_volume";

    public static float backMusicVolume, stepsVolume, damageEffectVolume;

    public static readonly float def_backMusicVolume = 30, def_stepsVolume = 100, def_damageEffectVolume = 100;

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

        isPausedWhileInMenu = bool.Parse(PlayerPrefs.GetString(key_isPausedWhileInMenu, "true"));
        SetPlayerRotationStyle(PlayerPrefs.GetInt(key_plRotStyle, 0));
        SetPlayerRotationSpeed(PlayerPrefs.GetFloat(key_plRotSpeed, def_plRotSpeed));
        SetCameraRotationSpeed_Horizontal(PlayerPrefs.GetFloat(key_camRotSpeed_Horizontal, def_camRotSpeed_Horizontal));
        SetCameraRotationSpeed_Vertical(PlayerPrefs.GetFloat(key_camRotSpeed_Vertical, def_camRotSpeed_Vertical));
        SetCameraDamping_BodyX(PlayerPrefs.GetFloat(key_camDamping_BodyX, def_camDamping_BodyX));
        SetCameraDamping_BodyY(PlayerPrefs.GetFloat(key_camDamping_BodyY, def_camDamping_BodyY));
        SetCameraDamping_BodyZ(PlayerPrefs.GetFloat(key_camDamping_BodyZ, def_camDamping_BodyZ));
        SetCameraDamping_Aim(PlayerPrefs.GetFloat(key_camDamping_Aim, def_camDamping_Aim));

        // Sounds
        SetBackgroundMusicVolume(PlayerPrefs.GetFloat(key_backMusicVolume, def_backMusicVolume));
        SetStepsVolume(PlayerPrefs.GetFloat(key_stepsVolume, def_stepsVolume));
        SetDamageEffectVolume(PlayerPrefs.GetFloat(key_damageEffectVolume, def_damageEffectVolume));
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
        SetCameraRotationStyle(PlayerPrefs.GetInt(key_camRotStyle, (int)CamRotStyle));
        PlayerPrefs.SetInt(key_plRotStyle, (int)PlRotStyle);
        OnPlRotStyleChanged?.Invoke();
    }

    public static void SetPlayerRotationStyle(string value)
    {
        if (!PlRotStyles_info.ContainsValue(value))
        {
            PlRotStyle = PlayerRotationStyle.withMouse;
        }
        else
        {
            foreach (KeyValuePair<PlayerRotationStyle, string> style in PlRotStyles_info)
            {
                if (value.Equals(style.Value))
                {
                    PlRotStyle = style.Key;
                    break;
                }
            }
        }
        SetCameraRotationStyle(PlayerPrefs.GetInt(key_camRotStyle, (int)CamRotStyle));
        PlayerPrefs.SetInt(key_plRotStyle, (int)PlRotStyle);
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
        PlayerPrefs.SetInt(key_camRotStyle, (int)CamRotStyle);
        OnCamRotStyleChanged?.Invoke();
    }

    public static void SetCameraRotationStyle(string value)
    {
        foreach (KeyValuePair<CameraRotationStyle, (string name, bool, bool)> style in CamRotStyles_info)
        {
            if (value.Equals(style.Value.name))
            {
                SetCameraRotationStyle((int)style.Key);
                return;
            }
        }
        SetCameraRotationStyle(0);
    }

    public static void SetPlayerRotationSpeed(float speed)
    {
        plRotSpeed = Mathf.Clamp(speed, plRotSpeedMin, plRotSpeedMax);
        PlayerPrefs.SetFloat(key_plRotSpeed, plRotSpeed);
        OnPlRotSpeedChanged?.Invoke();
    }
    public static void ResetPlayerRotationSpeed()
    {
        SetPlayerRotationSpeed(def_plRotSpeed);
    }

    public static void SetCameraRotationSpeed_Horizontal(float speed)
    {
        camRot_HorizontalSpeed = Mathf.Clamp(speed, camRotHorizontal_SpeedMin, camRot_HorizontalSpeedMax);
        PlayerPrefs.SetFloat(key_camRotSpeed_Horizontal, camRot_HorizontalSpeed);
        OnCamRotSpeed_HorizontalChanged?.Invoke();
    }
    public static void ResetCameraRotationSpeed_Horizontal()
    {
        SetCameraRotationSpeed_Horizontal(def_camRotSpeed_Horizontal);
    }

    public static void SetCameraRotationSpeed_Vertical(float speed)
    {
        camRot_VerticalSpeed = Mathf.Clamp(speed, camRotVertical_SpeedMin, camRot_VerticalSpeedMax);
        PlayerPrefs.SetFloat(key_camRotSpeed_Vertical, camRot_VerticalSpeed);
        OnCamRotSpeed_VerticalChanged?.Invoke();
    }
    public static void ResetCameraRotationSpeed_Vertical()
    {
        SetCameraRotationSpeed_Vertical(def_camRotSpeed_Vertical);
    }

    public static void SetCameraDamping_BodyX(float damping)
    {
        camDamping_BodyX = Mathf.Clamp(damping, camDampingMin, camDampingMax);
        PlayerPrefs.SetFloat(key_camDamping_BodyX, camDamping_BodyX);
        OnCamDamping_BodyXChanged?.Invoke();
    }
    public static void ResetCameraDamping_BodyX()
    {
        SetCameraDamping_BodyX(def_camDamping_BodyX);
    }

    public static void SetCameraDamping_BodyY(float damping)
    {
        camDamping_BodyY = Mathf.Clamp(damping, camDampingMin, camDampingMax);
        PlayerPrefs.SetFloat(key_camDamping_BodyY, camDamping_BodyY);
        OnCamDamping_BodyYChanged?.Invoke();
    }
    public static void ResetCameraDamping_BodyY()
    {
        SetCameraDamping_BodyY(def_camDamping_BodyY);
    }

    public static void SetCameraDamping_BodyZ(float damping)
    {
        camDamping_BodyZ = Mathf.Clamp(damping, camDampingMin, camDampingMax);
        PlayerPrefs.SetFloat(key_camDamping_BodyZ, camDamping_BodyZ);
        OnCamDamping_BodyZChanged?.Invoke();
    }
    public static void ResetCameraDamping_BodyZ()
    {
        SetCameraDamping_BodyZ(def_camDamping_BodyZ);
    }

    public static void SetCameraDamping_Aim(float damping)
    {
        camDamping_Aim = Mathf.Clamp(damping, camDampingMin, camDampingMax);
        PlayerPrefs.SetFloat(key_camDamping_Aim, camDamping_Aim);
        OnCamDamping_AimChanged?.Invoke();
    }
    public static void ResetCameraDamping_Aim()
    {
        SetCameraDamping_Aim(def_camDamping_Aim);
    }

    public static void SetBackgroundMusicVolume(float volume)
    {
        backMusicVolume = Mathf.Clamp(volume, 0, 100);
        PlayerPrefs.SetFloat(key_backMusicVolume, backMusicVolume);
        OnBackgroundMusicVolumeChanged?.Invoke();
    }
    public static void ResetBackgroundMusicVolume()
    {
        SetBackgroundMusicVolume(def_backMusicVolume);
    }

    public static void SetStepsVolume(float volume)
    {
        stepsVolume = Mathf.Clamp(volume, 0, 100);
        PlayerPrefs.SetFloat(key_stepsVolume, stepsVolume);
        OnStepsVolumeChanged?.Invoke();
    }
    public static void ResetStepsVolume()
    {
        SetStepsVolume(def_stepsVolume);
    }

    public static void SetDamageEffectVolume(float volume)
    {
        damageEffectVolume = Mathf.Clamp(volume, 0, 100);
        PlayerPrefs.SetFloat(key_damageEffectVolume, damageEffectVolume);
        OnDamageEffectVolumeChanged?.Invoke();
    }
    public static void ResetDamageEffectVolume()
    {
        SetDamageEffectVolume(def_damageEffectVolume);
    }
}
