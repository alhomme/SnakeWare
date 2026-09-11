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
    [SerializeField] private RSE_Death m_DeathRSE;
    [SerializeField] private RSE_NewGame m_NewGameRSE;


    private string m_CurrentScene;
    private bool m_IsDead;


    private void OnEnable()
    {
        m_CurrentScene = "StarterScene";
        m_LoadSceneRSE.Event += OnLoadScene;
        m_DeathRSE.Event += OnDeath;
        m_NewGameRSE.Event += OnNewGame;
    }

    private void OnDisable()
    {
        m_LoadSceneRSE.Event -= OnLoadScene;
        m_DeathRSE.Event -= OnDeath;
        m_NewGameRSE.Event -= OnNewGame;
    }

    private void OnLoadScene(string scene)
    {
        // Load scene
        Debug.Log("SceneLoader: Unload " + m_CurrentScene);
        //SceneManager.UnloadSceneAsync(currentScene);

        Debug.Log("SceneLoader: Load: " + scene);
        StartCoroutine(LoadNextScene(m_CurrentScene, scene));
        m_CurrentScene = scene;
    }

    private IEnumerator LoadNextScene(string oldScene, string newScene)
    {
        // Play Transition except for MainMenu, or if snake is dead in MG
        if (newScene != "MainMenu" && !m_IsDead)
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

        // Play Transition Except for MainMenu, or if snake is dead in MG
        if (newScene != "MainMenu" && !m_IsDead)
        {
            m_Transition.SetTrigger("End");
            // Wait
            yield return new WaitForSeconds(1);
        }

        // Add RSE SceneLoaded
        m_SceneLoadedRSE.Dispatch(newScene);
    }

    private void OnDeath()
    {
        m_IsDead = true;
    }

    private void OnNewGame()
    {
        m_IsDead = false;
    }

}
