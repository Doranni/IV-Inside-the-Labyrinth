using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealingController : MonoBehaviour
{
    [Header("Ability to start healing while not taking damage")]
    [SerializeField] private float timeBeforeHealing;
    [SerializeField] private float healingPerSecond, healingTimeStep;
    private float healingValueStep;
    private Coroutine healingCoroutine;
    private HealthController healthController;

    private void Start()
    {
        healthController = GetComponent<HealthController>();
        healingValueStep = healingPerSecond * healingTimeStep;
    }

    public void RegisterDamage()
    {
        if (healingCoroutine != null)
        {
            StopCoroutine(healingCoroutine);
        }
        healingCoroutine = StartCoroutine(HealingRoutine());
    }

    private IEnumerator HealingRoutine()
    {
        yield return new WaitForSeconds(timeBeforeHealing);
        while (healthController.Health < healthController.MaxHealth)
        {
            healthController.ChangeHealth(healingValueStep);
            yield return new WaitForSeconds(healingTimeStep);
        }
        StopCoroutine(healingCoroutine);
    }
}
