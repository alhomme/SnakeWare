using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MG_MashManager : MonoBehaviour
{
    [SerializeField] private GameObject m_PlayCanvas;
    [SerializeField] private GameObject m_GoalPanel;
    [SerializeField] private TextMeshProUGUI m_CountdownText;
    [SerializeField] private Slider m_Gauge;

    [SerializeField] private Animator m_BallAnimator;
    [SerializeField] private Animator m_IntroAnimator;

    [SerializeField] private InputActionReference m_ActionInput;

    [SerializeField] private RSO_Speed m_SpeedRSO;

    [SerializeField] private RSE_LoadScene m_LoadSceneRSE;
    [SerializeField] private RSE_SceneLoaded m_SceneLoadedRSE;
    [SerializeField] private RSE_MG_Success m_SuccessRSE;
    [SerializeField] private RSE_MG_Fail m_FailRSE;
    [SerializeField] private RSE_PlaySFX m_PlaySFXRSE;

    private bool m_IsListening;
    private int m_Countdown;
    private float m_CountTime;
    private float m_IncGauge;

    private void OnEnable()
    {
        m_SceneLoadedRSE.Event += OnSceneLoaded;

        m_IsListening = false;
    }

    private void OnDisable()
    {
        m_SceneLoadedRSE.Event -= OnSceneLoaded;
        m_ActionInput.action.performed -= OnAction;
    }

    private void OnSceneLoaded(string scene)
    {
        if (scene == "MiniGame_Mash")
            StartCoroutine(ShowIntroPanel());
    }

    private IEnumerator ShowIntroPanel()
    {
        yield return new WaitForSeconds(1f);
        m_IntroAnimator.SetTrigger("Start");
        yield return new WaitForSeconds(1f);

        m_ActionInput.action.performed += OnAction;
        m_PlayCanvas.SetActive(true);

        StartCoroutine(StartCountdown());
    }

    private void Update()
    {
        if (m_IsListening)
        {
            m_CountTime += Time.deltaTime;
            if (m_CountTime >= 1)
            {
                m_Countdown--;
                UpdateCountdown();
                if (m_Countdown == 0)
                {
                    // Fail
                    m_IsListening = false;
                    StartCoroutine(MG_Fail());
                    return;
                }

                m_CountTime = 0;
            }

            // Check Gauge
            if (m_Gauge.value == 1f)
            {
                // Success
                m_IsListening = false;
                StartCoroutine(MG_Success());
                return;
            }
        }
    }

    private IEnumerator StartCountdown()
    {
        // Print "Ready..."
        m_CountdownText.text = "Ready...";
        yield return new WaitForSeconds(0.5f);
        // Print "GO!"
        m_CountdownText.text = "GO!";
        yield return new WaitForSeconds(0.5f);


        m_IsListening = true;
        m_Countdown = 5;
        m_CountTime = 0f;
        m_IncGauge = 0.25f / m_SpeedRSO.Value;
        m_ActionInput.action.performed += OnAction;

        UpdateCountdown();
    }

    private IEnumerator MG_Success()
    {
        Debug.Log("MiniGame Success");
        m_ActionInput.action.performed -= OnAction;
        m_PlayCanvas.SetActive(false);

        m_PlaySFXRSE.Dispatch("Kick");
        m_BallAnimator.SetTrigger("Success");
        yield return new WaitForSeconds(0.5f);

        m_PlaySFXRSE.Dispatch("CrowdGoal");
        m_GoalPanel.SetActive(true);
        yield return new WaitForSeconds(2f);

        m_SuccessRSE.Dispatch();

        // Return to Arena
        m_PlaySFXRSE.Dispatch("Stop");
        m_LoadSceneRSE.Dispatch("Arena");
    }


    private IEnumerator MG_Fail()
    {
        Debug.Log("MiniGame Fail");
        m_ActionInput.action.performed -= OnAction;
        m_PlayCanvas.SetActive(false);
        m_PlaySFXRSE.Dispatch("Kick");

        m_BallAnimator.SetTrigger("Fail");
        yield return new WaitForSeconds(0.5f);

        m_PlaySFXRSE.Dispatch("CrowdBoo");
        yield return new WaitForSeconds(3f);

        // Return to Arena
        m_LoadSceneRSE.Dispatch("Arena");
        m_PlaySFXRSE.Dispatch("Stop");
        m_FailRSE.Dispatch();
    }

    private void UpdateCountdown()
    {
        if (m_Countdown > 0)
            m_CountdownText.text = m_Countdown.ToString();
        else
            m_CountdownText.text = "";
    }

    private void OnAction(InputAction.CallbackContext ctx)
    {
        // Increase gauge value depending on RSO_Speed
        m_Gauge.value += m_IncGauge;
    }
}
