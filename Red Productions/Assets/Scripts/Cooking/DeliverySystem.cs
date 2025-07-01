using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DeliverySystem : Interactable
{
    [Header("Delivery Settings")]
    [SerializeField] private GameObject[] deliveryFoodPrefabs;
    [SerializeField] private int minActiveOrders = 1;
    [SerializeField] private int maxActiveOrders = 3;
    [SerializeField] private float orderGenerationInterval = 20f;
    [SerializeField] private float deliveryDetectionRadius = 3f;
    [SerializeField] private LayerMask foodLayer;
    [SerializeField] private GameObject deliveryEffectPrefab;
    [SerializeField] private GameObject interactionIndicator;

    [Header("Timing Settings")]
    // Higher values = more time remaining (opposite of completion percentage)
    [SerializeField] private float happyThreshold = 0.75f; // Happy if 75%+ time remains
    [SerializeField] private float neutralThreshold = 0.50f; // Neutral if 50-75% time remains
    [SerializeField] private float frustratedThreshold = 0.25f; // Frustrated if 25-50% time remains
    [SerializeField] private float baseOrderTime = 120f; // Base time (2 minutes)
    [SerializeField] private float additionalTimePerItem = 50f; // Add 50 seconds per extra item
    [SerializeField] private float maxOrderTime = 300f; // Maximum order time (5 minutes)

    [Header("Point Values")]
    [SerializeField] private int happyDeliveryPoints = 100;
    [SerializeField] private int neutralDeliveryPoints = 75;
    [SerializeField] private int frustratedDeliveryPoints = 50;
    [SerializeField] private int angryDeliveryPoints = 25;

    [Header("UI References")]
    [SerializeField] private Transform orderUIContainer;
    [SerializeField] private GameObject orderUIPrefab;

    private List<DeliveryOrder> activeOrders = new List<DeliveryOrder>();
    private Dictionary<DeliveryOrder, GameObject> orderUIElements = new Dictionary<DeliveryOrder, GameObject>();
    private Coroutine orderExpiryCoroutine;

    [Serializable]
    public class DeliveryOrder
    {
        public Ingredient.IngredientType foodType;
        public int quantity;
        public float creationTime;
        public float expirationTime;

        public enum OrderState
        {
            Happy,
            Neutral,
            Frustrated,
            Angry
        }

        public OrderState GetCurrentState(float happyThreshold, float neutralThreshold, float frustratedThreshold)
        {
            float timeRemainingPercentage = GetTimeRemainingPercentage();

            if (timeRemainingPercentage >= happyThreshold)
                return OrderState.Happy;
            else if (timeRemainingPercentage >= neutralThreshold)
                return OrderState.Neutral;
            else if (timeRemainingPercentage >= frustratedThreshold)
                return OrderState.Frustrated;
            else
                return OrderState.Angry;
        }

        public float GetRemainingTime()
        {
            return Mathf.Max(0, expirationTime - Time.time);
        }

        public float GetTotalTime()
        {
            return expirationTime - creationTime;
        }

        public float GetTimeRemainingPercentage()
        {
            float totalTime = GetTotalTime();
            float remainingTime = GetRemainingTime();
            return remainingTime / totalTime;
        }
    }

    private void Start()
    {
        if (interactionIndicator != null)
            interactionIndicator.SetActive(false);

        if (string.IsNullOrEmpty(promptMessage))
            promptMessage = "Press Q to deliver food";

        StartCoroutine(GenerateOrdersRoutine());
        orderExpiryCoroutine = StartCoroutine(CheckOrderExpiryRoutine());
    }

    private void Update()
    {
        UpdateOrderUI();
    }

    protected override void Interact()
    {
        Debug.Log("Delivery System interacted with - checking for food items");

        Collider[] colliders = Physics.OverlapSphere(transform.position, deliveryDetectionRadius, foodLayer);
        List<DeliveryOrder> completedOrders = new List<DeliveryOrder>();
        int totalPointsEarned = 0;

        foreach (DeliveryOrder order in activeOrders)
        {
            int matchingFoodCount = 0;
            List<GameObject> matchingFoodItems = new List<GameObject>();

            foreach (Collider foodCollider in colliders)
            {
                Food food = foodCollider.GetComponent<Food>();
                if (food == null) continue;

                FoodType typeId = foodCollider.GetComponent<FoodType>();
                if (typeId != null && typeId.foodType == order.foodType)
                {
                    matchingFoodItems.Add(foodCollider.gameObject);
                    matchingFoodCount++;

                    if (matchingFoodCount >= order.quantity)
                        break;
                }
            }

            if (matchingFoodCount >= order.quantity)
            {
                int pointsEarned = CalculatePointsForOrder(order) * order.quantity;
                totalPointsEarned += pointsEarned;

                foreach (GameObject foodItem in matchingFoodItems)
                {
                    if (deliveryEffectPrefab != null)
                        Instantiate(deliveryEffectPrefab, foodItem.transform.position, Quaternion.identity);
                    Destroy(foodItem);
                }

                completedOrders.Add(order);
                Debug.Log($"Order completed! {order.quantity}x {order.foodType} - {pointsEarned} points");
            }
        }

        foreach (DeliveryOrder order in completedOrders)
        {
            if (orderUIElements.TryGetValue(order, out GameObject uiElement))
            {
                Destroy(uiElement);
                orderUIElements.Remove(order);
            }
            activeOrders.Remove(order);
        }

        if (totalPointsEarned > 0 && ScoreSystem.Instance != null)
        {
            ScoreSystem.Instance.AddScore(totalPointsEarned);
            Debug.Log($"Delivered food for {totalPointsEarned} total points!");
        }
    }

    private int CalculatePointsForOrder(DeliveryOrder order)
    {
        DeliveryOrder.OrderState state = order.GetCurrentState(happyThreshold, neutralThreshold, frustratedThreshold);

        if (state == DeliveryOrder.OrderState.Happy)
            return happyDeliveryPoints;
        else if (state == DeliveryOrder.OrderState.Neutral)
            return neutralDeliveryPoints;
        else if (state == DeliveryOrder.OrderState.Frustrated)
            return frustratedDeliveryPoints;
        else
            return angryDeliveryPoints;
    }

    private IEnumerator GenerateOrdersRoutine()
    {
        while (true)
        {
            if (activeOrders.Count < maxActiveOrders)
            {
                int ordersToGenerate = UnityEngine.Random.Range(1, maxActiveOrders - activeOrders.Count + 1);

                for (int i = 0; i < ordersToGenerate; i++)
                {
                    GenerateRandomOrder();
                }
            }

            yield return new WaitForSeconds(orderGenerationInterval);
        }
    }

    private void GenerateRandomOrder()
    {
        if (deliveryFoodPrefabs == null || deliveryFoodPrefabs.Length == 0)
        {
            Debug.LogError("<color=red>[DeliverySystem]</color> No delivery food prefabs assigned!");
            return;
        }

        // Log available prefabs
        Debug.Log($"<color=blue>[DeliverySystem]</color> Generating random order from {deliveryFoodPrefabs.Length} available prefabs");

        GameObject randomPrefab = deliveryFoodPrefabs[UnityEngine.Random.Range(0, deliveryFoodPrefabs.Length)];
        Debug.Log($"<color=blue>[DeliverySystem]</color> Selected prefab: {randomPrefab.name}");

        FoodType typeId = randomPrefab.GetComponent<FoodType>();
        if (typeId == null)
        {
            Debug.LogError($"<color=red>[DeliverySystem]</color> Prefab {randomPrefab.name} is missing FoodType component!");
            return;
        }

        int quantity = UnityEngine.Random.Range(1, 4); // Random amount between 1-3
        float orderTime = baseOrderTime + (additionalTimePerItem * (quantity - 1));
        // Cap the order time to maximum
        orderTime = Mathf.Min(orderTime, maxOrderTime);

        DeliveryOrder newOrder = new DeliveryOrder
        {
            foodType = typeId.foodType,
            quantity = quantity,
            creationTime = Time.time,
            expirationTime = Time.time + orderTime
        };

        activeOrders.Add(newOrder);
        CreateOrderUI(newOrder);

        // Enhanced detailed logging
        Debug.Log($"<color=green>[DeliverySystem]</color> NEW ORDER GENERATED:");
        Debug.Log($"<color=green>[DeliverySystem]</color> - Food Type: {newOrder.foodType}");
        Debug.Log($"<color=green>[DeliverySystem]</color> - Quantity: {newOrder.quantity}");
        Debug.Log($"<color=green>[DeliverySystem]</color> - Total Time: {orderTime} seconds");
        Debug.Log($"<color=green>[DeliverySystem]</color> - Creation Time: {newOrder.creationTime}");
        Debug.Log($"<color=green>[DeliverySystem]</color> - Expiration Time: {newOrder.expirationTime}");
        Debug.Log($"<color=green>[DeliverySystem]</color> - Current Order Count: {activeOrders.Count}/{maxActiveOrders}");
    }
    private void CreateOrderUI(DeliveryOrder order)
    {
        if (orderUIPrefab != null && orderUIContainer != null)
        {
            GameObject uiElement = Instantiate(orderUIPrefab, orderUIContainer);

            TextMeshProUGUI[] texts = uiElement.GetComponentsInChildren<TextMeshProUGUI>();
            foreach (TextMeshProUGUI text in texts)
            {
                if (text.name.Contains("Type"))
                    text.text = order.foodType.ToString();
                else if (text.name.Contains("Quantity"))
                    text.text = $"x{order.quantity}";
                else if (text.name.Contains("Timer"))
                    text.text = FormatTime(order.GetRemainingTime());
            }

            orderUIElements[order] = uiElement;
        }
    }

    private void UpdateOrderUI()
    {
        foreach (KeyValuePair<DeliveryOrder, GameObject> entry in orderUIElements)
        {
            DeliveryOrder order = entry.Key;
            GameObject uiElement = entry.Value;

            if (uiElement == null) continue;

            TextMeshProUGUI timerText = uiElement.GetComponentInChildren<TextMeshProUGUI>(true);
            if (timerText != null && timerText.name.Contains("Timer"))
                timerText.text = FormatTime(order.GetRemainingTime());

            Image stateIndicator = uiElement.GetComponentInChildren<Image>(true);
            if (stateIndicator != null && stateIndicator.name.Contains("StateIndicator"))
            {
                DeliveryOrder.OrderState state = order.GetCurrentState(happyThreshold, neutralThreshold, frustratedThreshold);

                if (state == DeliveryOrder.OrderState.Happy)
                    stateIndicator.color = Color.green;
                else if (state == DeliveryOrder.OrderState.Neutral)
                    stateIndicator.color = Color.yellow;
                else if (state == DeliveryOrder.OrderState.Frustrated)
                    stateIndicator.color = new Color(1.0f, 0.5f, 0.0f); // Orange
                else
                    stateIndicator.color = Color.red;
            }
        }
    }

    private string FormatTime(float timeInSeconds)
    {
        int minutes = Mathf.FloorToInt(timeInSeconds / 60);
        int seconds = Mathf.FloorToInt(timeInSeconds % 60);
        return $"{minutes:00}:{seconds:00}";
    }

    private IEnumerator CheckOrderExpiryRoutine()
    {
        while (true)
        {
            float currentTime = Time.time;

            // Use a for loop with backward iteration to safely remove elements
            for (int i = activeOrders.Count - 1; i >= 0; i--)
            {
                DeliveryOrder order = activeOrders[i];
                if (currentTime >= order.expirationTime)
                {
                    Debug.Log($"Order expired: {order.quantity}x {order.foodType}");

                    // Remove UI element
                    if (orderUIElements.TryGetValue(order, out GameObject uiElement))
                    {
                        Destroy(uiElement);
                        orderUIElements.Remove(order);
                    }

                    // Safely remove the order
                    activeOrders.RemoveAt(i);
                }
            }

            yield return new WaitForSeconds(1f);
        }
    }

    public void ShowInteractionIndicator(bool show)
    {
        if (interactionIndicator != null)
            interactionIndicator.SetActive(show);
    }

    private void OnDrawGizmosSelected()
    {
        // Draw detection radius
        Gizmos.color = new Color(1f, 0.5f, 0f, 0.3f); // Orange
        Gizmos.DrawSphere(transform.position, deliveryDetectionRadius);

        // Draw wire sphere for visibility
        Gizmos.color = new Color(1f, 0.5f, 0f, 1f);
        Gizmos.DrawWireSphere(transform.position, deliveryDetectionRadius);
    }

    private void OnDestroy()
    {
        if (orderExpiryCoroutine != null)
            StopCoroutine(orderExpiryCoroutine);
    }
}