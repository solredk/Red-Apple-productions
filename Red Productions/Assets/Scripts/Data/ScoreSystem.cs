using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class ScoreSystem : MonoBehaviour
{
    public static ScoreSystem Instance { get; private set; }

    [SerializeField] private TextMeshProUGUI[] scoreTexts;
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

    public void OnPlayerJoined(int playerIndex)
    {
        if (playerIndex < scoreTexts.Length)
        {
            scoreTexts[playerIndex].gameObject.SetActive(true);
        }
    }

    public void AddScore(int playerIndex, int extraScore)
    {
        scores.Add(extraScore);
        int index = playerIndex - 1; 

           scores[index] += extraScore;


        //scoreTexts[playerIndex-1].text = scores[playerIndex-1].ToString();

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
