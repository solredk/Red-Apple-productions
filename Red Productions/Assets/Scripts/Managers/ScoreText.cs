using TMPro;
using UnityEngine;

public class ScoreText : MonoBehaviour
{
    [Header("Game Modes")]
    [SerializeField] private GameMode gameMode;

    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI highScoreText;
    
    private SaveData saveData;

    private void Start()
    {
        LoadData();

        if (gameMode == GameMode.SinglePlayer)
        {
            // Display the last score and high score from the save data json file
            scoreText.text = "last score" + saveData.singlePlayerLastScore.ToString();
            highScoreText.text = "high score" + saveData.singlplayerHighscore.ToString();
        }

        else if (gameMode == GameMode.CoOp)
        {
            // Display the last score and high score from the save data json file
            scoreText.text = "last score" + saveData.coOpLastScore.ToString();
            highScoreText.text = "high score" + saveData.coOpHighScore.ToString();
        }
    }

    public void LoadData()
    {
        saveData = SaveSystem.DeserializeData();

        // check if there is no save data, if so, create a new one
        if (saveData == null)
        {
            saveData = new SaveData();
        }
    }
}
