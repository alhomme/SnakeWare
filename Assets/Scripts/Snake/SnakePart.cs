using UnityEngine;

public class SnakePart
{
    private GameObject mInstance;
    private Vector3Int mPosition;
    private Quaternion mRotation;

    public SnakePart(GameObject instance, Vector3Int position, Quaternion rotation)
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

    public Vector3Int Position
    {
        get { return mPosition; }
        set { mPosition = value; }
    }

    public int PositionX
    {
        get { return mPosition.x; }
        set { mPosition.x = value; }
    }

    public int PositionZ
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
