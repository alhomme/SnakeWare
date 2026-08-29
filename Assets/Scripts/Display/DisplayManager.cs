using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class DisplayManager : MonoBehaviour
{

    public GameObject snakeHeadPrefab;
    public GameObject snakeBodyPrefab;


    private Vector3 sNewGameHeadPosition = new Vector3(4, 0, 3);
    private Vector3 sNewGameBody1Position = new Vector3(4, 0, 2);
    private Vector3 sNewGameBody2Position = new Vector3(4, 0, 1);
    private Vector3 sNewGameBody3Position = new Vector3(4, 0, 0);
    private List<SnakePart> mSnakeParts;
    private IEnumerator mCoroutine;

    [SerializeField] private RSO_Source mSourceRSO;
    [SerializeField] private RSO_SnakePositions mSnakePosRSO;
    [SerializeField] private RSO_Direction mSnakeDirectionRSO;
    [SerializeField] private RSO_Speed mSnakeSpeedRSO;
    [SerializeField] private RSO_HasMoved mSnakeHasMovedRSO;

    [SerializeField] private RSE_EndGame mEndEvent;

    private void OnEnable()
    {
        //Debug.Log("DisplayManager OnEnable");
        mSnakeParts = new List<SnakePart>();

        switch (mSourceRSO.Value)
        {
            case GameSource.NewGame:
                InitSnake();
                // Instantiate new Item
                break;
            case GameSource.MiniGame:
                // Retrieve snake positions in RSO, instantiate the parts

                // Set Head in correct direction
                break;
        }

        mSnakeDirectionRSO.OnChanged += OnDirectionChanged;
        mEndEvent.Event += PauseSnake;

        // Start moving
        mSnakeHasMovedRSO.Value = false;
        mCoroutine = Move_Coroutine();
        StartCoroutine(mCoroutine);

    }

    public void OnDisable()
    {
        StopCoroutine(mCoroutine);
        mSnakeDirectionRSO.OnChanged -= OnDirectionChanged;
        mEndEvent.Event -= PauseSnake;
    }

    private IEnumerator Move_Coroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(1 / mSnakeSpeedRSO.Value);

            //Debug.Log("New Movement");
            Vector3 frontPosition = Vector3.zero;

            foreach (SnakePart part in mSnakeParts)
            {
                if (part == mSnakeParts[0])
                {
                    // Head
                    //Debug.Log("Head current position = " + part.Position);

                    switch (mSnakeDirectionRSO.Value)
                    {
                        case SnakeDirection.UP:
                            part.PositionX -= 1;
                            break;
                        case SnakeDirection.DOWN:
                            part.PositionX += 1;
                            break;
                        case SnakeDirection.RIGHT:
                            part.PositionZ += 1;
                            break;
                        case SnakeDirection.LEFT:
                            part.PositionZ -= 1;
                            break;
                    }

                    //Debug.Log("Head new position = " + part.Position);
                }
                else
                {
                    // Body
                    //Debug.Log("Body current position = " + part.Position);
                    //Debug.Log("Body front position = " + frontPosition);
                    part.PositionX = frontPosition.x;
                    part.PositionZ = frontPosition.z;
                    //Debug.Log("Body new position = " + part.Position);
                }

                // Store so that next body part moves to front position
                frontPosition.x = part.Instance.transform.position.x;
                frontPosition.z = part.Instance.transform.position.z;
                part.Instance.transform.position = part.Position;

            }

            mSnakeHasMovedRSO.Value = true;
            UpdateRSO_Snake();
        }
    }

    private void OnDirectionChanged(SnakeDirection newDirection)
    {
        mSnakeHasMovedRSO.Value = false;
        mSnakeParts[0].Rotation = Quaternion.Euler(0, (float)newDirection, 0);
        mSnakeParts[0].Instance.transform.rotation = mSnakeParts[0].Rotation;
    }

    private void PauseSnake()
    {
        // Stop snake from moving more
        StopCoroutine(mCoroutine);
    }


    private void InitSnake()
    {
        // Initialize new Snake: Head + 3 body parts
        SnakePart headPart = new SnakePart(
            Instantiate(snakeHeadPrefab, sNewGameHeadPosition, Quaternion.Euler(0, (float)mSnakeDirectionRSO.Value, 0)),
            sNewGameHeadPosition,
            Quaternion.Euler(0, (float)mSnakeDirectionRSO.Value, 0));

        SnakePart bodyPart1 = new SnakePart(
            Instantiate(snakeBodyPrefab, sNewGameBody1Position, Quaternion.identity),
            sNewGameBody1Position,
            Quaternion.identity);

        SnakePart bodyPart2 = new SnakePart(
            Instantiate(snakeBodyPrefab, sNewGameBody2Position, Quaternion.identity),
            sNewGameBody2Position,
            Quaternion.identity);

        SnakePart bodyPart3 = new SnakePart(
            Instantiate(snakeBodyPrefab, sNewGameBody3Position, Quaternion.identity),
            sNewGameBody3Position,
            Quaternion.identity);

        mSnakeParts.Add(headPart);
        mSnakeParts.Add(bodyPart1);
        mSnakeParts.Add(bodyPart2);
        mSnakeParts.Add(bodyPart3);

        // Create RSO Snake
        CreateRSO_Snake();
    }


    private void UpdateRSO_Snake()
    {
        for (int i = 0; i < mSnakeParts.Count; ++i)
        {
            mSnakePosRSO.Value[i] = mSnakeParts[i].Position;
        }
    }

    private void CreateRSO_Snake()
    {
        mSnakePosRSO.Value.Clear();
        foreach (SnakePart part in mSnakeParts)
        {
            mSnakePosRSO.Value.Add(part.Position);
        }
    }

}
