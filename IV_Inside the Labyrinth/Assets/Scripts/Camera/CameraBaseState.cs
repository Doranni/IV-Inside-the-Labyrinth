using UnityEngine;

public abstract class CameraBaseState
{
    public abstract void EnterState(CameraController2 camera);

    public abstract void UpdateState(CameraController2 camera);
}
