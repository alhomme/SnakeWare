using UnityEngine;

[CreateAssetMenu(fileName = "RSO_Speed", menuName = "SnakeRSO/RSO Speed")]
public class RSO_Speed : ScriptableObject
{
    private float mSpeed = 1f;

    public float Value
    {
        get { return mSpeed; }
        set { mSpeed = value; }
    }
}
