using UnityEngine;

public interface ISelectable
{
    void Selected();
    void Action(Vector3 point);
    void Deselected();
    void Cancel();
}

public interface IDamageable
{
    void TakeDamage(float damage);
    void Die();
}
