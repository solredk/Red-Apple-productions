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

    private List<int> scores = new List<int>();

    private bool isCoop;

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
        if (playerInputManager != null && playerInputManager.playerCount == 2 && isCoop && scores.Count < 1)
        {
            for (int i = 0; i < 2; i++)
            {
                scores.Add(0);

                GameObject playerObj = GameObject.FindGameObjectWithTag("Player");

                TextMeshProUGUI scoreText = playerObj.GetComponentInChildren<TextMeshProUGUI>();

                if (!scoreTexts.Contains(scoreText))
                {
                    scoreTexts.Add(scoreText);
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
        //adding the score to the player
        scores[playerIndex] += extraScore;

        //putting the new score in the text
        scoreTexts[playerIndex].text = scores[playerIndex].ToString();
    }
}
