using System;
using UnityEngine;

public enum SnakeDirection
{
    UP = 180, DOWN = 0, LEFT = 90, RIGHT = 270
}

[CreateAssetMenu(fileName = "RSO_Direction", menuName = "SnakeRSO/RSO Direction")]
public class RSO_Direction : RuntimeScriptableObject<SnakeDirection>
{
}
