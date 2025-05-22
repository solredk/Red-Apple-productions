using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class ScoreSystem : MonoBehaviour
{
    public static ScoreSystem Instance { get; private set; }

    [SerializeField] private List<TextMeshProUGUI> scoreTexts;
    [SerializeField] private PlayerInputManager playerInputManager;

    [SerializeField] private GameObject[] playerPrefab;

    private List<int> scores = new List<int>();

    [SerializeField] private bool isCoop;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if (playerInputManager != null && playerInputManager.playerCount == 2 && isCoop && scores.Count <= 1)
        {
            scores.Add(0);

            playerPrefab = GameObject.FindGameObjectsWithTag("Player");
            foreach (GameObject player in playerPrefab)
            {
                TextMeshProUGUI text = player.GetComponentInChildren<TextMeshProUGUI>();
                if (text != null && !scoreTexts.Contains(text))
                {
                    scoreTexts.Add(text);
                }
            }
        }

        else if (scores.Count < 1 &&!isCoop)
        {
            scores.Add(0);
        }
    }

    public void AddScore(int playerIndex, int extraScore)
    {
        // Check if the playerIndex is valid
        if (playerIndex < 0 || playerIndex >= scores.Count)
        {
            Debug.LogWarning($"AddScore: Ongeldige playerIndex {playerIndex}");
            return;
        }
        // Check if the scoreTexts list is valid
        if (playerIndex >= scoreTexts.Count || scoreTexts[playerIndex] == null)
        {
            Debug.LogWarning($"AddScore: Geen scoreText gevonden voor speler {playerIndex}");
            return;
        }

        //adding the score to the player
        scores[playerIndex] += extraScore;

        //putting the new score in the text
        scoreTexts[playerIndex].text = scores[playerIndex].ToString();
    }
}
