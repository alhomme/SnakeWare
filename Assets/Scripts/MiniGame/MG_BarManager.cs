using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MG_BarManager : MonoBehaviour
{
    [SerializeField] private RSO_Speed m_SpeedRSO;

    [SerializeField] private RSE_Death m_DeathRSE;
    [SerializeField] private RSE_LoadScene m_LoadSceneRSE;
    [SerializeField] private RSE_MG_Success m_SuccessRSE;
    [SerializeField] private RSE_MG_Fail m_FailRSE;

    private IEnumerator mCoroutine;

    private void OnEnable()
    {
        m_DeathRSE.Event += OnDeath;

        mCoroutine = WaitCoroutine();
    }

    private void OnDisable()
    {
        m_DeathRSE.Event -= OnDeath;
    }

    private void Start()
    {
        StartCoroutine(mCoroutine);
    }

    private IEnumerator WaitCoroutine()
    {
        yield return new WaitForSeconds(1);

        m_SuccessRSE.Dispatch();

        // Return to Arena
        m_LoadSceneRSE.Dispatch("Arena");
    }

    private void OnDeath()
    {

    }
}
