using UnityEngine;

public enum GameSource
{
    NewGame, MiniGame
}

[CreateAssetMenu(fileName = "RSO_Source", menuName = "SnakeRSO/RSO Source")]
public class RSO_Source : ScriptableObject
{
    private GameSource mSource = GameSource.NewGame;

    public GameSource Value
    {  
       get { return mSource; } 
       set { mSource = value; }
    }
}
