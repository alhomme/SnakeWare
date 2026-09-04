using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] Animator m_Transition;

    [SerializeField] RSE_LoadScene m_LoadSceneRSE;

    private string currentScene;


    private void OnEnable()
    {
        currentScene = "StarterScene";
        m_LoadSceneRSE.Event += OnLoadScene;
    }

    private void OnDisable()
    {
        m_LoadSceneRSE.Event += OnLoadScene;
    }

    private void OnLoadScene(string scene)
    {
        // Load scene
        Debug.Log("SceneLoader: Unload " + currentScene);
        SceneManager.UnloadSceneAsync(currentScene);

        Debug.Log("SceneLoader: Load: " + scene);
        StartCoroutine(LoadNextScene(scene));
        currentScene = scene;
    }

    private IEnumerator LoadNextScene(string scene)
    {
        // Play Transition
        //m_Transition.SetTrigger("Start");
        // Wait
        //yield return new WaitForSeconds(1);
        // Load Scene
        yield return SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(scene));
    }
}
