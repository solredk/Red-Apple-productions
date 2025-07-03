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

    [Header("Food Type Icons")]
    [SerializeField] private Sprite burgerIcon;
    [SerializeField] private Sprite friesIcon;
    [SerializeField] private Sprite chickenNuggetsIcon;
    [SerializeField] private Sprite milkShakesIcon;

    [Header("Order State Icons")]
    [SerializeField] private Sprite happyStateIcon;
    [SerializeField] private Sprite neutralStateIcon;
    [SerializeField] private Sprite frustratedStateIcon;
    [SerializeField] private Sprite angryStateIcon;

    [Header("Timing Settings")]
    [SerializeField] private float happyThreshold = 0.75f;
    [SerializeField] private float neutralThreshold = 0.50f;
    [SerializeField] private float frustratedThreshold = 0.25f;
    [SerializeField] private float baseOrderTime = 120f;
    [SerializeField] private float additionalTimePerItem = 50f;
    [SerializeField] private float maxOrderTime = 300f;

    [Header("Point Values")]
    [SerializeField] private int happyDeliveryPoints = 100;
    [SerializeField] private int neutralDeliveryPoints = 75;
    [SerializeField] private int frustratedDeliveryPoints = 50;
    [SerializeField] private int angryDeliveryPoints = 25;

    [Header("UI References")]
    [SerializeField] private Transform orderUIContainer;
    [SerializeField] private GameObject orderUIPrefab;

    private List<DeliveryOrder> activeOrders = new List<DeliveryOrder>();
    private Dictionary<DeliveryOrder, OrderUIElements> orderUIElements = new Dictionary<DeliveryOrder, OrderUIElements>();
    private Coroutine orderExpiryCoroutine;

    // Dictionaries for food type and order state sprites
    private Dictionary<Ingredient.IngredientType, Sprite> foodTypeSprites = new Dictionary<Ingredient.IngredientType, Sprite>();
    private Dictionary<DeliveryOrder.OrderState, Sprite> orderStateSprites = new Dictionary<DeliveryOrder.OrderState, Sprite>();

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

    // Helper class to cache UI element references
    private class OrderUIElements
    {
        public GameObject uiElement;
        public Image foodIcon;
        public Image stateIcon;
        public TextMeshProUGUI timerText;
        public TextMeshProUGUI qtyText;
    }

    private void Awake()
    {
        // Initialize sprite dictionaries
        foodTypeSprites.Add(Ingredient.IngredientType.burger, burgerIcon);
        foodTypeSprites.Add(Ingredient.IngredientType.fries, friesIcon);
        foodTypeSprites.Add(Ingredient.IngredientType.chickenNuggets, chickenNuggetsIcon);
        foodTypeSprites.Add(Ingredient.IngredientType.milkShakes, milkShakesIcon);

        orderStateSprites.Add(DeliveryOrder.OrderState.Happy, happyStateIcon);
        orderStateSprites.Add(DeliveryOrder.OrderState.Neutral, neutralStateIcon);
        orderStateSprites.Add(DeliveryOrder.OrderState.Frustrated, frustratedStateIcon);
        orderStateSprites.Add(DeliveryOrder.OrderState.Angry, angryStateIcon);
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

    protected override void Interact(GameObject playerGameObject)
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
            if (orderUIElements.TryGetValue(order, out OrderUIElements uiElements))
            {
                Destroy(uiElements.uiElement);
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

        GameObject randomPrefab = deliveryFoodPrefabs[UnityEngine.Random.Range(0, deliveryFoodPrefabs.Length)];
        FoodType typeId = randomPrefab.GetComponent<FoodType>();

        if (typeId == null)
        {
            Debug.LogError($"<color=red>[DeliverySystem]</color> Prefab {randomPrefab.name} is missing FoodType component!");
            return;
        }

        int quantity = UnityEngine.Random.Range(1, 4);
        float orderTime = baseOrderTime + (additionalTimePerItem * (quantity - 1));
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
    }

    private void CreateOrderUI(DeliveryOrder order)
    {
        if (orderUIPrefab == null || orderUIContainer == null)
            return;

        GameObject uiElement = Instantiate(orderUIPrefab, orderUIContainer);

        // Find UI elements and cache them
        OrderUIElements uiElements = new OrderUIElements
        {
            uiElement = uiElement,
            foodIcon = uiElement.transform.Find("FoodIcon")?.GetComponent<Image>(),
            stateIcon = uiElement.transform.Find("StateIcon")?.GetComponent<Image>(),
            timerText = uiElement.transform.Find("TimerText")?.GetComponent<TextMeshProUGUI>(),
            qtyText = uiElement.transform.Find("QuantityText")?.GetComponent<TextMeshProUGUI>()
        };

        orderUIElements[order] = uiElements;

        // Set initial values
        if (uiElements.foodIcon != null)
        {
            if (foodTypeSprites.ContainsKey(order.foodType))
            {
                uiElements.foodIcon.sprite = foodTypeSprites[order.foodType];
            }
            else
            {
                Debug.LogWarning($"No sprite found for food type: {order.foodType}");
            }
        }

        if (uiElements.qtyText != null)
        {
            uiElements.qtyText.text = $"x{order.quantity}";
        }

        if (uiElements.timerText != null)
        {
            uiElements.timerText.text = FormatTime(order.GetRemainingTime());
        }

        // Set initial state icon (optional)
        if (uiElements.stateIcon != null)
        {
            UpdateOrderStateIcon(order, uiElements.stateIcon);
        }
    }

    private void UpdateOrderUI()
    {
        foreach (DeliveryOrder order in activeOrders)
        {
            if (orderUIElements.TryGetValue(order, out OrderUIElements uiElements))
            {
                if (uiElements == null || uiElements.uiElement == null)
                {
                    // UI element was destroyed, remove it from the dictionary
                    orderUIElements.Remove(order);
                    continue;
                }

                if (uiElements.timerText != null)
                {
                    uiElements.timerText.text = FormatTime(order.GetRemainingTime());
                }

                if (uiElements.stateIcon != null)
                {
                    UpdateOrderStateIcon(order, uiElements.stateIcon);
                }
            }
        }
    }

    private void UpdateOrderStateIcon(DeliveryOrder order, Image stateIcon)
    {
        DeliveryOrder.OrderState state = order.GetCurrentState(happyThreshold, neutralThreshold, frustratedThreshold);

        if (orderStateSprites.ContainsKey(state))
        {
            stateIcon.sprite = orderStateSprites[state];
        }
        else
        {
            Debug.LogWarning($"No sprite found for order state: {state}");
            stateIcon.sprite = null; // Clear the sprite if no match is found
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

            for (int i = activeOrders.Count - 1; i >= 0; i--)
            {
                DeliveryOrder order = activeOrders[i];
                if (currentTime >= order.expirationTime)
                {
                    Debug.Log($"Order expired: {order.quantity}x {order.foodType}");

                    if (orderUIElements.TryGetValue(order, out OrderUIElements uiElements))
                    {
                        Destroy(uiElements.uiElement);
                        orderUIElements.Remove(order);
                    }

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
        Gizmos.color = new Color(1f, 0.5f, 0f, 0.3f);
        Gizmos.DrawSphere(transform.position, deliveryDetectionRadius);

        Gizmos.color = new Color(1f, 0.5f, 0f, 1f);
        Gizmos.DrawWireSphere(transform.position, deliveryDetectionRadius);
    }

    private void OnDestroy()
    {
        if (orderExpiryCoroutine != null)
            StopCoroutine(orderExpiryCoroutine);
    }
}