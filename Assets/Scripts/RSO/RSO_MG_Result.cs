using UnityEngine;

public enum MG_Result
{
    Fail, Success
}

[CreateAssetMenu(fileName = "RSO_MG_Result", menuName = "SnakeRSO/RSO MG Result")]
public class RSO_MG_Result : RuntimeScriptableObject<MG_Result>
{
}
