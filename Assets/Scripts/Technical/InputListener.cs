using UnityEngine;
using UnityEngine.InputSystem;

public class InputListener : MonoBehaviour
{
    [SerializeField] private RSO_InputType m_InputTypeRSO;

    [SerializeField] private InputActionReference m_Input;

    private void OnEnable()
    {
        m_InputTypeRSO.Value = InputType.Keyboard;
        m_Input.action.performed += OnInput;
    }

    private void OnDisable()
    {
        m_Input.action.performed -= OnInput;
    }

    private void OnInput(InputAction.CallbackContext ctx)
    {
        if (ctx.control.device is Keyboard)
        {
            m_InputTypeRSO.Value = InputType.Keyboard;
        }
        else if (ctx.control.device is Gamepad)
        {
            m_InputTypeRSO.Value = InputType.Gamepad;
        }
    }
}
