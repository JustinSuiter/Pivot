using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    public float maxHealth = 100f;
    protected float _currentHealth;

    protected virtual void Start()
    {
        _currentHealth = maxHealth;
    }

    public virtual void TakeDamage(float amount)
    {
        _currentHealth -= amount;
        if (_currentHealth <= 0f)
            Die();
    }

    protected virtual void Die()
    {
        Destroy(gameObject);
    }
}