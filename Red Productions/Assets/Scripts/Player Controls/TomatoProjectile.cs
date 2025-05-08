using TMPro;
using UnityEngine;

public class TomatoProjectile : MonoBehaviour
{
    [SerializeField] private  GameObject damagePopUp;
    [SerializeField] private GameObject blood;

    public int DamageOutput;
    private void Update()
    {
        // Move the tomato forward
        transform.Translate(Vector3.forward * Time.deltaTime * 10f);
        // Destroy the tomato after 5 seconds
        Destroy(gameObject, 5f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
 
            HealthSystem playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(10f); 
            }
        }
        else if (collision.gameObject.CompareTag("Enemy"))
        {

            HealthSystem enemyHealth = collision.gameObject.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                GameObject text = Instantiate(damagePopUp, transform.position, Quaternion.identity);
                text.GetComponent<TextMeshPro>().text = DamageOutput.ToString();
                enemyHealth.TakeDamage(DamageOutput);
                Instantiate(blood, transform.position, Quaternion.identity);
            }
        }
        Destroy(gameObject);
    }
}
