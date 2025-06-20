using TMPro;
using UnityEngine;

public class ScoreText : MonoBehaviour
{
    [SerializeField] private GameMode gameMode;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI highScoreText;
    
    private SaveData saveData;

    private void Start()
    {
        LoadData();

        if (gameMode == GameMode.SinglePlayer)
        {
            scoreText.text = "last score" + saveData.singlePlayerLastScore.ToString();
            highScoreText.text = "high score" + saveData.singlplayerHighscore.ToString();
        }

        else if (gameMode == GameMode.CoOp)
        {
            scoreText.text = "last score" + saveData.coOpLastScore.ToString();
            highScoreText.text = "high score" + saveData.coOpHighScore.ToString();
        }
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
