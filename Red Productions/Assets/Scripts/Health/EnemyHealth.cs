using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : HealthSystem
{
    [Header("Enemy Health Settings")]
    [SerializeField] private Enemybehavior enemyStats;

    [Header("Collider Components")]
    [SerializeField] private Collider enemyCollider;

    [Header("Health Bar Components")]
    [SerializeField] private GameObject healthBarCanvas;

    [Header("Damage Pop Up")]
    [SerializeField] private GameObject damagePopUp;

    [Header("Animation Components")]
    [SerializeField] private Animator zombieAnimator;

    [Header("Sound Components")]
    [SerializeField] private AudioSource[] deathSounds;
    [SerializeField] private AudioSource[] hitSounds;

    public int score;

    public bool isDead = false;

    private void Awake()
    {
        maxHealth = enemyStats.maxhealth;
    }
    
    private void Update()
    {
        //updating the health bar
        UpdateHealthUI(Color.green,Color.black);

        //checking if you are death and if so, call the die function
        if (currentHealth <= 0 && !isDead)
        {
            Die();
        }
    }

    public void TakeDamage(int damage, int playerId)
    {
        base.TakeDamage(damage);
        int randomIndex = Random.Range(0, hitSounds.Length);
        hitSounds[randomIndex].Play();
    }

    public override void Die()
    {
        base.Die();
        //choose an random death sound
        int randomIndex = Random.Range(0, deathSounds.Length);

        deathSounds[randomIndex].Play();

        enemyCollider.enabled = false;

        if (!isDead)
        {
            //add score to the player that killed this enemy
            ScoreSystem.Instance.AddScore(enemyStats.reward);
            isDead = true;
        }

        zombieAnimator.SetTrigger("die");

        //destroy the gameobject
        Destroy(gameObject ,2);
    }
}