using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveState : IState
{
    [SerializeField]
    private GameObject healthAndSanityPanel;

    public void Enter()
    {
        Time.timeScale = 1;
    }
}
