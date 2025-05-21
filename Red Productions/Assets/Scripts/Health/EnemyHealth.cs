using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : HealthSystem
{
    [SerializeField] private GameObject damagePopUp;

    [SerializeField] private GameObject canvas;

    [SerializeField] private int lastDamagedByPlayer;

    public int score;

    private bool isDead = false;

    private void Update()
    {
        //updating the health bar
        UpdateHealthUI(Color.green,Color.black);

        //checking if you are death and if so, call the die function
        if (currentHealth <= 0 && !isDead)
            Die();
    }

    public void TakeDamage(float damage, int playerIndex)
    {
        base.TakeDamage(damage);

        //set the lastDamagedByPlayer to the player that damaged this enemy
        lastDamagedByPlayer = playerIndex;
    }

    public override void Heal(float healAmount)
    {
        //the base heal function
        base.Heal(healAmount);
    }

    public override void Die()
    {
        base.Die();
        //add score to the player that killed this enemy
        ScoreSystem.Instance.AddScore(lastDamagedByPlayer, score);

        //destroy the gameobject
        Destroy(gameObject);
    }
}