using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private InputActionReference m_PlayInput;
    [SerializeField] private TextMeshProUGUI m_HighScoreValue;

    [SerializeField] private RSE_LoadScene m_LoadSceneRSE;
    [SerializeField] private RSE_NewGame m_NewGameRSE;


    private void OnEnable()
    {
        m_PlayInput.action.performed += LaunchGameKey;
    }

    private void OnDisable()
    {
        m_PlayInput.action.performed -= LaunchGameKey;
    }

    private void Start()
    {
        string highScore = PlayerPrefs.GetString("HighScore", "0");
        m_HighScoreValue.text = highScore;
    }

    private void LaunchGameKey(InputAction.CallbackContext ctx)
    {
        m_NewGameRSE.Dispatch();
        m_LoadSceneRSE.Dispatch("Arena");
    }

    public void LaunchGameClick()
    {
        m_NewGameRSE.Dispatch();
        m_LoadSceneRSE.Dispatch("Arena");
    }
}
