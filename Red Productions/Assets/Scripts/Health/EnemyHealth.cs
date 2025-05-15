using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : HealthSystem
{
    [SerializeField] private GameObject damagePopUp;

    [SerializeField] private GameObject canvas;

    private bool isDead = false;

    private void Update()
    {
        UpdateHealthUI();

        if (currentHealth <= 0 && !isDead)
        {
            Die();
        }
    }
    
    public override void TakeDamage(float damage)
    {
        //the base take damage function
        base.TakeDamage(damage);

        //update the health UI
        UpdateHealthUI();
    }

    public override void Heal(float healAmount)
    {
        //the base heal function
        base.Heal(healAmount);
        //update the health UI
        UpdateHealthUI();
    }

    public override void Die()
    {
        base.Die();
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