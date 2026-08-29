using UnityEngine;

public class EndManager : MonoBehaviour
{
    [SerializeField] private RSO_Life mSnakeLifeRSO;

    [SerializeField] private RSE_EndGame mEndEvent;

    private void OnEnable()
    {
        mSnakeLifeRSO.OnChanged += OnLifeChanged;
    }

    private void OnDisable()
    {
        mSnakeLifeRSO.OnChanged -= OnLifeChanged;
    }

    private void OnLifeChanged(int life)
    {
        if (life == 0)
        {
            mEndEvent.Dispatch();
        }
    }
}
