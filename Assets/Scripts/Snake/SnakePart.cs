using UnityEngine;

public class SnakePart
{
    private GameObject mInstance;
    private Vector3 mPosition;
    private Quaternion mRotation;

    public SnakePart(GameObject instance, Vector3 position, Quaternion rotation)
    {
        mInstance = instance;
        mPosition = position;
        mRotation = rotation;
    }

    public GameObject Instance
    {
        get { return mInstance; }
        set { mInstance = value; }
    }

    public Vector3 Position
    {
        get { return mPosition; }
        set { mPosition = value; }
    }

    public float PositionX
    {
        get { return mPosition.x; }
        set { mPosition.x = value; }
    }

    public float PositionY
    {
        get { return mPosition.y; }
        set { mPosition.y = value; }
    }

    public float PositionZ
    {
        get { return mPosition.z; }
        set { mPosition.z = value; }
    }

    public Quaternion Rotation
    {
        get { return mRotation; }
        set { mRotation = value; }
    }
}
