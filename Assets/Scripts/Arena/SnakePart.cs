using UnityEngine;

public class SnakePart
{
    private GameObject m_Instance;
    private Vector3Int m_Position;
    private Vector3Int m_NextPosition;

    public SnakePart(GameObject instance, Vector3Int position, Vector3Int nextPosition)
    {
        m_Instance = instance;
        m_Position = position;
        m_NextPosition = nextPosition;
    }

    public GameObject Instance
    {
        get { return m_Instance; }
        set { m_Instance = value; }
    }

    public Vector3Int Position
    {
        get { return m_Position; }
        set { m_Position = value; }
    }

    public int PositionX
    {
        get { return m_Position.x; }
        set { m_Position.x = value; }
    }

    public int PositionZ
    {
        get { return m_Position.z; }
        set { m_Position.z = value; }
    }

    public Vector3Int NextPosition
    {
        get { return m_NextPosition; }
        set { m_NextPosition = value; }
    }
}
