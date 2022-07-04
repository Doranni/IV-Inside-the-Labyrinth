using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Preferences
{
    public static PlayerRotationStyle plRotStyle { get; private set; }
    public static CameraRotationStyle camRotStyle { get; set; }

    public static List<(PlayerRotationStyle style, string name)> plRotStyles { get; private set; }
    public static List<(CameraRotationStyle style, string name, bool isAvailableWithPlRot_Mouse, bool isAvailableWithPlRot_Keyboard)>
        camRotStyles { get; private set; }

    public static float plRotSpeed, plRotSpeedMin = 100, plRotSpeedMax = 300,
        camRotSpeed, camRotSpeedMin = 100, camRotSpeedMax = 300;

    private static bool _isPausedWhileInMenu;

    public static float camFollowDamping_X, camFollowDamping_Y, camFollowDamping_Z, camFollowDamping_Yaw,
        camRotDamping_Horizontal, camRotDamping_Vertical, 
        camDampingMin = 0, camDampingMax = 20;

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

    private static string plRotStyleKey = "player_rot_style", camRotStyleKey = "camera_rot_style",
        plRotSpeedKey = "player_rot_speed", camRotSpeedKey = "camera_rot_speed",
        camFollowDamping_XKey = "camera_follow_damping_x", camFollowDamping_YKey = "camera_follow_damping_y", 
        camFollowDamping_ZKey = "camera_follow_damping_z", camFollowDamping_YawKey = "camera_follow_damping_yaw",
        camRotDamping_HorizontalKey = "camera_rot_damping_horizontal", camRotDamping_VerticalKey = "camera_rot_damping_vertical",
        isPausedWhileInMenuKey = "is_paused_in_menu";

    static Preferences()
    {
        plRotStyles = new List<(PlayerRotationStyle, string)>();
        plRotStyles.Add((PlayerRotationStyle.withMouse, "Mouse"));
        plRotStyles.Add((PlayerRotationStyle.withKeyboard, "AD keys or arrow keys"));

        camRotStyles = new List<(CameraRotationStyle, string, bool, bool)>();
        camRotStyles.Add((CameraRotationStyle.followPlayer, "Follow player", true, true));
        camRotStyles.Add((CameraRotationStyle.withMouse, "Mouse", false, true));
        camRotStyles.Add((CameraRotationStyle.withRightClickMouse, "Right click mouses", true, true));

        _isPausedWhileInMenu = bool.Parse(PlayerPrefs.GetString(isPausedWhileInMenuKey, "true"));
        SetPlayerRotationStyle(PlayerPrefs.GetInt(plRotStyleKey, 0));
        SetPlayerRotationSpeed(PlayerPrefs.GetFloat(plRotSpeedKey, plRotSpeedMin));
        SetCameraRotationSpeed(PlayerPrefs.GetFloat(camRotSpeedKey, camRotSpeedMin));
        SetCameraFollowingDamping_X(PlayerPrefs.GetFloat(camFollowDamping_XKey, 0.5f));
        SetCameraFollowingDamping_Y(PlayerPrefs.GetFloat(camFollowDamping_YKey, 0.5f));
        SetCameraFollowingDamping_Z(PlayerPrefs.GetFloat(camFollowDamping_ZKey, 0.5f));
        SetCameraFollowingDamping_Yaw(PlayerPrefs.GetFloat(camFollowDamping_YawKey, 0.5f));
        SetCameraRotatingDamping_Horizontal(PlayerPrefs.GetFloat(camRotDamping_HorizontalKey, 0.5f));
        SetCameraRotatingDamping_Vertical(PlayerPrefs.GetFloat(camRotDamping_VerticalKey, 0.5f));
    }

    public static string GetPlayerRotationStyleName()
    {
        return plRotStyles[(int)plRotStyle].name;
    }

    public static string GetCameraRotationStyleName()
    {
        return camRotStyles[(int)camRotStyle].name;
    }

    public static void SetPlayerRotationStyle(int index)
    {
        Debug.Log("SetPlayerRotationStyle: index - " + index);
        switch (index)
        {
            case 1:
                {
                    plRotStyle = PlayerRotationStyle.withKeyboard;
                    Debug.Log("SetPlayerRotationStyle - withKeyboard");
                    break;
                }
            default:
                {
                    plRotStyle = PlayerRotationStyle.withMouse;
                    Debug.Log("SetPlayerRotationStyle - withMouse");
                    break;
                }   
        }
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

    public static void SetCameraRotationSpeed(float speed)
    {
        camRotSpeed = Mathf.Clamp(speed, camRotSpeedMin, camRotSpeedMax);
        PlayerPrefs.SetFloat(camRotSpeedKey, camRotSpeed);
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
}
