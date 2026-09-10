using UnityEngine;

[CreateAssetMenu(fileName = "RSO_InputType", menuName = "SnakeRSO/RSO Input Type")]
public class RSO_InputType : RuntimeScriptableObject<InputType>
{
}

public enum InputType
{
    Keyboard,
    Gamepad
}
