using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI mScoreText;
    [SerializeField] private Image mLife;
    [SerializeField] private Sprite[] mLifeSprites;
    [SerializeField] private GameObject mGameOverPanel;
    [SerializeField] private TextMeshProUGUI mGameOverScoreText;

    [SerializeField] private RSO_Life mLifeRSO;
    [SerializeField] private RSO_Score mScoreRSO;
    [SerializeField] private RSE_EndGame mEndGameRSE;

    private void OnEnable()
    {
        //Debug.Log("UI Manager: OnEnable");
        mScoreRSO.OnChanged += OnScoreChanged;
        mLifeRSO.OnChanged += OnLifeChanged;
        mEndGameRSE.Event += OnGameEnded;
    }

    private void OnDisable()
    {
        mScoreRSO.OnChanged -= OnScoreChanged;
        mLifeRSO.OnChanged -= OnLifeChanged;
        mEndGameRSE.Event -= OnGameEnded;
    }

    private void OnScoreChanged(int newScore)
    {
        //Debug.Log("UI Manager: OnScoreChanged");
        mScoreText.text = newScore.ToString();
    }

    private void OnLifeChanged(int newLife)
    {
        Debug.Log("UI Manager: OnLifeChanged");
        mLife.sprite = mLifeSprites[newLife];
    }

    private void OnGameEnded()
    {
        Debug.Log("UiManager: OnGameEnded");
        mLife.sprite = mLifeSprites[0];
        mGameOverScoreText.text = mScoreRSO.Value.ToString();
        mGameOverPanel.SetActive(true);
    }

}
