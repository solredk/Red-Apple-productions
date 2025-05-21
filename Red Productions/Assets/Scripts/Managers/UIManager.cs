using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class UIManager : MonoBehaviour
{
    [Header("canvasses and buttons")]
    [SerializeField] private GameObject pauseCanvas;
    [SerializeField] private GameObject lobbyCanvas;
    [SerializeField] private GameObject lastButton;

    [Header("Player Input and Event System")]   
    [SerializeField] private PlayerInputManager playerInputManager;
    [SerializeField] private EventSystem eventSystem;

    [Header("text components")]
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
        // Check if the game is paused or not and toggle the pause state accordingly
        if (!isPaused)
        {
            // If the game is not paused, set the time scale to 0 and show the pause menu and put the variable to true
            isPaused = true;
            Time.timeScale = 0f;
            pauseCanvas.SetActive(true);
        }
        else if (isPaused)
        {
            // If the game is paused, set the time scale back to 1 and hide the pause menu and put the variable to false
            isPaused = false;
            Time.timeScale = 1f;
            pauseCanvas.SetActive(false);
        }
    }

    private void LastButtonPressed()
    {
        // Check if the current selected game object is null and set it to the last button pressed
        if (eventSystem.currentSelectedGameObject == null && lastButton != null)
        {
            eventSystem.SetSelectedGameObject(lastButton);
        }
        else
        {
            // If the current selected game object is not null, set the last button to the current selected game object
            lastButton = eventSystem.currentSelectedGameObject;
        }
    }

    public void DoJoinLobbyUI()
    {
        //out the numbers of players to the text component
        PlayerCountText.text = playerInputManager.playerCount.ToString() + " players";
        // Check if the player count is 2 and hide the lobby canvas
        if (playerInputManager.playerCount == 2)
        {
            lobbyCanvas.SetActive(false);
        }
    }
}
