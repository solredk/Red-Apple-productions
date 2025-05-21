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
    public void OnPlayerJoined(int playerIndex)
    {
        if (playerIndex < scoreTexts.Count)
        {
            scoreTexts[playerIndex].gameObject.SetActive(true);
        }
    }

    public void AddScore(int playerIndex, int extraScore)
    {
        int index = playerIndex; 
        Debug.Log(playerIndex + "/ of the "+ scores.Count);
        scores[index] += extraScore;


        scoreTexts[playerIndex].text = scores[playerIndex].ToString();

        // if (playerIndex < 0 || playerIndex >= scores.Length) return;

        //  scores[playerIndex] += extraScore;
        // if (playerIndex < scoreTexts.Length)
        //{
        //  scoreTexts[playerIndex].text = scores[playerIndex].ToString();
        //}
    }

    public void AddScoreSinglePlayer(int extraScore)
    {
        AddScore(0, extraScore);
    }

    public int GetScore(int playerIndex)
    {
        if (playerIndex < 0 || playerIndex >= scores.Count) return 0;
        return scores[playerIndex];
    }
}
