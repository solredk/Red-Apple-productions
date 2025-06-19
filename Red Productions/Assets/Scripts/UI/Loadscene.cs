using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;
public class Loadscene : MonoBehaviour
{
    [Header("scene index")]
    [SerializeField] private int sceneIndex;

    public void LoadScene()
    {
        //resetting the time scale to 1
        Time.timeScale = 1f;        
        
        SceneManager.LoadScene(sceneIndex);
    }

    public void ExitGame()
    {
         // Exiting the game
        Application.Quit();
    }
}
