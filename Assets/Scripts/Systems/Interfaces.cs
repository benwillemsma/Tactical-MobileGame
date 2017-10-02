using UnityEngine;

public interface ISelectable
{
    void Selected();
    void Deselected();
    void Cancel();
}

public interface IDamageable
{
    void TakeDamage(float damage);
    void Die();
}
