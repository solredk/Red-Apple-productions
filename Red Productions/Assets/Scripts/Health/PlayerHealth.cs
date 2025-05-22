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

        if (currentHealth <= 0)
            Die();

        if (currentHealth < maxHealth)
        {
            //if the cooldown is not at 0, then decrease the cooldown
            if (healCooldown > 0)
                healCooldown -= Time.deltaTime;

            //if the cooldown is at 0, then heal the player
            else
                Heal(1 * Time.deltaTime);
        }
    }


    public override void Heal(float healAmount)
    {
        base.Heal(healAmount);

        //check if the player is at max health, if so, reset the cooldown
        if (currentHealth >= maxHealth)
            healCooldown = 10;
    }

    public override void Die()
    {
        base.Die();
      //  deathScreen.gameObject.SetActive(true);
    }
}
