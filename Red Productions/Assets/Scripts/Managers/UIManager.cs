using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject pauseCanvas;
    [SerializeField] private GameObject lobbyCanvas;
    [SerializeField] private GameObject lastButton;

    [SerializeField] private PlayerInputManager playerInputManager;
    [SerializeField] private EventSystem eventSystem;

    [SerializeField] private TextMeshProUGUI PlayerCountText;

    private bool isPaused = false;

    private void Awake()
    {
        pauseCanvas.SetActive(false);
    }

    private void Update()
    {
        LastButtonPressed();
    }

    public void Pause()
    {
        if (!isPaused)
        {
            isPaused = true;
            Time.timeScale = 0f;
            pauseCanvas.SetActive(true);
        }
        else if (isPaused)
        {
            isPaused = false;
            Time.timeScale = 1f;
            pauseCanvas.SetActive(false);
        }
    }

    private void LastButtonPressed()
    {
        if (eventSystem.currentSelectedGameObject == null && lastButton != null)
        {
            eventSystem.SetSelectedGameObject(lastButton);
        }
        else
        {
            lastButton = eventSystem.currentSelectedGameObject;
        }
    }

    public void DoJoinLobbyUI()
    {
        PlayerCountText.text = playerInputManager.playerCount.ToString() + " players";
        if (playerInputManager.playerCount == 2)
        {
            lobbyCanvas.SetActive(false);
        }
    }
}
