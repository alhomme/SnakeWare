using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class SnakeManager : MonoBehaviour
{
    private int[] m_SizeMap = { 8, 16 };
    private Vector3Int m_NewGameHeadPosition = new Vector3Int(4, 0, 4);

    [SerializeField] private RSO_SnakePositions m_SnakePositionsRSO;

    [SerializeField] private RSE_NewGame m_NewGameRSE;
    [SerializeField] private RSE_Move m_MoveRSE;
    [SerializeField] private RSE_MG_Success m_SuccessRSE;

    private void OnEnable()
    {
        m_NewGameRSE.Event += OnNewGame;
        m_MoveRSE.Event += OnSnakeMove;
        m_SuccessRSE.Event += OnSuccess;

        m_SnakePositionsRSO.Value = new List<Vector3Int>();
    }

    private void OnDisable()
    {
        m_NewGameRSE.Event -= OnNewGame;
        m_MoveRSE.Event -= OnSnakeMove;
        m_SuccessRSE.Event -= OnSuccess;
    }

    private void OnNewGame()
    {
        m_SnakePositionsRSO.Value.Clear();

        m_SnakePositionsRSO.Value.Add(m_NewGameHeadPosition);
        for (int i = 1; i < 4; i++)
        {
            Vector3Int bodyPosition = m_NewGameHeadPosition;
            bodyPosition.z -= i;
            m_SnakePositionsRSO.Value.Add(bodyPosition);
        }
    }

    private void OnSnakeMove(List<SnakePart> newPositions)
    {
        for (int i = 0; i < newPositions.Count; i++)
        {
            m_SnakePositionsRSO.Value[i] = newPositions[i].Position;
        }
    }

    private void OnSuccess()
    {
        int lastIdx = m_SnakePositionsRSO.Value.Count - 1;

        if (lastIdx < 0)
            return ;

        int newX = m_SnakePositionsRSO.Value[lastIdx].x +
            (m_SnakePositionsRSO.Value[lastIdx].x - m_SnakePositionsRSO.Value[lastIdx - 1].x);
        int newZ = m_SnakePositionsRSO.Value[lastIdx].z +
            (m_SnakePositionsRSO.Value[lastIdx].z - m_SnakePositionsRSO.Value[lastIdx - 1].z);

        Vector3Int newPosition = new Vector3Int(newX, m_SnakePositionsRSO.Value[lastIdx].y, newZ);

        //Debug.Log("New part at " + newPosition);

        // Correct the position if it's outside the map
        if (newPosition.x < 0 || newPosition.x > m_SizeMap[0])
        {
            newPosition.x = m_SnakePositionsRSO.Value[lastIdx].x;
            if (newPosition.z < m_SizeMap[1] / 2)
                newPosition.z++;
            else
                newPosition.z--;

            //Debug.Log("Corrected position at " + newPosition);
        }
        if (newPosition.z < 0 || newPosition.z > m_SizeMap[1])
        {
            newPosition.z = m_SnakePositionsRSO.Value[lastIdx].z;
            if (newPosition.x < m_SizeMap[0] / 2)
                newPosition.x++;
            else
                newPosition.x--;

            //Debug.Log("Corrected position at " + newPosition);
        }

        m_SnakePositionsRSO.Value.Add(newPosition);
    }
}
