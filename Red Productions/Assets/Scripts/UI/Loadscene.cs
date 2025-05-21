using UnityEngine;
using UnityEngine.SceneManagement;
public class Loadscene : MonoBehaviour
{
    [Header("scene index")]
    [SerializeField] private int sceneIndex;

    public void LoadScene()
    {
        //loading the scene with the given index
        SceneManager.LoadScene(sceneIndex);

        //resetting the time scale to 1
        Time.timeScale = 1f;
    }
}
