using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum PlayerState
{
    alive,
    dead
}

public class PlayerHealth : HealthSystem
{
    [Header("health bar display")]
    [SerializeField] private Image deathScreen;

    [SerializeField] InputManager inputManager;

    [SerializeField] private float healCooldown = 5f;

    [SerializeField] private bool isCoop;
    
    public PlayerState playerState = PlayerState.alive;

    private void Update()
    {
        UpdateHealthUI(Color.red, Color.green);

        if (currentHealth <= 0)
            Die();

        if (currentHealth < maxHealth && playerState == PlayerState.alive)
        {
            //if the cooldown is not at 0, then decrease the cooldown
            if (healCooldown > 0)
                healCooldown -= Time.deltaTime;

            //if the cooldown is at 0, then heal the player
            else
                Heal(1 * Time.deltaTime);
        }
        
        if (playerState == PlayerState.dead && !isCoop)
            SceneManager.LoadScene(3);

        else if (playerState == PlayerState.dead && isCoop)
        {
            gameObject.GetComponent<Collider>().enabled = false;
            foreach (Renderer r in GetComponentsInChildren<Renderer>())
            {
                r.enabled = false;
            }
        }
    }


    public override void Heal(float healAmount)
    {
        base.Heal(healAmount);

        //check if the player is at max health, if so, reset the cooldown
        if (currentHealth >= maxHealth)
            healCooldown = 10;
    }
    
    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
        //reset the cooldown
        healCooldown = 10f;
    }

    public override void Die()
    {
        base.Die();
        ScoreSystem.Instance.SaveData();
        playerState = PlayerState.dead;
    }
}
