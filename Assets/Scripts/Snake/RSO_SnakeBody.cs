using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RSO_SnakeBody", menuName = "Snake/RSO SnakeBody")]
public class RSO_SnakeBody : ScriptableObject
{
    private LinkedList<SnakeBody> mSnakeBody = new LinkedList<SnakeBody>();

    public LinkedList<SnakeBody> Body
    {
        get { return mSnakeBody; }
    }

    public SnakeBody Head
    {
        get { return mSnakeBody.Last.Value; }
    }

    public SnakeDirection HeadDirection
    {
        get { return mSnakeBody.Last.Value.GetDirection(); }
    }

    public void AddBody(SnakeBody body)
    {
        mSnakeBody.AddFirst(body);
    }
}
