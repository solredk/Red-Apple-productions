using System.Runtime.CompilerServices;
using TMPro;
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

    [SerializeField] private InputManager inputManager;

    [SerializeField] private float healCooldown = 5f;

    [SerializeField] private bool isCoop;

    [SerializeField] private TextMeshProUGUI healthText;

    [SerializeField] private UpgradeItem upgradeItem;

    public PlayerState playerState = PlayerState.alive;

        
    private void Awake()
    {
        upgradeItem.amount = maxHealth;        
    }
    private void Update()
    {

        maxHealth = upgradeItem.amount;

        if (currentHealth <= 0)
        {
            Die();
        }

        if (currentHealth < maxHealth && playerState == PlayerState.alive)
        {
            //if the cooldown is not at 0, then decrease the cooldown
            if (healCooldown > 0)
                healCooldown -= Time.deltaTime;

            //if the cooldown is at 0, then heal the player
            else
            {
                Heal(1 * Time.deltaTime);
            }
        }

        if (playerState == PlayerState.dead && !isCoop)
        {
            SceneManager.LoadScene(3);
        }

        else if (playerState == PlayerState.dead && isCoop)
        {
            gameObject.GetComponent<Collider>().enabled = false;

            foreach (Renderer r in GetComponentsInChildren<Renderer>())
            {
                r.enabled = false;
            }
        }

        UpdateHealthUI(Color.red, Color.green);
    }

    protected override void UpdateHealthUI(Color frontColour, Color backColour)
    {
        base.UpdateHealthUI(frontColour, backColour);

        healthText.text = $"{currentHealth}/{maxHealth}";
    }

    public override void Heal(float healAmount)
    {
        base.Heal(healAmount);

        //check if the player is at max health, if so, reset the cooldown
        if (currentHealth >= maxHealth)
        {
            healCooldown = 10;
        }
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

       // ScoreSystem.Instance.SaveData();

        playerState = PlayerState.dead;
    }
}
