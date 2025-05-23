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
        scoreText.text = "last score" + saveData.singlePlayerLastScore.ToString();
        highScoreText.text = "high score" + saveData.singlePlayerHighscore.ToString();
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
