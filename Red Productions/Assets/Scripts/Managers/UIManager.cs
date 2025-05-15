using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject pauseCanvas;
    [SerializeField] private GameObject lobbyCanvas;

    [SerializeField] private PlayerInputManager playerInputManager;

    [SerializeField] private TextMeshProUGUI PlayerCountText;

    private bool isPaused = false;

    private void Awake()
    {
        pauseCanvas.SetActive(false);
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

    public void DoJoinLobbyUI()
    {
        PlayerCountText.text = playerInputManager.playerCount.ToString() + " players";
        if (playerInputManager.playerCount == 2)
        {
            lobbyCanvas.gameObject.SetActive(false);
        }
    }
}
