using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DamageType
{
    StaticDamage,
    OneTimeUse,
    DamageOverTime
}

public class DamageOnContact : MonoBehaviour
{
    public DamageType type;
    public float damage;

    void OnTriggerEnter(Collider other)
    {
        if (type == DamageType.StaticDamage || type == DamageType.OneTimeUse)
        {
            IDamageable damageObject = other.GetComponent<IDamageable>();
            if (damageObject != null)
                damageObject.TakeDamage(damage);
            if (type == DamageType.OneTimeUse)
                Destroy(gameObject);
         }
    }

    void OnTriggerStay(Collider other)
    {
        if (type == DamageType.DamageOverTime)
        {
            IDamageable damageObject = other.GetComponent<IDamageable>();
            if (damageObject != null)
                damageObject.TakeDamage(damage);
        }
    }
}
