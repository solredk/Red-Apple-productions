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
        UpdateHealthUI(Color.green);

        if (currentHealth <= 0 && !isDead)
        {
            Die();
        }
    }

    public void TakeDamage(float damage, int playerIndex)
    {
        base.TakeDamage(damage);
        UpdateHealthUI(Color.green);

        lastDamagedByPlayer = playerIndex;
    }

    public override void Heal(float healAmount)
    {
        //the base heal function
        base.Heal(healAmount);
        //update the health UI
        UpdateHealthUI(Color.green);
    }

    public override void Die()
    {
        base.Die();
        Debug.Log(ScoreSystem.Instance);
        ScoreSystem.Instance.AddScore(score, lastDamagedByPlayer);
        Destroy(gameObject);
        //isDead = true;
        //StartCoroutine(DieTimer());
    }
    
    IEnumerator DieTimer()
    {
        GetComponent<Collider>().enabled = false;
        GetComponent<MeshRenderer>().enabled = false;
        canvas.SetActive(false);
        yield return new WaitForSeconds(5f);
        GetComponent<Collider>().enabled = true;
        GetComponent<MeshRenderer>().enabled = true;
        canvas.SetActive(true);
        Heal(maxHealth);
        isDead = false;
    }
}