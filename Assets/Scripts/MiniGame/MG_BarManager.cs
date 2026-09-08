using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MG_BarManager : MonoBehaviour
{
    [SerializeField] private GameObject m_IntroPanel;
    [SerializeField] private GameObject m_PlayCanvas;
    [SerializeField] private Transform m_StartPoint;
    [SerializeField] private Transform m_EndPoint;
    [SerializeField] private RectTransform m_SafeZone;
    [SerializeField] private RectTransform m_Pointer;

    [SerializeField] private Animator m_BallAnimator;
    [SerializeField] private GameObject m_GoalPanel;
    [SerializeField] private AudioSource m_KickAudioSource;
    [SerializeField] private AudioSource m_CrowdGoalAudioSource;
    [SerializeField] private AudioSource m_CrowdBooAudioSource;

    [SerializeField] private InputActionReference m_ActionInput;

    [SerializeField] private RSO_Speed m_SpeedRSO;

    [SerializeField] private RSE_Death m_DeathRSE;
    [SerializeField] private RSE_LoadScene m_LoadSceneRSE;
    [SerializeField] private RSE_MG_Success m_SuccessRSE;
    [SerializeField] private RSE_MG_Fail m_FailRSE;

    private bool m_MoveSlider;
    private Vector3 m_TargetPosition;

    private void OnEnable()
    {
        m_DeathRSE.Event += OnDeath;
        m_MoveSlider = false;
    }

    private void OnDisable()
    {
        m_DeathRSE.Event -= OnDeath;
        m_ActionInput.action.performed -= OnAction;
    }

    private void Start()
    {
        m_TargetPosition = m_EndPoint.position;

        StartCoroutine(ShowIntroPanel());
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
        // Add fade animation

        m_IntroPanel.SetActive(true);
        yield return new WaitForSeconds(2);
        m_IntroPanel.SetActive(false);


        m_ActionInput.action.performed += OnAction;
        m_PlayCanvas.SetActive(true);
        m_MoveSlider = true;
        //StartCoroutine(PlayCoroutine());
    }

    private void OnAction(InputAction.CallbackContext ctx)
    {
        m_MoveSlider = false;

        m_KickAudioSource.Play();

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
        Debug.Log("MiniGame Success");
        m_BallAnimator.SetTrigger("StartGoal");

        yield return new WaitForSeconds(1f);
        m_CrowdGoalAudioSource.Play();

        m_GoalPanel.SetActive(true);

        yield return new WaitForSeconds(2f);

        m_SuccessRSE.Dispatch();

        // Return to Arena
        m_LoadSceneRSE.Dispatch("Arena");
    }

    private IEnumerator MG_Fail()
    {
        Debug.Log("MiniGame Fail");
        m_BallAnimator.SetTrigger("StartMiss");
        m_CrowdBooAudioSource.Play();

        yield return new WaitForSeconds(0.5f);

        yield return new WaitForSeconds(2f);

        // Return to Arena
        m_LoadSceneRSE.Dispatch("Arena");

        m_FailRSE.Dispatch();
    }

    private void OnDeath()
    {

    }
}
