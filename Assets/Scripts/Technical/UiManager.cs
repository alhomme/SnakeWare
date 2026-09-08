using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_ScoreText;
    [SerializeField] private Image m_Life;
    [SerializeField] private Sprite[] m_LifeSprites;
    [SerializeField] private GameObject m_GameOverPanel;
    [SerializeField] private TextMeshProUGUI m_GameOverScoreText;

    [SerializeField] private InputActionReference m_ActionInput;

    [SerializeField] private RSO_Life m_LifeRSO;
    [SerializeField] private RSO_Score m_ScoreRSO;

    [SerializeField] private RSE_NewGame m_NewGameRSE;
    [SerializeField] private RSE_Death m_DeathRSE;
    [SerializeField] private RSE_LoadScene m_LoadSceneRSE;

    private void OnEnable()
    {
        m_ScoreRSO.OnChanged += OnScoreChanged;
        m_LifeRSO.OnChanged += OnLifeChanged;
        m_NewGameRSE.Event += OnNewGame;
        m_DeathRSE.Event += OnDeath;
    }

    private void OnDisable()
    {
        m_ScoreRSO.OnChanged -= OnScoreChanged;
        m_LifeRSO.OnChanged -= OnLifeChanged;
        m_NewGameRSE.Event -= OnNewGame;
        m_DeathRSE.Event -= OnDeath;

        m_ActionInput.action.performed -= OnAction;
    }

    private void OnScoreChanged(int newScore)
    {
        //Debug.Log("UI Manager: OnScoreChanged");
        m_ScoreText.text = newScore.ToString();
    }

    private void OnLifeChanged(int newLife)
    {
        Debug.Log("UI Manager: OnLifeChanged, newLife = " + newLife);
        m_Life.sprite = m_LifeSprites[newLife];
    }

    private void OnNewGame()
    {
        Debug.Log("UI Manager: OnNewGame");
        m_GameOverPanel.SetActive(false);
        m_Life.enabled = true;
        m_ScoreText.enabled = true;
    }

    private void OnDeath()
    {
        Debug.Log("UiManager: OnGameEnded");
        m_Life.sprite = m_LifeSprites[0];
        m_GameOverScoreText.text = m_ScoreRSO.Value.ToString();
        m_GameOverPanel.SetActive(true);

        m_ActionInput.action.performed += OnAction;
    }

    private void OnAction(InputAction.CallbackContext ctx)
    {
        m_ActionInput.action.performed -= OnAction;
        // Load Menu
        m_Life.enabled = false;
        m_ScoreText.enabled = false;
        m_GameOverPanel.SetActive(false);
        m_LoadSceneRSE.Dispatch("MainMenu");
    }
}
