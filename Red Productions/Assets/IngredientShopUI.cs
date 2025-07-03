using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class IngredientShopUI : Interactable
{
    [Header("Manager Reference")]
    [SerializeField] private IngredientManager manager;

    [Header("Spawn Points")]
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private float clearSpawnRadius = 0.5f;

    [Header("Food Costs")]
    [SerializeField] private int burgerCost = 65;
    [SerializeField] private int nuggetCost = 35;
    [SerializeField] private int milkshakeCost = 15;
    [SerializeField] private int friesCost = 40;

    [Header("UI Settings")]
    [Tooltip("Assign your regular shop UI canvas or panel here")]
    [SerializeField] private GameObject shopHolder;

    [Header("UI Navigation")]
    [Tooltip("Assign your shop buttons in navigation order")]
    [SerializeField] private List<Button> shopButtons;

    [Header("Input Actions")]
    [Tooltip("Reference to your InputActionAsset (should have UI/Navigate, UI/Submit, UI/Cancel)")]
    [SerializeField] private InputActionAsset inputActions;

    private PlayerMovement currentPlayerMovement;
    private PlayerHealth currentPlayerHealth;

    private bool isShopOpen;
    private float lastKnownHealth;

    private int selectedButtonIndex = 0;
    private EventSystem eventSystem;
    private InputAction navigateAction;
    private InputAction submitAction;
    private InputAction cancelAction;

    protected override void Interact(GameObject playerGameObject)
    {
        base.Interact(playerGameObject);

        currentPlayerMovement = playerGameObject.GetComponent<PlayerMovement>();
        currentPlayerHealth = playerGameObject.GetComponent<PlayerHealth>();

        OpenShopUI();
    }

    private void Awake()
    {
        eventSystem = EventSystem.current;
    }

    private void OnEnable()
    {
        SetupInput();
    }

    private void OnDisable()
    {
        TeardownInput();
    }

    private void SetupInput()
    {
        if (inputActions == null) return;

        // These names must match your InputActionAsset
        navigateAction = inputActions.FindAction("UI/Navigate");
        submitAction = inputActions.FindAction("UI/Submit");
        cancelAction = inputActions.FindAction("UI/Cancel");

        if (navigateAction != null)
        {
            navigateAction.performed += OnNavigate;
            navigateAction.Enable();
        }
        if (submitAction != null)
        {
            submitAction.performed += OnSubmit;
            submitAction.Enable();
        }
        if (cancelAction != null)
        {
            cancelAction.performed += OnCancel;
            cancelAction.Enable();
        }
    }

    private void TeardownInput()
    {
        if (navigateAction != null)
        {
            navigateAction.performed -= OnNavigate;
            navigateAction.Disable();
        }
        if (submitAction != null)
        {
            submitAction.performed -= OnSubmit;
            submitAction.Disable();
        }
        if (cancelAction != null)
        {
            cancelAction.performed -= OnCancel;
            cancelAction.Disable();
        }
    }

    public void OpenShopUI()
    {
        if (!isShopOpen)
        {
            if (shopHolder != null)
                shopHolder.SetActive(true);

            if (currentPlayerMovement != null)
                currentPlayerMovement.SetCurrentSpeed(0f);

            if (currentPlayerHealth != null)
                lastKnownHealth = currentPlayerHealth.currentHealth;

            isShopOpen = true;

            // Select the first button for navigation
            if (shopButtons != null && shopButtons.Count > 0)
            {
                selectedButtonIndex = 0;
                eventSystem.SetSelectedGameObject(shopButtons[selectedButtonIndex].gameObject);
            }
        }
    }

    public void CloseShopUI()
    {
        if (isShopOpen)
        {
            if (shopHolder != null)
                shopHolder.SetActive(false);

            if (currentPlayerMovement != null)
                currentPlayerMovement.SetCurrentSpeed(5f);

            isShopOpen = false;
            eventSystem.SetSelectedGameObject(null);
        }
    }

    private void Update()
    {
        if (isShopOpen && currentPlayerHealth && currentPlayerHealth.currentHealth < lastKnownHealth)
        {
            Debug.Log("Player took damage while in shop, closing UI");
            CloseShopUI();
        }
    }

    private void OnNavigate(InputAction.CallbackContext ctx)
    {
        if (!isShopOpen || shopButtons == null || shopButtons.Count == 0) return;

        Vector2 nav = ctx.ReadValue<Vector2>();
        if (nav.y < -0.5f)
        {
            // Down
            selectedButtonIndex = (selectedButtonIndex + 1) % shopButtons.Count;
            eventSystem.SetSelectedGameObject(shopButtons[selectedButtonIndex].gameObject);
        }
        else if (nav.y > 0.5f)
        {
            // Up
            selectedButtonIndex = (selectedButtonIndex - 1 + shopButtons.Count) % shopButtons.Count;
            eventSystem.SetSelectedGameObject(shopButtons[selectedButtonIndex].gameObject);
        }
    }

    private void OnSubmit(InputAction.CallbackContext ctx)
    {
        if (!isShopOpen || shopButtons == null || shopButtons.Count == 0) return;
        shopButtons[selectedButtonIndex].onClick.Invoke();
    }

    private void OnCancel(InputAction.CallbackContext ctx)
    {
        if (isShopOpen)
            CloseShopUI();
    }

    public void OnBurgerClick()
    {
        TrySpawn(Ingredient.IngredientType.burger, burgerCost);
    }

    public void OnChickenNuggetClick()
    {
        TrySpawn(Ingredient.IngredientType.chickenNuggets, nuggetCost);
    }

    public void OnMilkshakeClick()
    {
        TrySpawn(Ingredient.IngredientType.milkShakes, milkshakeCost);
    }

    public void OnFriesClick()
    {
        TrySpawn(Ingredient.IngredientType.fries, friesCost);
    }

    private void TrySpawn(Ingredient.IngredientType foodType, int cost)
    {
        if (ScoreSystem.Instance != null && ScoreSystem.Instance.score >= cost)
        {
            ScoreSystem.Instance.AddScore(-cost);

            Transform spawnPoint = FindClearSpawnPoint();
            if (spawnPoint == null && spawnPoints.Length > 0)
            {
                spawnPoint = spawnPoints[0];
                Debug.Log("<color=yellow>[IngredientShopUI]</color> All spawn points blocked, using the first point.");
            }

            if (spawnPoint != null && manager != null)
            {
                manager.InstantiateIngredientGroup(foodType, spawnPoint.position, spawnPoint.rotation);
            }
        }
        else
        {
            Debug.LogError("Not enough points to buy this item!");
        }
    }

    private Transform FindClearSpawnPoint()
    {
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogError("<color=red>[IngredientShopUI]</color> No spawn points assigned!");
            return null;
        }

        List<int> indices = new List<int>();
        for (int i = 0; i < spawnPoints.Length; i++)
        {
            indices.Add(i);
        }

        for (int i = 0; i < indices.Count; i++)
        {
            int temp = indices[i];
            int randomIndex = Random.Range(i, indices.Count);
            indices[i] = indices[randomIndex];
            indices[randomIndex] = temp;
        }

        foreach (int index in indices)
        {
            Transform point = spawnPoints[index];
            if (point != null)
            {
                Collider[] colliders = Physics.OverlapSphere(point.position, clearSpawnRadius);
                if (colliders.Length == 0)
                {
                    return point;
                }
            }
        }
        return null;
    }

    private void OnDrawGizmosSelected()
    {
        if (spawnPoints != null)
        {
            Gizmos.color = Color.green;
            foreach (Transform spawnPoint in spawnPoints)
            {
                if (spawnPoint != null)
                {
                    Gizmos.DrawWireSphere(spawnPoint.position, clearSpawnRadius);
                }
            }
        }
    }
}