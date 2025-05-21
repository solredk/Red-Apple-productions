using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerHealth : HealthSystem
{
    [Header("health bar display")]
    [SerializeField] private Image deathScreen;

    [SerializeField] InputManager inputManager;

    [SerializeField] private float healCooldown = 5f;

    private void Update()
    {
        UpdateHealthUI(Color.red, Color.green);

        if ( currentHealth < maxHealth)
        {
            
            if (healCooldown <= 0)
            {
                Heal(1 * Time.deltaTime);
            }
            if (healCooldown > 0)
            {
                healCooldown -= Time.deltaTime;
            }
        }


        if (currentHealth <= 0)
        {
            Die();
        }
    }


    public override void Heal(float healAmount)
    {
        base.Heal(healAmount);

        if (currentHealth >= maxHealth)
        {
            healCooldown = 10;
        }
    }

    public override void Die()
    {
        base.Die();
        deathScreen.gameObject.SetActive(true);
    }
}
