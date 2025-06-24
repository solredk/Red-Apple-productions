using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;
public class Loadscene : MonoBehaviour
{
    [Header("scene index")]
    [SerializeField] private int sceneIndex;
    [SerializeField] private Animator Animator;

    private float timer;

    public void LoadScene()
    {
        //resetting the time scale to 1
        Time.timeScale = 1f;

        StartCoroutine(LoadAnimation());
    }

    private IEnumerator LoadAnimation()
    {
        Animator.SetTrigger("FadeOut");

        // Wait for the animation to finish
        yield return new WaitForSeconds(2);
        
        // Load the scene after the animation
        SceneManager.LoadScene(sceneIndex);
    }
}


