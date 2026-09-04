using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class SnakeMove : MonoBehaviour
{
    private int[] m_SizeMap = { 8, 16 };
    private IEnumerator m_Coroutine;
    private List<SnakePart> m_SnakeParts;
    private GameObject m_ItemInstance;

    [SerializeField] private GameObject m_SnakeHeadPrefab;
    [SerializeField] private GameObject m_SnakeBodyPrefab;

    [SerializeField] private SSO_Items m_ItemsSSO;

    [SerializeField] private RSO_Direction m_DirectionRSO;
    [SerializeField] private RSO_Speed m_SpeedRSO;
    [SerializeField] private RSO_SnakePositions m_SnakePositionsRSO;

    [SerializeField] private RSE_Move m_MoveRSE;
    [SerializeField] private RSE_Collision m_CollisionRSE;
    [SerializeField] private RSE_Death m_DeathRSE;
    [SerializeField] private RSE_LoadScene m_LoadSceneRSE;

    private void OnEnable()
    {
        m_DirectionRSO.OnChanged += OnDirectionChanged;
        m_CollisionRSE.Event += OnCollision;
        m_DeathRSE.Event += OnDeath;

        m_Coroutine = MoveCoroutine();
    }

    private void OnDisable()
    {
        Debug.Log("SnakeMove.OnDisable");

        StopCoroutine(m_Coroutine);
        m_DirectionRSO.OnChanged -= OnDirectionChanged;
        m_CollisionRSE.Event -= OnCollision;
        m_DeathRSE.Event -= OnDeath;
    }

    private void Start()
    {
        // Instantiate Snake
        InstantiateSnake();
        // Instantiate Item
        InstantiateItem();
        // Start Movement
        StartCoroutine(StartRoundCoroutine());
    }

    private IEnumerator StartRoundCoroutine()
    {
        // Wait 0.5 seconds before starting to move
        // Allows player to get ready
        // TODO: Add a "Ready ? GO!" Panel
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(m_Coroutine);
    }

    private IEnumerator MoveCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(1 / m_SpeedRSO.Value);

            for (int i = 0; i < m_SnakeParts.Count; i++)
            {
                if (i == 0)
                {
                    // Head
                    switch (m_DirectionRSO.Value)
                    {
                        case SnakeDirection.UP:
                            m_SnakeParts[i].PositionX -= 1;
                            break;
                        case SnakeDirection.DOWN:
                            m_SnakeParts[i].PositionX += 1;
                            break;
                        case SnakeDirection.RIGHT:
                            m_SnakeParts[i].PositionZ += 1;
                            break;
                        case SnakeDirection.LEFT:
                            m_SnakeParts[i].PositionZ -= 1;
                            break;
                    }
                }
                else
                {
                    // Body
                    m_SnakeParts[i].Position = m_SnakeParts[i].NextPosition;
                    m_SnakeParts[i].NextPosition = m_SnakeParts[i - 1].Position;
                }

                m_SnakeParts[i].Instance.transform.position = m_SnakeParts[i].Position;
            }

            m_MoveRSE.Dispatch(m_SnakeParts);
        }
    }

    private void InstantiateSnake()
    {
        m_SnakeParts = new List<SnakePart>();

        // Head
        GameObject headInstance = Instantiate(
            m_SnakeHeadPrefab,
            m_SnakePositionsRSO.Value[0],
            Quaternion.Euler(0, (float)m_DirectionRSO.Value, 0));

        SnakePart headPart = new SnakePart(
            headInstance,
            m_SnakePositionsRSO.Value[0],
            Vector3Int.zero);
        m_SnakeParts.Add(headPart);

        // Body
        for (int i = 1; i < m_SnakePositionsRSO.Value.Count; i++)
        {
            GameObject bodyInstance = Instantiate(
                m_SnakeBodyPrefab,
                m_SnakePositionsRSO.Value[i],
                Quaternion.identity);

            SnakePart bodyPart = new SnakePart(
                bodyInstance,
                m_SnakePositionsRSO.Value[i],
                m_SnakeParts[i-1].Position);

            m_SnakeParts.Add(bodyPart);
        }
    }

    private void InstantiateItem()
    {
        int randomItem = UnityEngine.Random.Range(0, m_ItemsSSO.Value.Count);
        Debug.Log("Item n°" + randomItem + " = " + m_ItemsSSO.Value[randomItem].tag);
        m_ItemInstance = Instantiate(m_ItemsSSO.Value[randomItem], GenerateItemPosition(), Quaternion.identity);
    }

    private Vector3 GenerateItemPosition()
    {
        Vector3Int itemPosition = Vector3Int.zero;
        bool positionIsGood = false;
        int randomX;
        int randomZ;

        while (!positionIsGood)
        {
            randomX = UnityEngine.Random.Range(1, m_SizeMap[0]);
            randomZ = UnityEngine.Random.Range(1, m_SizeMap[1]);
            itemPosition = new Vector3Int(randomX, 0, randomZ);
            //Debug.Log("Random position: " + itemPosition);
            if (!m_SnakePositionsRSO.Value.Contains(itemPosition))
            {
                //Debug.Log("Valid random postion");
                positionIsGood = true;
            }
        }

        Debug.Log("Item position: " + itemPosition);
        return itemPosition;
    }

    private void OnDirectionChanged(SnakeDirection direction)
    {
        m_SnakeParts[0].Instance.transform.rotation = Quaternion.Euler(0, (float)direction, 0);
    }

    private void OnDeath()
    {
        StopCoroutine(m_Coroutine);
    }

    private void OnCollision(string other)
    {
        Debug.Log("SnakeMove.OnCollision: " + other);

        StopCoroutine(m_Coroutine);

        if (other.StartsWith("MiniGame"))
        {
            m_LoadSceneRSE.Dispatch(other);
        }
    }
}
