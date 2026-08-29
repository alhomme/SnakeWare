using UnityEngine;

public enum GameSource
{
    NewGame, MiniGame
}

[CreateAssetMenu(fileName = "RSO_Source", menuName = "SnakeRSO/RSO Source")]
public class RSO_Source : RuntimeScriptableObject<GameSource>
{
}
