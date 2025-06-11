using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : HealthSystem
{
    [SerializeField] private GameObject damagePopUp;

    [SerializeField] private GameObject canvas;

    [SerializeField] private int lastDamagedByPlayer;

    [SerializeField] private Enemybehavior enemyBehavior;

    public int score;

    private bool isDead = false;

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

        //add score to the player that killed this enemy
        ScoreSystem.Instance.AddScore(score);

        //destroy the gameobject
        Destroy(gameObject);
    }
}