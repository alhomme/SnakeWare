using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MG_BarManager : MonoBehaviour
{
    [SerializeField] private GameObject m_PlayCanvas;
    [SerializeField] private Transform m_StartPoint;
    [SerializeField] private Transform m_EndPoint;
    [SerializeField] private RectTransform m_SafeZone;
    [SerializeField] private RectTransform m_Pointer;

    [SerializeField] private Animator m_BallAnimator;
    [SerializeField] private Animator m_IntroAnimator;
    [SerializeField] private GameObject m_GoalPanel;

    [SerializeField] private InputActionReference m_ActionInput;

    [SerializeField] private RSO_Speed m_SpeedRSO;

    [SerializeField] private RSE_LoadScene m_LoadSceneRSE;
    [SerializeField] private RSE_SceneLoaded m_SceneLoadedRSE;
    [SerializeField] private RSE_MG_Success m_SuccessRSE;
    [SerializeField] private RSE_MG_Fail m_FailRSE;
    [SerializeField] private RSE_PlaySFX m_PlaySFXRSE;

    private bool m_MoveSlider;
    private Vector3 m_TargetPosition;

    private void OnEnable()
    {
        m_SceneLoadedRSE.Event += OnSceneLoaded;
        m_MoveSlider = false;
    }

    private void OnDisable()
    {
        m_SceneLoadedRSE.Event -= OnSceneLoaded;
        m_ActionInput.action.performed -= OnAction;
    }

    private void Update()
    {
        if (m_MoveSlider)
        {
            float speed = 500 + (m_SpeedRSO.Value * 100);
            m_Pointer.position = Vector3.MoveTowards(m_Pointer.position, m_TargetPosition, speed * Time.deltaTime);

            if (Vector3.Distance(m_Pointer.position, m_StartPoint.position) < 0.1f)
            {
                m_TargetPosition = m_EndPoint.position;
            }
            else if (Vector3.Distance(m_Pointer.position, m_EndPoint.position) < 0.1f)
            {
                m_TargetPosition = m_StartPoint.position;
            }

        }
    }

    private IEnumerator ShowIntroPanel()
    {
        m_TargetPosition = m_EndPoint.position;

        yield return new WaitForSeconds(1f);
        m_IntroAnimator.SetTrigger("Start");
        yield return new WaitForSeconds(1f);

        m_ActionInput.action.performed += OnAction;
        m_PlayCanvas.SetActive(true);
        m_MoveSlider = true;
    }

    private void OnAction(InputAction.CallbackContext ctx)
    {
        m_MoveSlider = false;
        m_ActionInput.action.performed -= OnAction;

        m_PlaySFXRSE.Dispatch("Kick");

        if (RectTransformUtility.RectangleContainsScreenPoint(m_SafeZone, m_Pointer.position, null))
        {
            StartCoroutine(MG_Success());
            
        }
        else
        {
            StartCoroutine(MG_Fail());
        }
    }

    private IEnumerator MG_Success()
    {
        Utils.Log("MiniGame Success");

        m_BallAnimator.SetTrigger("StartGoal");
        yield return new WaitForSeconds(1f);
        
        m_PlaySFXRSE.Dispatch("CrowdGoal");
        m_GoalPanel.SetActive(true);
        yield return new WaitForSeconds(2f);

        // Return to Arena
        m_SuccessRSE.Dispatch();
        m_PlaySFXRSE.Dispatch("Stop");
        m_LoadSceneRSE.Dispatch("Arena");
    }

    private IEnumerator MG_Fail()
    {
        Utils.Log("MiniGame Fail");

        m_BallAnimator.SetTrigger("StartMiss");
        yield return new WaitForSeconds(0.5f);

        m_PlaySFXRSE.Dispatch("CrowdBoo");
        yield return new WaitForSeconds(3f);


        // Return to Arena
        m_FailRSE.Dispatch();
        m_PlaySFXRSE.Dispatch("Stop");
        m_LoadSceneRSE.Dispatch("Arena");
    }

    private void OnSceneLoaded(string scene)
    {
        if (scene ==  "MiniGame_Bar")
            StartCoroutine(ShowIntroPanel());
    }

}
