using UnityEngine;

public class TomatoProjectile : MonoBehaviour
{

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
        Destroy(gameObject);
    }
}
