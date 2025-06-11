using TMPro;
using UnityEngine;

public class ScoreText : MonoBehaviour
{

    private SaveData saveData;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI highScoreText;

    private void Start()
    {
        LoadData();
        if (GameManager.Instance.gameMode == GameMode.SinglePlayer)
        {
            scoreText.text = "last score" + saveData.singlePlayerLastScore.ToString();
            highScoreText.text = "high score" + saveData.singlplayerHighscore.ToString();
        }
        else if (GameManager.Instance.gameMode == GameMode.CoOp)
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
