using UnityEngine;

public class DeliveryPoint : MonoBehaviour
{
    [Tooltip("Amount of points given for each successful food delivery")]
    [SerializeField] private int pointsPerDelivery = 100;

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object has a Food component
        Food food = other.GetComponent<Food>();
        if (food != null)
        {
            // Add points to the score system
            if (ScoreSystem.Instance != null)
            {
                ScoreSystem.Instance.AddScore(pointsPerDelivery);
                Debug.Log($"Food delivered! +{pointsPerDelivery} points");
            }

            // Destroy the food object
            Destroy(food.gameObject);
        }
    }
}