using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] private RSO_Score m_ScoreRSO;

    [SerializeField] private RSE_NewGame m_NewGameRSE;
    [SerializeField] private RSE_MG_Success m_SuccessRSE;

    private void OnEnable()
    {
        Debug.Log("ScoreManager: OnEnable");

        m_NewGameRSE.Event += OnNewGame;
        m_SuccessRSE.Event += OnSuccess;
    }

    private void OnDisable()
    {
        m_NewGameRSE.Event -= OnNewGame;
        m_SuccessRSE.Event -= OnSuccess;
    }

    private void OnNewGame()
    {
        m_ScoreRSO.Value = 0;
    }

    private void OnSuccess()
    {
        m_ScoreRSO.Value++;
    }
}
