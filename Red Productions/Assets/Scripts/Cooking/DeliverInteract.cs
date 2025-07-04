using UnityEngine;

public class DeliverInteract : Interactable
{
    [Header("Delivery Settings")]
    [SerializeField] private float deliveryRadius = 3f;
    [SerializeField] private LayerMask foodLayer;
    [SerializeField] private GameObject interactionSprite;

    private void Start()
    {
        // Set default prompt message
        if (string.IsNullOrEmpty(promptMessage))
            promptMessage = "Press Q to deliver food";

        // Hide interaction indicator by default
        if (interactionSprite != null)
            interactionSprite.SetActive(false);
    }

    public void ShowInteractionIndicator(bool show)
    {
        if (interactionSprite != null)
            interactionSprite.SetActive(show);
    }

    protected override void Interact(GameObject playerGameObject)
    {
        Debug.Log("<color=blue>[DeliverInteract]</color> Checking for food to deliver...");

        // Find all Food objects within the delivery radius
        Collider[] colliders = Physics.OverlapSphere(transform.position, deliveryRadius, foodLayer);
        int deliveredCount = 0;

        foreach (Collider collider in colliders)
        {
            Food food = collider.GetComponent<Food>();
            if (food != null)
            {
                // Award 100 points for each food
                if (ScoreSystem.Instance != null)
                {
                    ScoreSystem.Instance.AddScore(100);
                }

                // Destroy the delivered food
                Destroy(collider.gameObject);
                deliveredCount++;
            }
        }

        if (deliveredCount > 0)
        {
            Debug.Log($"<color=green>[DeliverInteract]</color> Delivered {deliveredCount} food items! +{deliveredCount * 100} points");
        }
        else
        {
            Debug.Log("<color=orange>[DeliverInteract]</color> No food found to deliver!");
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Visualize delivery radius
        Gizmos.color = new Color(1f, 0.5f, 0f, 0.3f); // Orange semi-transparent
        Gizmos.DrawSphere(transform.position, deliveryRadius);

        // Draw wire sphere for better visibility
        Gizmos.color = new Color(1f, 0.5f, 0f, 1f);
        Gizmos.DrawWireSphere(transform.position, deliveryRadius);
    }
}