using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RSO_SnakePositions", menuName = "SnakeRSO/RSO SnakePositons")]
public class RSO_SnakePositions : ScriptableObject
{
    private List<Vector3> mSnake = new List<Vector3>();

    public List<Vector3> Value
    {
        get { return mSnake; }
    }

}
