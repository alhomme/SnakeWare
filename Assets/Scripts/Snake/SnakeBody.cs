using UnityEngine;
using static UnityEditor.FilePathAttribute;

public enum toremoveSnakeDirection
{
    UP = -90, DOWN = 90, LEFT = 180, RIGHT = 0
}

public class SnakeBody
{
    private bool mIsHead;
    private GameObject mObject;
    private Vector3 mPosition;
    private SnakeDirection mDirection;
    private Quaternion mRotation;

    public SnakeBody(Vector3 position, bool isHead = false, SnakeDirection direction = SnakeDirection.RIGHT)
    {
        mIsHead = isHead;
        mPosition = position;
        SetDirection(direction);
    }

    public void Move(float speed, SnakeBody front)
    {
        Debug.Log("Direction = " + mDirection);
        mObject.transform.Translate(speed * Time.deltaTime * new Vector3(1, 0, 0));
        mObject.transform.rotation = mRotation;
        if (!mIsHead)
        {
            // Rotate according to the next body part
            SetDirection(front.GetDirection());
        }
    }

    public bool IsHead()
    {
        return mIsHead;
    }

    public SnakeDirection GetDirection()
    {
        return mDirection;
    }

    public void SetDirection(SnakeDirection direction)
    {
        mDirection = direction;
        mRotation = Quaternion.Euler(0, (float)mDirection, 0);
    }

    public Quaternion GetRotation()
    {
        return mRotation;
    }

    public Vector3 GetPosition()
    {
        return mPosition;
    }

    public GameObject GetGameObject()
    {
        return mObject;
    }

    public void SetGameObject(GameObject go)
    {
        mObject = go;
    }


}
