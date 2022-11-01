public interface ICameraState
{
    public void Initialize() { }
    public void Enter() { }
    public void Exit() { }
    public void LateUpdate() { }
    public void MapViewToggle_performed() { }
    public void FirstViewToggle_performed() { }
    public void Destroy() { }
}
