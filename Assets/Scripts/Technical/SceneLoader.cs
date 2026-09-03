using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] RSE_LoadScene m_LoadSceneRSE;

    private string currentScene;


    private void OnEnable()
    {
        currentScene = null;
        m_LoadSceneRSE.Event += OnLoadScene;
    }

    private void OnDisable()
    {
        m_LoadSceneRSE.Event += OnLoadScene;
    }

    private void OnLoadScene(string scene)
    {
        // Load scene
        Debug.Log("SceneLoader: OnLoadScene: " + scene);

        if (currentScene != null)
        {
            Debug.Log("SceneLoader: Unload " + currentScene);
            SceneManager.UnloadSceneAsync(currentScene);
        }

        SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);
        currentScene = scene;
    }
}
