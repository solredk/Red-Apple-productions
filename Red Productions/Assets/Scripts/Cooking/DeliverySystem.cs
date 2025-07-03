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
    [SerializeField] private Transform foodIconContainer;
    [SerializeField] private Image stateIcon;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private GameObject orderUIElement;

    [Header("Food Type Sprites")]
    [SerializeField] private Sprite burgerIcon;
    [SerializeField] private Sprite friesIcon;
    [SerializeField] private Sprite chickenNuggetsIcon;
    [SerializeField] private Sprite milkShakesIcon;

    [Header("Order State Sprites")]
    [SerializeField] private Sprite happyStateIcon;
    [SerializeField] private Sprite neutralStateIcon;
    [SerializeField] private Sprite frustratedStateIcon;
    [SerializeField] private Sprite angryStateIcon;

    [Header("Delivery Text UI")]
    [SerializeField] private GameObject deliveryTextUIPrefab;
    [SerializeField] private Transform deliveryTextUIParent;

    private Dictionary<Ingredient.IngredientType, Sprite> foodTypeSprites;
    private Dictionary<DeliveryOrder.OrderState, Sprite> orderStateSprites;
    private List<DeliveryOrder> activeOrders = new List<DeliveryOrder>();
    private DeliveryOrder activeOrder;
    private Dictionary<DeliveryOrder, GameObject> orderUIElements = new Dictionary<DeliveryOrder, GameObject>();
    private Coroutine orderExpiryCoroutine;
    private GameObject currentDeliveryTextUI;

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
            return Mathf.Max(0, expirationTime - Time.unscaledTime);
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

    private void Awake()
    {
        // Initialize the dictionaries with the serialized sprites
        foodTypeSprites = new Dictionary<Ingredient.IngredientType, Sprite>
        {
            { Ingredient.IngredientType.burger, burgerIcon },
            { Ingredient.IngredientType.fries, friesIcon },
            { Ingredient.IngredientType.chickenNuggets, chickenNuggetsIcon },
            { Ingredient.IngredientType.milkShakes, milkShakesIcon }
        };

        orderStateSprites = new Dictionary<DeliveryOrder.OrderState, Sprite>
        {
            { DeliveryOrder.OrderState.Happy, happyStateIcon },
            { DeliveryOrder.OrderState.Neutral, neutralStateIcon },
            { DeliveryOrder.OrderState.Frustrated, frustratedStateIcon },
            { DeliveryOrder.OrderState.Angry, angryStateIcon }
        };
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

    protected override void Interact(GameObject gameObject)
    {
        Debug.Log("Delivery System interacted with - checking for food items");

        Collider[] colliders = Physics.OverlapSphere(transform.position, deliveryDetectionRadius, foodLayer);
        List<DeliveryOrder> completedOrders = new List<DeliveryOrder>();
        int totalPointsEarned = 0;

        if (activeOrder == null)
        {
            Debug.Log("No active order to fulfill.");
            return;
        }

        int matchingFoodCount = 0;
        List<GameObject> matchingFoodItems = new List<GameObject>();

        foreach (Collider foodCollider in colliders)
        {
            Food food = foodCollider.GetComponent<Food>();
            if (food == null) continue;

            FoodType typeId = foodCollider.GetComponent<FoodType>();
            if (typeId != null && typeId.foodType == activeOrder.foodType)
            {
                matchingFoodItems.Add(foodCollider.gameObject);
                matchingFoodCount++;

                if (matchingFoodCount >= activeOrder.quantity)
                    break;
            }
        }

        if (matchingFoodCount >= activeOrder.quantity)
        {
            int pointsEarned = CalculatePointsForOrder(activeOrder) * activeOrder.quantity;
            totalPointsEarned += pointsEarned;

            foreach (GameObject foodItem in matchingFoodItems)
            {
                if (deliveryEffectPrefab != null)
                    Instantiate(deliveryEffectPrefab, foodItem.transform.position, Quaternion.identity);
                Destroy(foodItem);
            }

            completedOrders.Add(activeOrder);
            Debug.Log($"Order completed! {activeOrder.quantity}x {activeOrder.foodType} - {pointsEarned} points");

            // Show delivery text UI that persists until new order
            ShowDeliveryTextUI(pointsEarned);

            // Stop the timer by nulling the active order and hiding the order UI
            activeOrder = null;
            orderUIElement.SetActive(false);
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

    private IEnumerator GenerateOrdersRoutine()
    {
        while (true)
        {
            if (activeOrder == null)
            {
                GenerateRandomOrder();
            }

            yield return new WaitForSeconds(orderGenerationInterval);
        }
    }

    private void GenerateRandomOrder()
    {
        // Destroy any existing delivery text UI when a new order starts
        if (currentDeliveryTextUI != null)
        {
            Destroy(currentDeliveryTextUI);
            currentDeliveryTextUI = null;
        }

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

        activeOrder = new DeliveryOrder
        {
            foodType = typeId.foodType,
            quantity = quantity,
            creationTime = Time.unscaledTime,
            expirationTime = Time.unscaledTime + orderTime
        };

        UpdateUIForOrder(activeOrder);

        // Enhanced detailed logging
        Debug.Log($"<color=green>[DeliverySystem]</color> NEW ORDER GENERATED:");
        Debug.Log($"<color=green>[DeliverySystem]</color> - Food Type: {activeOrder.foodType}");
        Debug.Log($"<color=green>[DeliverySystem]</color> - Quantity: {activeOrder.quantity}");
        Debug.Log($"<color=green>[DeliverySystem]</color> - Order Time: {orderTime}");
    }

    private void UpdateUIForOrder(DeliveryOrder order)
    {
        // Clear existing food icons
        foreach (Transform child in foodIconContainer)
        {
            Destroy(child.gameObject);
        }

        // Instantiate food icons based on quantity
        for (int i = 0; i < order.quantity; i++)
        {
            GameObject icon = new GameObject("FoodIcon");
            Image image = icon.AddComponent<Image>();

            if (foodTypeSprites.TryGetValue(order.foodType, out Sprite sprite))
            {
                image.sprite = sprite;
            }
            else
            {
                Debug.LogWarning($"No sprite found for food type: {order.foodType}");
            }

            icon.transform.SetParent(foodIconContainer, false);
        }

        if (timerText != null)
        {
            timerText.text = FormatTime(order.GetRemainingTime());
        }

        if (stateIcon != null)
        {
            UpdateOrderStateIcon(order, stateIcon);
        }

        orderUIElement.SetActive(true);
    }

    private void UpdateOrderUI()
    {
        if (activeOrder != null)
        {
            if (timerText != null)
            {
                timerText.text = FormatTime(activeOrder.GetRemainingTime());
            }

            if (stateIcon != null)
            {
                UpdateOrderStateIcon(activeOrder, stateIcon);
            }
        }
    }

    private void UpdateOrderStateIcon(DeliveryOrder order, Image stateIcon)
    {
        DeliveryOrder.OrderState state = order.GetCurrentState(happyThreshold, neutralThreshold, frustratedThreshold);

        if (orderStateSprites.TryGetValue(state, out Sprite sprite))
        {
            stateIcon.sprite = sprite;
        }
        else
        {
            Debug.LogWarning($"No sprite found for order state: {state}");
            stateIcon.sprite = null;
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
            if (activeOrder != null)
            {
                if (Time.unscaledTime >= activeOrder.expirationTime)
                {
                    Debug.Log($"Order expired: {activeOrder.quantity}x {activeOrder.foodType}");
                    activeOrder = null;
                    orderUIElement.SetActive(false);
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
        StopAllCoroutines();
    }

    private void ShowDeliveryTextUI(int points)
    {
        // Destroy any existing UI first
        if (currentDeliveryTextUI != null)
        {
            Destroy(currentDeliveryTextUI);
        }

        if (deliveryTextUIPrefab == null)
        {
            Debug.LogError("DeliveryTextUIPrefab is not assigned!");
            return;
        }

        // Create new UI and store a reference
        currentDeliveryTextUI = Instantiate(deliveryTextUIPrefab,
            deliveryTextUIParent != null ? deliveryTextUIParent : transform);

        // Get the TextMeshProUGUI component from the instantiated prefab
        TextMeshProUGUI tmpText = currentDeliveryTextUI.GetComponentInChildren<TextMeshProUGUI>();

        if (tmpText != null)
        {
            tmpText.text = $"Delivery Completed! +{points} points";
        }
        else
        {
            Debug.LogError("TextMeshProUGUI component not found in DeliveryTextUIPrefab!");
        }

        // This UI will persist until a new order is generated
        // or the game object is destroyed
    }

    private int CalculatePointsForOrder(DeliveryOrder order)
    {
        switch (order.GetCurrentState(happyThreshold, neutralThreshold, frustratedThreshold))
        {
            case DeliveryOrder.OrderState.Happy:
                return happyDeliveryPoints;
            case DeliveryOrder.OrderState.Neutral:
                return neutralDeliveryPoints;
            case DeliveryOrder.OrderState.Frustrated:
                return frustratedDeliveryPoints;
            case DeliveryOrder.OrderState.Angry:
                return angryDeliveryPoints;
            default:
                return 0;
        }
    }
}