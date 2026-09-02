using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private InputActionReference playActionRef;
    [SerializeField] private TextMeshProUGUI highScoreValue;

    [SerializeField] private RSO_Source mSourceRSO;

    private void OnEnable()
    {
        playActionRef.action.performed += LaunchGameKey;
    }

    private void OnDisable()
    {
        playActionRef.action.performed -= LaunchGameKey;
    }

    private void Start()
    {
        string highScore = PlayerPrefs.GetString("HighScore", "0");
        highScoreValue.text = highScore;

        mSourceRSO.Value = GameSource.NewGame;
    }

    private void LaunchGameKey(InputAction.CallbackContext ctx)
    {
        SceneManager.LoadScene("MainScene");
    }

    public void LaunchGameClick()
    {
        SceneManager.LoadScene("MainScene");
    }
}
