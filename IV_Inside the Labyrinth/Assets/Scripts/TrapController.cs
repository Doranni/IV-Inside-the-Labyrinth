using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapController : MonoBehaviour
{
    [SerializeField] private float healthDamage_Immediate, sanityDamage_Immediate;
    [SerializeField] private float rechargingTime;
    [SerializeField] private Effect[] effects;

    private HealthController trapHealth;
    private float rechargingTimeLeft;
    private bool isCharged = true;

    public float HealthDamage_Immediate => healthDamage_Immediate;
    public float SanityDamage_Immediate => sanityDamage_Immediate;
    public float RechargingTime => rechargingTime;
    public Effect[] Effects => effects;
    public bool IsCharged => isCharged;

    private void Start()
    {
        trapHealth = GetComponent<HealthController>();
        trapHealth.OnDeath += TrapHealth_OnDeath;
    }

    private void Update()
    {
        if (!isCharged)
        {
            if (rechargingTimeLeft <= 0)
            {
                isCharged = true;
            }
            else
            {
                rechargingTimeLeft -= Time.deltaTime;
            }
        }
    }

    private void TrapHealth_OnDeath()
    {
        Destroy(gameObject);
    }

    public void Activate(float damage)
    {
        isCharged = false;
        rechargingTimeLeft = rechargingTime;
        trapHealth.ChangeHealth(-damage);
    }
}
