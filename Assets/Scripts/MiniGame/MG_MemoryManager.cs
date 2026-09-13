using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class MG_MemoryManager : MonoBehaviour
{
    [SerializeField] private Animator m_BallAnimator;
    [SerializeField] private Animator m_IntroAnimator;
    [SerializeField] private GameObject m_GoalPanel;

    [SerializeField] private InputActionReference[] m_Inputs;
    [SerializeField] private TextMeshProUGUI[] m_InputsText;


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
    private int m_RandomInput;
    private List<int> m_Sequence;
    private int m_Count;
    private int m_GoalCount;


    private void OnEnable()
    {
        m_SceneLoadedRSE.Event += OnSceneLoaded;
        m_InputTypeRSO.OnChanged += OnInputChanged;

        m_Inputs[0].action.performed += OnButton1;
        m_Inputs[1].action.performed += OnButton2;
        m_Inputs[2].action.performed += OnButton3;
        m_Inputs[3].action.performed += OnButton4;

        m_RandomInput = 0;
        m_Sequence = new List<int>();
    }

    private void OnDisable()
    {
        m_SceneLoadedRSE.Event -= OnSceneLoaded;
        m_InputTypeRSO.OnChanged -= OnInputChanged;

        m_Inputs[0].action.performed -= OnButton1;
        m_Inputs[1].action.performed -= OnButton2;
        m_Inputs[2].action.performed -= OnButton3;
        m_Inputs[3].action.performed -= OnButton4;

        StopAllCoroutines();
    }

    private void Start()
    {
        if (m_InputTypeRSO.Value == InputType.Keyboard)
            m_UsedInputs = m_KeyboardInputs;
        else if (m_InputTypeRSO.Value == InputType.Gamepad)
            m_UsedInputs = m_GamepadInputs;

        m_IsListening = false;

        // Use RSO_Speed
        m_Count = 0;
        m_GoalCount = 2 + (int)m_SpeedRSO.Value;

        //OnSceneLoaded("MiniGame_Memory");
    }


    private void OnSceneLoaded(string scene)
    {
        if (scene == "MiniGame_Memory")
            StartCoroutine(ShowIntroPanel());
    }

    private IEnumerator ShowIntroPanel()
    {
        yield return new WaitForSeconds(1f);
        m_IntroAnimator.SetTrigger("Start");
        yield return new WaitForSeconds(1f);

        StartCoroutine(StartSequence());
    }

    private IEnumerator StartSequence()
    {
        for (int i = 0; i < m_GoalCount; i++)
        {
            m_RandomInput = UnityEngine.Random.Range(0, 4);
            m_Sequence.Add(m_RandomInput);

            m_InputsText[m_RandomInput].text = m_UsedInputs[m_RandomInput];
            yield return new WaitForSeconds(0.7f);
            m_InputsText[m_RandomInput].text = "";
            yield return new WaitForSeconds(0.1f);
        }

        // Whistle sfx
        m_PlaySFXRSE.Dispatch("Whistle");
        Utils.LogList(m_Sequence);
        m_IsListening = true;
    }

    private IEnumerator MG_Success()
    {
        Utils.Log("MiniGame Success");
        m_IsListening = false;

        // Play Success sfx

        ClearInputsText();

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
        Utils.Log("MiniGame Fail");
        m_IsListening = false;

        // Add error sfx

        ClearInputsText();

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

    private void ClearInputsText()
    {
        foreach (var input in m_InputsText)
        {
            input.text = "";
        }
    }


    private void OnButton1(InputAction.CallbackContext ctx)
    {
        if (m_IsListening)
        {
            ClearInputsText();
            m_InputsText[0].text = m_UsedInputs[0];

            if (m_Sequence[m_Count] == 0)
            {
                // Success
                m_Count++;
                if (m_Count == m_GoalCount)
                    StartCoroutine(MG_Success());

            }
            else
            {
                // Fail
                StartCoroutine(MG_Fail());
            }
        }
    }

    private void OnButton2(InputAction.CallbackContext ctx)
    {
        if (m_IsListening)
        {
            ClearInputsText();
            m_InputsText[1].text = m_UsedInputs[1];

            if (m_Sequence[m_Count] == 1)
            {
                // Success
                m_Count++;
                if (m_Count == m_GoalCount)
                    StartCoroutine(MG_Success());

            }
            else
            {
                // Fail
                StartCoroutine(MG_Fail());
            }
        }
    }

    private void OnButton3(InputAction.CallbackContext ctx)
    {
        if (m_IsListening)
        {
            ClearInputsText();
            m_InputsText[2].text = m_UsedInputs[2];
            if (m_Sequence[m_Count] == 2)
            {
                // Success
                m_Count++;
                if (m_Count == m_GoalCount)
                    StartCoroutine(MG_Success());

            }
            else
            {
                // Fail
                StartCoroutine(MG_Fail());
            }
        }
    }

    private void OnButton4(InputAction.CallbackContext ctx)
    {
        if (m_IsListening)
        {
            ClearInputsText();
            m_InputsText[3].text = m_UsedInputs[3];
            if (m_Sequence[m_Count] == 3)
            {
                // Success
                m_Count++;
                if (m_Count == m_GoalCount)
                    StartCoroutine(MG_Success());

            }
            else
            {
                // Fail
                StartCoroutine(MG_Fail());
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
