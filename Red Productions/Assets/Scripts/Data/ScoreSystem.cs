using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class ScoreSystem : MonoBehaviour
{
    public static ScoreSystem Instance { get; private set; }

    [Header("single player en co-op settings")]
    [SerializeField] private TextMeshProUGUI scoreText;

    [SerializeField] private WaveSpawner waveSpawner;

    [SerializeField] private GameObject[] playerPrefab;

    private int score;

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
    }


    public void AddScore(int extraScore)
    {
        waveSpawner.zombiesKilled++;

        score += extraScore;

        //putting the new score in the text
        scoreText.text = score.ToString();
    }
    
    public void SaveData()
    {        
        if (GameManager.Instance.gameMode == GameMode.CoOp)
        {
            if (score > saveData.coOpHighScore)
            {
                saveData.coOpHighScore = score;
            }
            saveData.coOpLastScore = score;
        }

        else
        {
            if (score > saveData.singlePlayerLastScore)
            {
                saveData.singlplayerHighscore = score;
            }

            saveData.singlePlayerLastScore = score;
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
