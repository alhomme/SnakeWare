using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] private Animator m_Transition;

    [SerializeField] private RSE_LoadScene m_LoadSceneRSE;
    [SerializeField] private RSE_SceneLoaded m_SceneLoadedRSE;
    [SerializeField] private RSE_PlayMusic m_PlayMusicRSE;


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
        // Play Transition Except for MainMenu
        if (newScene != "MainMenu")
        {
            m_Transition.SetTrigger("Start");
            // Wait
            yield return new WaitForSeconds(1);
        }

        SceneManager.UnloadSceneAsync(oldScene);

        m_PlayMusicRSE.Dispatch(newScene);

        // Load Scene
        yield return SceneManager.LoadSceneAsync(newScene, LoadSceneMode.Additive);
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(newScene));

        // Play Transition Except for MainMenu
        if (newScene != "MainMenu")
        {
            m_Transition.SetTrigger("End");
            // Wait
            yield return new WaitForSeconds(1);
        }

        // Add RSE SceneLoaded
        m_SceneLoadedRSE.Dispatch(newScene);
    }
}
