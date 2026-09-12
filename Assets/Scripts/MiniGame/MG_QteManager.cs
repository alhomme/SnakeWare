using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.InputSystem;

public class MG_QteManager : MonoBehaviour
{
    [SerializeField] private PositionConstraint m_CameraPC;

    [SerializeField] private Animator m_BallAnimator;
    [SerializeField] private Animator m_IntroAnimator;
    [SerializeField] private GameObject m_GoalPanel;

    [SerializeField] private TextMeshProUGUI m_InputText;

    [SerializeField] private InputActionReference[] m_Inputs;


    [SerializeField] private RSO_Speed m_SpeedRSO;
    [SerializeField] private RSO_InputType m_InputTypeRSO;

    [SerializeField] private RSE_LoadScene m_LoadSceneRSE;
    [SerializeField] private RSE_SceneLoaded m_SceneLoadedRSE;
    [SerializeField] private RSE_MG_Success m_SuccessRSE;
    [SerializeField] private RSE_MG_Fail m_FailRSE;
    [SerializeField] private RSE_PlaySFX m_PlaySFXRSE;

    private string[] m_KeyboardInputs =
        { "<sprite name=kd>", "<sprite name=kr>", "<sprite name=kl>", "<sprite name=ku>"};
    private string[] m_GamepadInputs =
        { "<sprite name=xa>", "<sprite name=xb>", "<sprite name=xx>", "<sprite name=xy>"};
    private string[] m_UsedInputs;
    private int m_RandomInput = 0;
    private int m_Step;
    private bool m_IsListening;
    private int m_KeyPressed = 0; // 0 = Idle, 1 = Correct, 2 = Fail
    private float m_Countdown = 0f;
    private float m_MaxTime = 0f;


    private void OnEnable()
    {
        m_SceneLoadedRSE.Event += OnSceneLoaded;
        m_InputTypeRSO.OnChanged += OnInputChanged;

        m_Inputs[0].action.performed += OnButton1;
        m_Inputs[1].action.performed += OnButton2;
        m_Inputs[2].action.performed += OnButton3;
        m_Inputs[3].action.performed += OnButton4;

        // Use RSO_Speed
        m_MaxTime = 2f;
        m_MaxTime -= (m_SpeedRSO.Value - 1f) / 10f;

        Debug.Log("Max Time = " + m_MaxTime);
    }

    private void OnDisable()
    {
        m_SceneLoadedRSE.Event -= OnSceneLoaded;
        m_InputTypeRSO.OnChanged -= OnInputChanged;

        m_Inputs[0].action.performed -= OnButton1;
        m_Inputs[1].action.performed -= OnButton2;
        m_Inputs[2].action.performed -= OnButton3;
        m_Inputs[3].action.performed -= OnButton4;
    }

    private void Start()
    {
        if (m_InputTypeRSO.Value == InputType.Keyboard)
            m_UsedInputs = m_KeyboardInputs;
        else if (m_InputTypeRSO.Value == InputType.Gamepad)
            m_UsedInputs = m_GamepadInputs;

        m_IsListening = false;
        m_Step = 0;

        //OnSceneLoaded("MiniGame_QTE");
    }

    private void Update()
    {
        // Check for correct Input
        if (m_IsListening)
        {
            if (m_KeyPressed != 0)
            {
                m_IsListening = false;
                ClearQTE();

                if (m_KeyPressed == 1)
                {
                    if (m_Step == 2)
                    {
                        m_CameraPC.enabled = false;
                        StartCoroutine(MG_Success());
                    }
                    else
                    {
                        StartCoroutine(StartNext());
                        m_Step++;
                    }
                }
                else if (m_KeyPressed == 2)
                {
                    StartCoroutine(MG_Fail());
                }
            }

            if (m_Countdown >= m_MaxTime)
            {
                m_IsListening = false;
                ClearQTE();
                StartCoroutine(MG_Fail());
            }

            m_Countdown += Time.deltaTime;
        }
    }

    private IEnumerator MG_Success()
    {
        Debug.Log("MiniGame Success");

        m_PlaySFXRSE.Dispatch("Kick");
        m_BallAnimator.SetTrigger("Success");
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
        Debug.Log("MiniGame Fail");

        if (m_Step == 2)
            m_CameraPC.enabled = false;

        m_PlaySFXRSE.Dispatch("Kick");
        m_BallAnimator.SetTrigger("Fail");
        yield return new WaitForSeconds(0.5f);
        
        m_PlaySFXRSE.Dispatch("CrowdBoo");
        yield return new WaitForSeconds(3f);

        // Return to Arena
        m_FailRSE.Dispatch();
        m_PlaySFXRSE.Dispatch("Stop");
        m_LoadSceneRSE.Dispatch("Arena");
    }

    private IEnumerator StartNext()
    {
        Debug.Log("Start next QTE");

        m_BallAnimator.SetTrigger("Success");
        yield return new WaitForSeconds(2f);

        CreateQTE();
    }

    private void OnSceneLoaded(string scene)
    {
        if (scene == "MiniGame_QTE")
            StartCoroutine(ShowIntroPanel());
    }

    private IEnumerator ShowIntroPanel()
    {
        yield return new WaitForSeconds(1f);
        m_IntroAnimator.SetTrigger("Start");
        yield return new WaitForSeconds(1f);

        //StartCoroutine(TestAnims());
        CreateQTE();
    }


    private void CreateQTE()
    {
        Debug.Log("QTEManager.CreateQTE");

        SetRandomInput();
        UpdateQTE();
        m_KeyPressed = 0;
        m_IsListening = true;
        m_Countdown = 0f;
    }

    private void SetRandomInput()
    {
        m_RandomInput = UnityEngine.Random.Range(0, 4);
    }

    private void UpdateQTE()
    {
        m_InputText.text = m_UsedInputs[m_RandomInput];
    }

    private void ClearQTE()
    {
        m_InputText.text = "";
    }

    private void OnButton1(InputAction.CallbackContext ctx)
    {
        if (m_IsListening)
        {
            if (m_RandomInput == 0)
            {
                // Success
                m_KeyPressed = 1;
            }
            else
            {
                // Fail
                m_KeyPressed = 2;
            }
        }
    }

    private void OnButton2(InputAction.CallbackContext ctx)
    {
        if (m_IsListening)
        {
            if (m_RandomInput == 1)
            {
                // Success
                m_KeyPressed = 1;
            }
            else
            {
                // Fail
                m_KeyPressed = 2;
            }
        }
    }

    private void OnButton3(InputAction.CallbackContext ctx)
    {
        if (m_IsListening)
        {
            if (m_RandomInput == 2)
            {
                // Success
                m_KeyPressed = 1;
            }
            else
            {
                // Fail
                m_KeyPressed = 2;
            }
        }
    }

    private void OnButton4(InputAction.CallbackContext ctx)
    {
        if (m_IsListening)
        {
            if (m_RandomInput == 3)
            {
                // Success
                m_KeyPressed = 1;
            }
            else
            {
                // Fail
                m_KeyPressed = 2;
            }
        }
    }

    private void OnInputChanged(InputType newInputType)
    {
        if (newInputType == InputType.Keyboard)
            m_UsedInputs = m_KeyboardInputs;
        else if (newInputType == InputType.Gamepad)
            m_UsedInputs = m_GamepadInputs;
    }
}
