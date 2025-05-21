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
        transform.Translate(10f * Time.deltaTime * Vector3.forward);
        // Destroy the tomato after 5 seconds
        Destroy(gameObject, 5f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        //checking if the enemy is hit
        if (collision.gameObject.CompareTag("Enemy"))
        {
            //putting the enemyhealth script on an variabel to make it easier to use
            EnemyHealth enemyHealth = collision.gameObject.GetComponent<EnemyHealth>();

            //checking if the enemyhealth script is not null so nothing has gone wrong
            if (collision.gameObject != null)
            {
                // instantiating the damage pop up text above the enemy with the damage output
                GameObject text = Instantiate(damagePopUp, transform.position, Quaternion.identity);
                text.GetComponent<TextMeshPro>().text = DamageOutput.ToString();

                // make the enemy take damage
                enemyHealth.TakeDamage(DamageOutput, playerIndex);

                // instantiating the blood effect above the enemy on the place he was hit
                Vector3 bloodSpawnPosition = transform.position + Vector3.up * 0.5f;
                Instantiate(blood, bloodSpawnPosition, Quaternion.identity);
            }
        }
        Destroy(gameObject);
    }
}
