using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController2 : MonoBehaviour
{
    CameraBaseState currentState;
    public CameraFirstViewState firstViewState = new CameraFirstViewState();
    public CameraThirdViewState thirdViewState = new CameraThirdViewState();
    public CameraMapViewState mapViewState = new CameraMapViewState();

    // Start is called before the first frame update
    void Start()
    {
        currentState = thirdViewState;
        currentState.EnterState(this);
    }

    // Update is called once per frame
    void Update()
    {
        currentState.UpdateState(this);
    }

    public void SwitchState(CameraBaseState state)
    {
        currentState = state;
        state.EnterState(this);
    }
}
