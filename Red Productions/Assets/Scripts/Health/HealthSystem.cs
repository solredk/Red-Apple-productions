using UnityEngine;

public abstract class HealthSystem : MonoBehaviour
{
    [SerializeField] protected float currentHealth;
    [SerializeField] protected float maxHealth;

    private void Start()
    {
        currentHealth = maxHealth;
    }



    public virtual void TakeDamage(float damage)
    {
        currentHealth -= damage;
    }

    public virtual void Heal(float healAmount)
    {
        // Ensure we don't heal beyond max health
        if (currentHealth + healAmount > maxHealth)
        {
            healAmount = maxHealth - currentHealth;
        }

        currentHealth += healAmount;
    }

    public virtual void Die()
    {
        //death logic
    }
}
