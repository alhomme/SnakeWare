using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RSO_SnakePositions", menuName = "SnakeRSO/RSO SnakePositons")]
public class RSO_SnakePositions : RuntimeScriptableObject<List<Vector3Int>>
{
}
