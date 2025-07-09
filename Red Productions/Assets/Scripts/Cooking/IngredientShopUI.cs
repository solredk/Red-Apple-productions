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

    [Header("Food Costs")]
    [SerializeField] private int burgerCost = 30;
    [SerializeField] private int nuggetCost = 25;
    [SerializeField] private int milkshakeCost = 25;
    [SerializeField] private int friesCost = 25;

    [Header("UI Settings")]
    [SerializeField] private GameObject shopHolder;

    [Header("UI Navigation")]
    [SerializeField] private List<Button> shopButtons;

    [Header("Input Actions")]
    [SerializeField] private InputActionAsset inputActions;

    [SerializeField] private string gameplayActionMapName = "Player";
    [SerializeField] private string uiActionMapName = "UI";

    // --- Manual Ingredient Prefab Assignments ---
    [Header("Manual Ingredient Prefabs")]
    [SerializeField] private GameObject burgerBreadPrefab;
    [SerializeField] private GameObject burgerMeatPrefab;
    [SerializeField] private GameObject burgerCheesePrefab;

    [SerializeField] private GameObject friesBagPrefab;

    [SerializeField] private GameObject nuggetsNuggetPrefab;

    [SerializeField] private GameObject milkshakeCupPrefab;

    private PlayerMovement currentPlayerMovement;
    private PlayerHealth currentPlayerHealth;

    private bool isShopOpen;
    private float lastKnownHealth;

    private int selectedButtonIndex = 0;
    private EventSystem eventSystem;
    private InputAction navigateAction;
    private InputAction submitAction;
    private InputAction cancelAction;
    private InputActionMap gameplayMap;
    private InputActionMap uiMap;

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
        if (inputActions != null)
        {
            gameplayMap = inputActions.FindActionMap(gameplayActionMapName, true);
            uiMap = inputActions.FindActionMap(uiActionMapName, true);
        }
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

        navigateAction = inputActions.FindAction($"{uiActionMapName}/Navigate", true);
        submitAction = inputActions.FindAction($"{uiActionMapName}/Submit", true);
        cancelAction = inputActions.FindAction($"{uiActionMapName}/Cancel", true);

        if (navigateAction != null)
            navigateAction.performed += OnNavigate;
        if (submitAction != null)
            submitAction.performed += OnSubmit;
        if (cancelAction != null)
            cancelAction.performed += OnCancel;
    }

    private void TeardownInput()
    {
        if (navigateAction != null)
            navigateAction.performed -= OnNavigate;
        if (submitAction != null)
            submitAction.performed -= OnSubmit;
        if (cancelAction != null)
            cancelAction.performed -= OnCancel;
    }

    public void OpenShopUI()
    {
        if (isShopOpen) return;

        if (shopHolder != null)
            shopHolder.SetActive(true);

        if (gameplayMap != null) gameplayMap.Disable();
        if (uiMap != null) uiMap.Enable();

        if (currentPlayerMovement != null)
            currentPlayerMovement.SetCurrentSpeed(0f);

        if (currentPlayerHealth != null)
            lastKnownHealth = currentPlayerHealth.currentHealth;

        isShopOpen = true;

        if (shopButtons != null && shopButtons.Count > 0)
        {
            selectedButtonIndex = 0;
            eventSystem.SetSelectedGameObject(shopButtons[selectedButtonIndex].gameObject);
        }
    }

    public void CloseShopUI()
    {
        if (!isShopOpen) return;

        if (shopHolder != null)
            shopHolder.SetActive(false);

        if (gameplayMap != null) gameplayMap.Enable();
        if (uiMap != null) uiMap.Disable();

        if (currentPlayerMovement != null)
            currentPlayerMovement.SetCurrentSpeed(5f);

        isShopOpen = false;
        eventSystem.SetSelectedGameObject(null);
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
            selectedButtonIndex = (selectedButtonIndex + 1) % shopButtons.Count;
            eventSystem.SetSelectedGameObject(shopButtons[selectedButtonIndex].gameObject);
        }
        else if (nav.y > 0.5f)
        {
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
        TrySpawnManual(burgerCost, new GameObject[] {
            burgerBreadPrefab, burgerMeatPrefab, burgerCheesePrefab
        });
    }

    public void OnChickenNuggetClick()
    {
        TrySpawnManual(nuggetCost, new GameObject[] {
            nuggetsNuggetPrefab
        });
    }

    public void OnMilkshakeClick()
    {
        TrySpawnManual(milkshakeCost, new GameObject[] {
            milkshakeCupPrefab
        });
    }

    public void OnFriesClick()
    {
        TrySpawnManual(friesCost, new GameObject[] {
            friesBagPrefab
        });
    }

    private void TrySpawnManual(int cost, GameObject[] prefabs)
    {
        if (ScoreSystem.Instance != null && ScoreSystem.Instance.score >= cost)
        {
            ScoreSystem.Instance.AddScore(-cost);

            Vector3 spawnPos = (spawnPoints != null && spawnPoints.Length > 0)
                ? spawnPoints[0].position
                : Vector3.zero;
            Quaternion spawnRot = (spawnPoints != null && spawnPoints.Length > 0)
                ? spawnPoints[0].rotation
                : Quaternion.identity;

            foreach (var prefab in prefabs)
            {
                if (prefab != null)
                {
                    Instantiate(prefab, spawnPos, spawnRot);
                }
                else
                {
                    Debug.LogWarning("A required ingredient prefab is not assigned in the Inspector.");
                }
            }
        }
        else
        {
            Debug.LogError("Not enough points to buy this item!");
        }
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
                    Gizmos.DrawWireSphere(spawnPoint.position, 0.5f);
                }
            }
        }
    }
}