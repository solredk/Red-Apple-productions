using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerHealth : HealthSystem
{
    [Header("health bar display")]
    [SerializeField] private Image deathScreen;

    private void Update()
    {
        UpdateHealthUI();
        if (Input.GetKeyDown(KeyCode.L))
        {
            TakeDamage(10);
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            Heal(10);
        }
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public override void TakeDamage(float damage)
    {
        //the base take damage function
        base.TakeDamage(damage);

        //reset the lerp timer
        lerpTimer = 0;

        //update the health UI
        UpdateHealthUI();
    }

    public override void Heal(float healAmount)
    {
        //the base heal function
        base.Heal(healAmount);

        //reset the lerp timer
        lerpTimer = 0;

        //update the health UI
        UpdateHealthUI();
    }

    public override void Die()
    {
        base.Die();
        deathScreen.gameObject.SetActive(true);
    }
}
