using UnityEngine;

public class SpeedManager : MonoBehaviour
{
    [SerializeField] private RSO_Speed m_SpeedRSO;

    [SerializeField] private RSE_NewGame m_NewGameRSE;
    [SerializeField] private RSE_Death m_DeathRSE;
    [SerializeField] private RSE_MG_Success m_SuccessRSE;

    private void OnEnable()
    {
        m_NewGameRSE.Event += OnNewGame;
        m_DeathRSE.Event += OnDeath;
        m_SuccessRSE.Event += OnSuccess;
    }

    private void OnDisable()
    {
        m_NewGameRSE.Event -= OnNewGame;
        m_DeathRSE.Event -= OnDeath;
        m_SuccessRSE.Event -= OnSuccess;
    }

    private void OnNewGame()
    {
        m_SpeedRSO.Value = 1.0f;
    }

    private void OnDeath()
    {
        m_SpeedRSO.Value = 1.0f;
    }

    private void OnSuccess()
    {
        m_SpeedRSO.Value += 0.5f;
    }
}
