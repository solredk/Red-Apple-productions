using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using System;
using System.Linq;

public class DeliverySystem : Interactable
{
    [Header("Delivery Settings")]
    [SerializeField] private GameObject[] deliveryFoodPrefabs;
    [SerializeField] private int minActiveOrders = 1;
    [SerializeField] private int maxActiveOrders = 1; // Only 1 order at a time
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
    [SerializeField] private Transform foodIconContainer; // Container for food icons

    private DeliveryOrder activeOrder;
    private GameObject orderUIElement;
    private Image stateIcon;
    private TextMeshProUGUI timerText;

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
            else if (timeRemainingPercentage < frustratedThreshold)
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

        // Instantiate and cache the UI element
        orderUIElement = Instantiate(orderUIPrefab, orderUIContainer);
        stateIcon = orderUIElement.transform.Find("StateIcon")?.GetComponent<Image>();
        timerText = orderUIElement.transform.Find("TimerText")?.GetComponent<TextMeshProUGUI>();
        orderUIElement.SetActive(false); // Initially hide the UI element

        StartCoroutine(GenerateOrdersRoutine());
        StartCoroutine(CheckOrderExpiryRoutine());
    }

    private void Update()
    {
        UpdateOrderUI();
    }

    protected override void Interact(GameObject playerGameObject)
    {
        Debug.Log("Delivery System interacted with - checking for food items");

        Collider[] colliders = Physics.OverlapSphere(transform.position, deliveryDetectionRadius, foodLayer);
        bool orderCompleted = false;
        int totalPointsEarned = 0;

        if (activeOrder != null)
        {
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

                orderCompleted = true;
                Debug.Log($"Order completed! {activeOrder.quantity}x {activeOrder.foodType} - {pointsEarned} points");
            }
        }

        if (orderCompleted)
        {
            activeOrder = null;
            orderUIElement.SetActive(false);
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
            if (activeOrder == null)
            {
                GenerateRandomOrder();
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

        activeOrder = new DeliveryOrder
        {
            foodType = typeId.foodType,
            quantity = quantity,
            creationTime = Time.time,
            expirationTime = Time.time + orderTime
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
            image.sprite = foodTypeSprites[order.foodType];
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
            if (activeOrder != null)
            {
                if (Time.time >= activeOrder.expirationTime)
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
}