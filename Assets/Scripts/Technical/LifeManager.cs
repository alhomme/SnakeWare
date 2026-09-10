using UnityEngine;

public class LifeManager : MonoBehaviour
{
    [SerializeField] private RSO_Life m_LifeRSO;

    [SerializeField] private RSE_NewGame m_NewGameRSE;
    [SerializeField] private RSE_Collision m_CollisionRSE;
    [SerializeField] private RSE_MG_Fail m_FailRSE;

    private void OnEnable()
    {
        m_NewGameRSE.Event += OnNewGame;
        m_CollisionRSE.Event += OnCollision;
        m_FailRSE.Event += OnFail;

        m_LifeRSO.Value = 3;
    }

    private void OnDisable()
    {
        m_NewGameRSE.Event -= OnNewGame;
        m_CollisionRSE.Event -= OnCollision;
        m_FailRSE.Event -= OnFail;
    }

    private void OnNewGame()
    {
        m_LifeRSO.Value = 3;
    }

    private void OnCollision(string other)
    {
        Debug.Log("LifeManager.OnCollision " + other);
        if (other == "Wall" || other == "Snake")
        {
            m_LifeRSO.Value = 0;
        }
    }

    private void OnFail()
    {
        m_LifeRSO.Value--;
    }
}
