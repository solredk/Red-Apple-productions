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
    [Header("Player Animator Component")]
    [SerializeField] private Animator Animator;

    [Header("Health Text Component")]
    [SerializeField] private TextMeshProUGUI healthText;

    [Header("Player Health Upgrade Component")]
    [SerializeField] private UpgradeItem upgradeItem;

    [SerializeField] private AudioSource[] damageSound;

    [Header("Playerstate Component")]
    public PlayerState playerState;

    private float healCooldown = 5f;


    private void Awake()
    {
        upgradeItem.amount = maxHealth;        
    }

    private void Update()
    {

        maxHealth = upgradeItem.amount;

        if (currentHealth <= 0 && playerState == PlayerState.alive)
        {
            // ScoreSystem.Instance.SaveData();
            playerState = PlayerState.dead;
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
        damageSound[Random.Range(0, damageSound.Length)].Play();
        //reset the cooldown
        healCooldown = 10f;
    }

    public override void Die()
    {
        base.Die();
        Animator.SetBool("Died", true);
        if (playerState == PlayerState.dead && GameManager.Instance.gameMode == GameMode.SinglePlayer)
        {
            ScoreSystem.Instance.SaveData();
            GameManager.Instance.loadscene.LoadScene();
        }

        else if (playerState == PlayerState.dead && GameManager.Instance.gameMode == GameMode.CoOp)
        {
            Debug.Log("Player died, disabling collider and playing death animation");
            gameObject.GetComponent<Collider>().enabled = false;
            GameManager.Instance.PlayerDied();
        }
        // ScoreSystem.Instance.SaveData();

    }

    public void ResetPlayerState()
    {
        GameManager.Instance.PlayerSpawned();
        playerState = PlayerState.alive;
        currentHealth = maxHealth;
        gameObject.GetComponent<Collider>().enabled = true;
        Animator.SetBool("Died", false);
    }
}
