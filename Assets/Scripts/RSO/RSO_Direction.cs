using System;
using UnityEngine;

public enum SnakeDirection
{
    UP = 180, DOWN = 0, LEFT = 90, RIGHT = 270
}

[CreateAssetMenu(fileName = "RSO_Direction", menuName = "SnakeRSO/RSO Direction")]
public class RSO_Direction : ScriptableObject
{
    private SnakeDirection mDirection = SnakeDirection.RIGHT;
    public event Action<SnakeDirection> OnChanged; 

    public SnakeDirection Value
    {
        get { return mDirection; }
        set {
            mDirection = value;
            OnChanged?.Invoke(mDirection);
        }
    }
}
