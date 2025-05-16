using TMPro;
using UnityEngine;

public class TomatoProjectile : MonoBehaviour
{
    [SerializeField] private  GameObject damagePopUp;
    [SerializeField] private GameObject blood;
    public int playerIndex = 0;

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

            EnemyHealth enemyHealth = collision.gameObject.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                // Maak de damage popup
                GameObject text = Instantiate(damagePopUp, transform.position, Quaternion.identity);
                text.GetComponent<TextMeshPro>().text = DamageOutput.ToString();

                // Breng schade aan de vijand
                enemyHealth.TakeDamage(DamageOutput, playerIndex);

                // Spawn de blood particles een beetje boven de vijand
                Vector3 bloodSpawnPosition = transform.position + Vector3.up * 0.5f; // Verhoog met 0.5f boven de vijand
                Instantiate(blood, bloodSpawnPosition, Quaternion.identity);
            }
        }
        Destroy(gameObject);
    }
}
