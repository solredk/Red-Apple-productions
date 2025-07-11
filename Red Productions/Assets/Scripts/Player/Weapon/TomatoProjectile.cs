using UnityEngine;
using TMPro;

public class TomatoProjectile : MonoBehaviour
{
    [SerializeField] private GameObject tomatoSplatter;
    [SerializeField] private GameObject damagePopUp;
    [SerializeField] private float tomatoSpeed = 17.0f;
    public int DamageOutput;
    public int playerId = 0; // Add player ID for tracking who shot the tomato

    private void Update()
    {
        // Move the tomato forward (fixed multiplication operator)
        transform.Translate(tomatoSpeed * Time.deltaTime * Vector3.forward);
        // Destroy the tomato after 5 seconds
        Destroy(gameObject, 5f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Instantiating the splatter effect above the hit point
        Vector3 spawnpoint = transform.position + Vector3.up * 0.5f;
        // Checking if the enemy is hit
        if (collision.gameObject.CompareTag("Enemy"))
        {
            // Getting the enemy health script
            EnemyHealth healthSystem = collision.gameObject.GetComponent<EnemyHealth>();
            // Checking if the health system exists (fixed null check)
            if (healthSystem != null)
            {
                // Make the enemy take damage
                healthSystem.TakeDamage(DamageOutput, playerId);

                // Create damage popup at collision point
                if (damagePopUp != null)
                {
                    GameObject popup = Instantiate(damagePopUp, spawnpoint, Quaternion.identity);
                    // Set the damage text
                    TextMeshProUGUI damageText = popup.GetComponentInChildren<TextMeshProUGUI>();
                    if (damageText != null)
                    {
                        damageText.text = DamageOutput.ToString();
                    }
                    Destroy(popup, 2f);
                }
            }
        }
        // Instantiate splatter effect
        Instantiate(tomatoSplatter, spawnpoint, Quaternion.identity);
        // Destroy the tomato projectile on collision
        Destroy(gameObject);
    }
}