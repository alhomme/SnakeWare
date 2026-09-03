using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StarterManager : MonoBehaviour
{
    [SerializeField] RSE_LoadScene m_LoadSceneRSE;


    public IEnumerator Start()
    {
        Debug.Log("StarterManager");

        yield return SceneManager.LoadSceneAsync("TechnicalScene", LoadSceneMode.Additive);

        m_LoadSceneRSE.Dispatch("MainMenu");
    }

}
