using UnityEngine;
using UnityEngine.SceneManagement;
public class Loadscene : MonoBehaviour
{
    [SerializeField] private int sceneIndex;
    public void LoadScene()
    {
        SceneManager.LoadScene(sceneIndex);
        Time.timeScale = 1f;
    }
}
