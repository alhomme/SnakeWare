using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] Animator m_Transition;

    [SerializeField] RSE_LoadScene m_LoadSceneRSE;
    [SerializeField] RSE_SceneLoaded m_SceneLoadedRSE;

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
        //SceneManager.UnloadSceneAsync(currentScene);

        Debug.Log("SceneLoader: Load: " + scene);
        StartCoroutine(LoadNextScene(currentScene, scene));
        currentScene = scene;
    }

    private IEnumerator LoadNextScene(string oldScene, string newScene)
    {
        // FadeIn
        // Unload
        // Load
        // FadeOut


        // Play Transition
        //m_Transition.SetTrigger("Start");
        // Wait
        //yield return new WaitForSeconds(1);

        SceneManager.UnloadSceneAsync(oldScene);

        // Load Scene
        yield return SceneManager.LoadSceneAsync(newScene, LoadSceneMode.Additive);
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(newScene));

        // Add RSE SceneLoaded
        m_SceneLoadedRSE.Dispatch(newScene);
    }
}
