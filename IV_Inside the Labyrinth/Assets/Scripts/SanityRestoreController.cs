using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SanityRestoreController : MonoBehaviour
{
    [Header("Ability to restore sanity being near with light")]
    [SerializeField] private float timeBeforeRestoringSanity;
    [SerializeField] private float restoringPerSecond, restoringTimeStep;
    private float restoringValueStep;
    private Coroutine restoringSanityCoroutine;
    private SanityController sanityController;

    private bool _isUnderSanityLightEffect = false;
    public bool isUnderSanityLightEffect => _isUnderSanityLightEffect;

    public delegate void Delegate();
    public event Delegate OnSanityLightEnter, OnSanityLightExit;

    void Start()
    {
        sanityController = GetComponent<SanityController>();
        restoringValueStep = restoringPerSecond * restoringTimeStep;
    }

    public void EnterSanityLight()
    {
        if (!_isUnderSanityLightEffect)
        {
            restoringSanityCoroutine = StartCoroutine(RestoreSanityRoutine());
            _isUnderSanityLightEffect = true;
            if (OnSanityLightEnter != null)
            {
                OnSanityLightEnter();
            }
        }
    }

    public void ExitSanityLight()
    {
        if (_isUnderSanityLightEffect)
        {
            StopCoroutine(restoringSanityCoroutine);
            _isUnderSanityLightEffect = false;
            if (OnSanityLightExit != null)
            {
                OnSanityLightExit();
            }
        }
    }

    private IEnumerator RestoreSanityRoutine()
    {
        yield return new WaitForSeconds(timeBeforeRestoringSanity);
        while (true)
        {
            if (sanityController.Sanity < sanityController.MaxSanity)
            {
                sanityController.ChangeSanity(restoringValueStep);
            }
            yield return new WaitForSeconds(restoringTimeStep);
        }
    }
}
