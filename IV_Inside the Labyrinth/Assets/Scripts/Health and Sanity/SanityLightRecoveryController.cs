using System;
using System.Collections;
using UnityEngine;

public class SanityLightRecoveryController : MonoBehaviour
{
    [Header("Ability to recover Sanity being near with Sanity Light")]
    [SerializeField] private float timeBeforeRecovery;
    [SerializeField] private float sanityRestoringPerSecond, restoringTimeStep;

    private float sanityRestoringValueStep;
    private Coroutine recoveryCoroutine;
    private SanityController sanityController;

    private bool isUnderSanityLightEffect = false;
    public bool IsUnderSanityLightEffect => isUnderSanityLightEffect;

    public event Action OnSanityLightEnter, OnSanityLightExit;

    void Start()
    {
        sanityController = GetComponent<SanityController>();
        sanityRestoringValueStep = sanityRestoringPerSecond * restoringTimeStep;
    }

    public void EnterSanityLight()
    {
        if (!isUnderSanityLightEffect)
        {
            recoveryCoroutine = StartCoroutine(RestoreSanityRoutine());
            isUnderSanityLightEffect = true;
            OnSanityLightEnter?.Invoke();
        }
    }

    public void ExitSanityLight()
    {
        if (isUnderSanityLightEffect)
        {
            StopCoroutine(recoveryCoroutine);
            isUnderSanityLightEffect = false;
            OnSanityLightExit?.Invoke();
        }
    }

    private IEnumerator RestoreSanityRoutine()
    {
        yield return new WaitForSeconds(timeBeforeRecovery);
        while (true)
        {
            if (sanityController.Sanity < sanityController.MaxSanity)
            {
                sanityController.ChangeSanity(sanityRestoringValueStep);
            }
            yield return new WaitForSeconds(restoringTimeStep);
        }
    }
}
