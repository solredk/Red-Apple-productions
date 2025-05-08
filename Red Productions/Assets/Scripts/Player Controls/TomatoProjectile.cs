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
        Destroy(gameObject);
    }
}
