using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : HealthSystem
{
    [SerializeField] private GameObject damagePopUp;

    [SerializeField] private GameObject canvas;

    [SerializeField] private int lastDamagedByPlayer;

    [SerializeField] private Enemybehavior enemyBehavior;

    [SerializeField] private Animator zombieAnimator;

    [SerializeField] private Enemybehavior enemyStats;

    [SerializeField] private Collider enemyCollider;

    public int score;

    public bool isDead = false;

    private void Awake()
    {
        maxHealth = enemyBehavior.maxhealth;
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

    public override void Die()
    {
        base.Die();
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