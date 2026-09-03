using UnityEngine;

public class DeathManager : MonoBehaviour
{
    [SerializeField] private RSO_Life m_LifeRSO;

    [SerializeField] private RSE_Death m_DeathRse;


    private void OnEnable()
    {
        m_LifeRSO.OnChanged += OnLifeChanged;
    }

    private void OnDisable()
    {
        m_LifeRSO.OnChanged -= OnLifeChanged;
    }

    private void OnLifeChanged(int life)
    {
        if (life == 0)
        {
            m_DeathRse.Dispatch();
        }
    }
}
