using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class MG_ComboManager : MonoBehaviour
{
    [SerializeField] private Animator m_SnakeAnimator;
    [SerializeField] private Animator m_IntroAnimator;

    [SerializeField] private TextMeshProUGUI m_CountdownText;
    [SerializeField] private TextMeshProUGUI m_OldInputs;
    [SerializeField] private TextMeshProUGUI m_NextInputs;
    [SerializeField] private TextMeshProUGUI m_CurrentInput;

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
    private bool m_IsListening;
    private List<int> m_Sequence;
    private int m_SequenceLength;
    private int m_InputCount;
    private int m_ButtonPressed;
    private float m_Countdown;

    private void OnEnable()
    {
        m_SceneLoadedRSE.Event += OnSceneLoaded;
        m_InputTypeRSO.OnChanged += OnInputChanged;

        m_Inputs[0].action.performed += OnButton1;
        m_Inputs[1].action.performed += OnButton2;
        m_Inputs[2].action.performed += OnButton3;
        m_Inputs[3].action.performed += OnButton4;

        m_IsListening = false;
        m_Sequence = new List<int>();

        OnInputChanged(m_InputTypeRSO.Value);
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

    private void OnSceneLoaded(string scene)
    {
        if (scene == "MiniGame_Combo")
            StartCoroutine(ShowIntroPanel());
    }

    private IEnumerator ShowIntroPanel()
    {
        yield return new WaitForSeconds(1f);
        m_IntroAnimator.SetTrigger("Start");
        yield return new WaitForSeconds(1f);

        // Create Sequence
        CreateSequence();

        StartCoroutine(Countdown());
    }

    private IEnumerator Countdown()
    {
        m_SnakeAnimator.SetTrigger("Start");
        m_CountdownText.text = "3";
        yield return new WaitForSeconds(1f);
        m_CountdownText.text = "2";
        yield return new WaitForSeconds(1f);
        m_CountdownText.text = "1";
        yield return new WaitForSeconds(1f);
        m_CountdownText.text = "";

        m_ButtonPressed = -1;
        m_Countdown = 0f;
        m_IsListening = true;
    }

    private void Update()
    {
        if (m_IsListening)
        {
            m_Countdown += Time.deltaTime;
            if (m_Countdown >= 0.5f)
            {
                // Fail
                StartCoroutine(MG_Fail());
                m_IsListening = false;
            }

            if (m_ButtonPressed >= 0)
            {
                if (m_ButtonPressed == m_Sequence[m_InputCount])
                {
                    m_InputCount++;
                    UpdateUI();
                    
                    if (m_InputCount == m_SequenceLength)
                    {
                        // Success
                        StartCoroutine(MG_Success());
                        m_IsListening = false;
                    }
                }
                else
                {
                    // Fail
                    StartCoroutine(MG_Fail());
                    m_IsListening = false;
                }
                m_ButtonPressed = -1;
                m_Countdown = 0f;
            }
        }
    }

    private IEnumerator MG_Success()
    {
        Debug.Log("MiniGame Success");

        m_SnakeAnimator.SetTrigger("Success");
        //yield return new WaitForSeconds(1f);

        m_PlaySFXRSE.Dispatch("CrowdGoal");
        yield return new WaitForSeconds(2f);

        // Return to Arena
        m_SuccessRSE.Dispatch();
        m_PlaySFXRSE.Dispatch("Stop");
        m_LoadSceneRSE.Dispatch("Arena");
    }

    private IEnumerator MG_Fail()
    {
        Debug.Log("MiniGame Fail");

        m_SnakeAnimator.SetTrigger("Fail");
        //yield return new WaitForSeconds(0.5f);

        m_PlaySFXRSE.Dispatch("CrowdBoo");
        yield return new WaitForSeconds(3f);

        // Return to Arena
        m_FailRSE.Dispatch();
        m_PlaySFXRSE.Dispatch("Stop");
        m_LoadSceneRSE.Dispatch("Arena");
    }

    private void CreateSequence()
    {
        m_SequenceLength = 3 + (int)(m_SpeedRSO.Value / 0.5f);

        int randomInput;

        for (int i = 0; i < m_SequenceLength; i++)
        {
            randomInput = UnityEngine.Random.Range(0, 4);
            m_Sequence.Add(randomInput);
        }

        m_InputCount = 0;

        PrintSequence(m_Sequence);
        UpdateUI();
    }

    private void UpdateUI()
    {
        m_OldInputs.text = "";
        m_CurrentInput.text = "";
        m_NextInputs.text = "";

        // Inputs already pressed
        for (int i = 0; i < m_InputCount; i++)
        {
            m_OldInputs.text += m_UsedInputs[m_Sequence[i]];
        }

        // Current Input
        if (m_InputCount < m_SequenceLength)
            m_CurrentInput.text = m_UsedInputs[m_Sequence[m_InputCount]];

        // Inputs to come
        for (int i = m_InputCount + 1; i < m_SequenceLength; i++)
        {
            m_NextInputs.text += m_UsedInputs[m_Sequence[i]];
        }
    }

    private void OnButton1(InputAction.CallbackContext ctx)
    {
        if (m_IsListening)
        {
            m_ButtonPressed = 0;
        }
    }

    private void OnButton2(InputAction.CallbackContext ctx)
    {
        if (m_IsListening)
        {
            m_ButtonPressed = 1;
        }
    }

    private void OnButton3(InputAction.CallbackContext ctx)
    {
        if (m_IsListening)
        {
            m_ButtonPressed = 2;
        }
    }

    private void OnButton4(InputAction.CallbackContext ctx)
    {
        if (m_IsListening)
        {
            m_ButtonPressed = 3;
        }
    }

    private void OnInputChanged(InputType newInputType)
    {
        if (newInputType == InputType.Keyboard)
            m_UsedInputs = m_KeyboardInputs;
        else if (newInputType == InputType.Gamepad)
            m_UsedInputs = m_GamepadInputs;
    }

    private void PrintSequence(List<int> sequence)
    {
        string str = "Sequence = { ";

        foreach (int i in sequence)
        {
            str += i.ToString();
            str += " ";
        }
        str += "}";

        Debug.Log(str);
    }
}
