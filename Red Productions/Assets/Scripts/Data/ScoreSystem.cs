using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class ScoreSystem : MonoBehaviour
{
    public static ScoreSystem Instance { get; private set; }

    [SerializeField] private TextMeshProUGUI[] scoreTexts;
    private int[] scores;

    private bool isCoop;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
        isCoop = GetComponent<PlayerInputManager>() != null;

        if (isCoop)
        {
            int maxPlayers = scoreTexts.Length;
            scores = new int[maxPlayers];

            for (int i = 0; i < scoreTexts.Length; i++)
            {
                scoreTexts[i].text = "0";
                scoreTexts[i].gameObject.SetActive(false);
            }
        }
        else
        {
            scores = new int[1];
            scoreTexts[0].text = "0";
            scoreTexts[0].gameObject.SetActive(true);
        }
    }

    public void OnPlayerJoined(int playerIndex)
    {
        if (playerIndex < scoreTexts.Length)
        {
            scoreTexts[playerIndex].gameObject.SetActive(true);
        }
    }

    public void AddScore(int playerIndex, int extraScore)
    {
        if (playerIndex < 0 || playerIndex >= scores.Length) return;

        scores[playerIndex] += extraScore;
        if (playerIndex < scoreTexts.Length)
        {
            scoreTexts[playerIndex].text = scores[playerIndex].ToString();
        }
    }

    public void AddScoreSinglePlayer(int extraScore)
    {
        AddScore(0, extraScore);
    }

    public int GetScore(int playerIndex)
    {
        if (playerIndex < 0 || playerIndex >= scores.Length) return 0;
        return scores[playerIndex];
    }
}
