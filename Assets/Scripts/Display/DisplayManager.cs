using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Rendering;

public class DisplayManager : MonoBehaviour
{

    [SerializeField] private GameObject mSnakeHeadPrefab;
    [SerializeField] private GameObject mSnakeBodyPrefab;
    [SerializeField] private List<GameObject> mItemPrefabs;

    private int[] mSizeMap = { 8, 16 };
    private Vector3 sNewGameHeadPosition = new Vector3(4, 0, 4);
    private Vector3 sNewGameBody1Position = new Vector3(4, 0, 3);
    private Vector3 sNewGameBody2Position = new Vector3(4, 0, 2);
    private Vector3 sNewGameBody3Position = new Vector3(4, 0, 1);
    private List<SnakePart> mSnakeParts;
    private IEnumerator mCoroutine;
    //RSO
    [SerializeField] private RSO_Source mSourceRSO;
    [SerializeField] private RSO_SnakePositions mSnakePosRSO;
    [SerializeField] private RSO_Direction mSnakeDirectionRSO;
    [SerializeField] private RSO_Speed mSnakeSpeedRSO;
    [SerializeField] private RSO_HasMoved mSnakeHasMovedRSO;
    // RSE
    [SerializeField] private RSE_EndGame mEndRSE;
    [SerializeField] private RSE_MG_Success mSuccessRSE;

    private void OnEnable()
    {
        //Debug.Log("DisplayManager OnEnable");
        mSnakeParts = new List<SnakePart>();

        switch (mSourceRSO.Value)
        {
            case GameSource.NewGame:
                Debug.Log("New Game");
                InitSnake();
                
                break;
            case GameSource.MiniGame:
                Debug.Log("Return from mini game");
                // Retrieve snake positions in RSO, instantiate the parts
                RetrieveSnake();

                break;
        }

        // Instantiate new Item
        int randomItem = UnityEngine.Random.Range(0, mItemPrefabs.Count);
        Debug.Log("Item n°" + randomItem + " = " + mItemPrefabs[randomItem].tag);
        Instantiate(mItemPrefabs[randomItem], GenerateItemPosition(), Quaternion.identity);

        mSnakeDirectionRSO.OnChanged += OnDirectionChanged;
        mEndRSE.Event += PauseSnake;
        mSuccessRSE.Event += GrowSnake;

        // Start moving
        mSnakeHasMovedRSO.Value = false;
        mCoroutine = MoveCoroutine();
        StartCoroutine(mCoroutine);

    }

    public void OnDisable()
    {
        StopCoroutine(mCoroutine);
        mSnakeDirectionRSO.OnChanged -= OnDirectionChanged;
        mEndRSE.Event -= PauseSnake;
        mSuccessRSE.Event -= GrowSnake;
    }

    private IEnumerator MoveCoroutine()
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

    private void GrowSnake()
    {
        Debug.Log("Grow Snake");

        int lastIdx = mSnakePosRSO.Value.Count - 1;

        float newX = mSnakePosRSO.Value[lastIdx].x + 
            (mSnakePosRSO.Value[lastIdx].x - mSnakePosRSO.Value[lastIdx-1].x);
        float newZ = mSnakePosRSO.Value[lastIdx].z +
            (mSnakePosRSO.Value[lastIdx].z - mSnakePosRSO.Value[lastIdx - 1].z);

        Vector3 newPosition = new Vector3(newX, mSnakePosRSO.Value[lastIdx].y, newZ);

        Debug.Log("New part at " + newPosition);

        if (newPosition.x < 0 || newPosition.x > mSizeMap[0])
        {
            newPosition.x = mSnakePosRSO.Value[lastIdx].x;
            if (newPosition.z < mSizeMap[1] / 2)
                newPosition.z++;
            else
                newPosition.z--;

            Debug.Log("Corrected position at " + newPosition);
        }
        if (newPosition.z < 0 || newPosition.z > mSizeMap[1])
        {
            newPosition.z = mSnakePosRSO.Value[lastIdx].z;
            if (newPosition.x < mSizeMap[0] / 2)
                newPosition.x++;
            else
                newPosition.x--;

            Debug.Log("Corrected position at " + newPosition);
        }

        SnakePart newBodyPart = new SnakePart(
            Instantiate(mSnakeBodyPrefab, newPosition, Quaternion.identity),
            newPosition,
            Quaternion.identity);

        mSnakeParts.Add(newBodyPart);
        mSnakePosRSO.Value.Add(newBodyPart.Position);
    }


    private void InitSnake()
    {
        // Initialize new Snake: Head + 3 body parts
        SnakePart headPart = new SnakePart(
            Instantiate(mSnakeHeadPrefab, sNewGameHeadPosition, Quaternion.Euler(0, (float)mSnakeDirectionRSO.Value, 0)),
            sNewGameHeadPosition,
            Quaternion.Euler(0, (float)mSnakeDirectionRSO.Value, 0));

        SnakePart bodyPart1 = new SnakePart(
            Instantiate(mSnakeBodyPrefab, sNewGameBody1Position, Quaternion.identity),
            sNewGameBody1Position,
            Quaternion.identity);

        SnakePart bodyPart2 = new SnakePart(
            Instantiate(mSnakeBodyPrefab, sNewGameBody2Position, Quaternion.identity),
            sNewGameBody2Position,
            Quaternion.identity);

        SnakePart bodyPart3 = new SnakePart(
            Instantiate(mSnakeBodyPrefab, sNewGameBody3Position, Quaternion.identity),
            sNewGameBody3Position,
            Quaternion.identity);

        mSnakeParts.Add(headPart);
        mSnakeParts.Add(bodyPart1);
        mSnakeParts.Add(bodyPart2);
        mSnakeParts.Add(bodyPart3);

        // Create RSO Snake
        CreateRSO_Snake();
    }

    private void RetrieveSnake()
    {
        // Head
        Quaternion headRotation = Quaternion.Euler(0, (float)mSnakeDirectionRSO.Value, 0);
        SnakePart headPart = new SnakePart(
            Instantiate(mSnakeHeadPrefab, mSnakePosRSO.Value[0], headRotation),
            mSnakePosRSO.Value[0],
            headRotation);

        mSnakeParts.Add(headPart);

        // Body
        for (int i = 1; i < mSnakePosRSO.Value.Count; i++)
        {
            SnakePart bodyPart = new SnakePart(
            Instantiate(mSnakeBodyPrefab, mSnakePosRSO.Value[i], Quaternion.identity),
            mSnakePosRSO.Value[i],
            Quaternion.identity);
            mSnakeParts.Add(bodyPart);
        }
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

    private Vector3 GenerateItemPosition()
    {
        Vector3 itemPosition = Vector3.zero;
        bool positionIsGood = false;
        int minXZ = 1;
        int maxX = 8;
        int maxZ = 16;
        float randomX;
        float randomZ;

        while (!positionIsGood)
        {
            randomX = UnityEngine.Random.Range(minXZ, maxX);
            randomZ = UnityEngine.Random.Range(minXZ, maxZ);
            itemPosition = new Vector3(randomX, 0, randomZ);
            Debug.Log("Random position: " + itemPosition);
            if (!mSnakePosRSO.Value.Contains(itemPosition))
            {
                Debug.Log("Valid random postion");
                positionIsGood = true;
            }
        }

        Debug.Log("Return " + itemPosition);

        return itemPosition;
    }

}
