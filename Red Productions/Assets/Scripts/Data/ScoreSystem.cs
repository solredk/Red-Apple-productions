using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class ScoreSystem : MonoBehaviour
{
    public static ScoreSystem Instance { get; private set; }

    [Header("single player en co-op settings")]
    [SerializeField] private List<TextMeshProUGUI> scoreTexts;

    [SerializeField] private WaveSpawner waveSpawner;

    [SerializeField] private GameObject[] playerPrefab;

    private List<int> scores = new List<int>();

    [Header("co-op settings")]
    [SerializeField] private PlayerInputManager playerInputManager;

    [SerializeField] private bool isCoop;

    private SaveData saveData;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        LoadData();
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
                    scoreTexts.Add(text);
            }
        }

        else if (scores.Count < 1 &&!isCoop)
            scores.Add(0);
    }

    public void AddScore(int playerIndex, int extraScore)
    {
        waveSpawner.zombiesKilled++;

        //adding the score to the player
        scores[playerIndex] += extraScore;

        //putting the new score in the text
        scoreTexts[playerIndex].text = scores[playerIndex].ToString();
    }

    public void SaveData()
    {        
        if (isCoop)
        {
            saveData.multiPlayerPlayerScore = new List<int>(scores);
        }

        else if (!isCoop)
        {
            saveData.singlePlayerScore = scores[0];
            saveData.singlePlayerLastScore = scores[0];
            saveData.singlePlayerHighscore = Math.Max(saveData.singlePlayerHighscore, saveData.singlePlayerScore);
            saveData.gameMode = GameMode.SinglePlayer;
        }

        SaveSystem.SerializeData(saveData);
    }





    public void LoadData()
    {
        saveData = SaveSystem.DeserializeData();

        if (saveData == null)
        {
            saveData = new SaveData();
        }
    }
}
