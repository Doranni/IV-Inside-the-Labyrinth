using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Preferences
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

    public static Dictionary<PlayerRotationStyle, string> plRotStyles { get; private set; }
    public static Dictionary<CameraRotationStyle, (string name, bool isAvailableWithPlRot_Mouse, bool isAvailableWithPlRot_Keyboard)>
        camRotStyles { get; private set; }

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

    // Sounds
    private static string backMusicVolumeKey = "background_music_volume", stepsVolumeKey = "steps_volume",
        damageEffectVolumeKey = "damage_effect_volume";

    public static float backMusicVolume, stepsVolume, damageEffectVolume;

    public static readonly float backMusicVolume_def = 30, stepsVolume_def = 100, damageEffectVolume_def = 100;

    static Preferences()
    {
        // Movement and Camera
        plRotStyles = new Dictionary<PlayerRotationStyle, string>();
        plRotStyles.Add(PlayerRotationStyle.withMouse, "Mouse");
        plRotStyles.Add(PlayerRotationStyle.withKeyboard, "AD keys or arrow keys");

        camRotStyles = new Dictionary<CameraRotationStyle, (string, bool, bool)>();
        camRotStyles.Add(CameraRotationStyle.followPlayer, ("Follow player", true, true));
        camRotStyles.Add(CameraRotationStyle.withMouse, ("Mouse", false, true));
        camRotStyles.Add(CameraRotationStyle.withRightClickMouse, ("Right click mouse", true, true));

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
        return plRotStyles[plRotStyle];
    }

    public static string GetCameraRotationStyleName()
    {
        return camRotStyles[camRotStyle].name;
    }

    public static void SetPlayerRotationStyle(int key)
    {
        if (!plRotStyles.ContainsKey((PlayerRotationStyle)key))
        {
            plRotStyle = PlayerRotationStyle.withMouse;
        }
        plRotStyle = (PlayerRotationStyle)key;
        SetCameraRotationStyle(PlayerPrefs.GetInt(camRotStyleKey, (int)camRotStyle));
        PlayerPrefs.SetInt(plRotStyleKey, (int)plRotStyle);
        PlayerPrefs.SetInt(camRotStyleKey, (int)camRotStyle);
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
    }

    public static void SetPlayerRotationSpeed(float speed)
    {
        plRotSpeed = Mathf.Clamp(speed, plRotSpeedMin, plRotSpeedMax);
        PlayerPrefs.SetFloat(plRotSpeedKey, plRotSpeed);
    }

    public static void SetCameraRotationSpeed_Horizontal(float speed)
    {
        camRot_HorizontalSpeed = Mathf.Clamp(speed, camRotHorizontal_SpeedMin, camRot_HorizontalSpeedMax);
        PlayerPrefs.SetFloat(camRotSpeed_HorizontalKey, camRot_HorizontalSpeed);
    }

    public static void SetCameraRotationSpeed_Vertical(float speed)
    {
        camRot_VerticalSpeed = Mathf.Clamp(speed, camRotVertical_SpeedMin, camRot_VerticalSpeedMax);
        PlayerPrefs.SetFloat(camRotSpeed_VerticalKey, camRot_VerticalSpeed);
    }

    public static void SetCameraFollowingDamping_X(float damping)
    {
        camFollowDamping_X = Mathf.Clamp(damping, camDampingMin, camDampingMax);
        PlayerPrefs.SetFloat(camFollowDamping_XKey, camFollowDamping_X);
    }

    public static void SetCameraFollowingDamping_Y(float damping)
    {
        camFollowDamping_Y = Mathf.Clamp(damping, camDampingMin, camDampingMax);
        PlayerPrefs.SetFloat(camFollowDamping_YKey, camFollowDamping_Y);
    }

    public static void SetCameraFollowingDamping_Z(float damping)
    {
        camFollowDamping_Z = Mathf.Clamp(damping, camDampingMin, camDampingMax);
        PlayerPrefs.SetFloat(camFollowDamping_ZKey, camFollowDamping_Z);
    }

    public static void SetCameraFollowingDamping_Yaw(float damping)
    {
        camFollowDamping_Yaw = Mathf.Clamp(damping, camDampingMin, camDampingMax);
        PlayerPrefs.SetFloat(camFollowDamping_YawKey, camFollowDamping_Yaw);
    }

    public static void SetCameraRotatingDamping_Horizontal(float damping)
    {
        camRotDamping_Horizontal = Mathf.Clamp(damping, camDampingMin, camDampingMax);
        PlayerPrefs.SetFloat(camRotDamping_HorizontalKey, camRotDamping_Horizontal);
    }

    public static void SetCameraRotatingDamping_Vertical(float damping)
    {
        camRotDamping_Vertical = Mathf.Clamp(damping, camDampingMin, camDampingMax);
        PlayerPrefs.SetFloat(camRotDamping_VerticalKey, camRotDamping_Vertical);
    }

    public static void SetBackgroundMusicVolume(float volume)
    {
        backMusicVolume = Mathf.Clamp(volume, 0, 100);
        PlayerPrefs.SetFloat(backMusicVolumeKey, backMusicVolume);
    }

    public static void SetStepsVolume(float volume)
    {
        stepsVolume = Mathf.Clamp(volume, 0, 100);
        PlayerPrefs.SetFloat(stepsVolumeKey, stepsVolume);
    }

    public static void SetDamageEffectVolume(float volume)
    {
        damageEffectVolume = Mathf.Clamp(volume, 0, 100);
        PlayerPrefs.SetFloat(damageEffectVolumeKey, damageEffectVolume);
    }
}
