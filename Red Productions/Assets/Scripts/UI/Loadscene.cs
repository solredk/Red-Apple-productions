using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;
public class Loadscene : MonoBehaviour
{
    [Header("scene index")]
    [SerializeField] private int sceneIndex;

    [SerializeField] private Animator animator;

    private float timer;

    public void LoadScene()
    {
        //resetting the time scale to 1
        Time.timeScale = 1f;        
        
  //      animator.SetTrigger("FadeOut");

    //    timer += Time.deltaTime;

//        if (timer >= 1)
            //loading the scene with the given index
            SceneManager.LoadScene(sceneIndex);
    }
}
