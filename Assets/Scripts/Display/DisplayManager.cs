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

    private int[] mSizeMap = { 8, 16 };
    private Vector3Int sNewGameHeadPosition = new Vector3Int(4, 0, 4);
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
    [SerializeField] private RSE_NewRound mNewRoundRSE;
    // SSO
    [SerializeField] private SSO_Items mItemsSSO;

    private void OnEnable()
    {
        //Debug.Log("DisplayManager OnEnable");
        mSnakeDirectionRSO.OnChanged += OnDirectionChanged;
        mEndRSE.Event += PauseSnake;
        mNewRoundRSE.Event += StartNewRound;

        mCoroutine = MoveCoroutine();
    }

    public void OnDisable()
    {
        StopCoroutine(mCoroutine);
        mSnakeDirectionRSO.OnChanged -= OnDirectionChanged;
        mEndRSE.Event -= PauseSnake;
        mNewRoundRSE.Event -= StartNewRound;
    }

    private void Start()
    {
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
        //int randomItem = UnityEngine.Random.Range(0, mItemsSSO.Value.Count);
        int randomItem = 0;
        Debug.Log("Item n°" + randomItem + " = " + mItemsSSO.Value[randomItem].tag);
        Instantiate(mItemsSSO.Value[randomItem], GenerateItemPosition(), Quaternion.identity);        
    }

    private void PauseSnake()
    {
        // Stop snake from moving more
        StopCoroutine(mCoroutine);
    }

    private void StartNewRound(bool growSnake)
    {
        if (growSnake)
        {
            Debug.Log("Grow Snake");

            int lastIdx = mSnakePosRSO.Value.Count - 1;

            int newX = mSnakePosRSO.Value[lastIdx].x +
                (mSnakePosRSO.Value[lastIdx].x - mSnakePosRSO.Value[lastIdx - 1].x);
            int newZ = mSnakePosRSO.Value[lastIdx].z +
                (mSnakePosRSO.Value[lastIdx].z - mSnakePosRSO.Value[lastIdx - 1].z);

            Vector3Int newPosition = new Vector3Int(newX, mSnakePosRSO.Value[lastIdx].y, newZ);

            //Debug.Log("New part at " + newPosition);

            if (newPosition.x < 0 || newPosition.x > mSizeMap[0])
            {
                newPosition.x = mSnakePosRSO.Value[lastIdx].x;
                if (newPosition.z < mSizeMap[1] / 2)
                    newPosition.z++;
                else
                    newPosition.z--;

                //Debug.Log("Corrected position at " + newPosition);
            }
            if (newPosition.z < 0 || newPosition.z > mSizeMap[1])
            {
                newPosition.z = mSnakePosRSO.Value[lastIdx].z;
                if (newPosition.x < mSizeMap[0] / 2)
                    newPosition.x++;
                else
                    newPosition.x--;

                //Debug.Log("Corrected position at " + newPosition);
            }

            SnakePart newBodyPart = new SnakePart(
                Instantiate(mSnakeBodyPrefab, newPosition, Quaternion.identity),
                newPosition,
                Quaternion.identity);

            mSnakeParts.Add(newBodyPart);
            mSnakePosRSO.Value.Add(newBodyPart.Position);
        }


        // Start moving the snake
        mSnakeHasMovedRSO.Value = true;
        StartCoroutine(mCoroutine);
    }

    private IEnumerator MoveCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(1 / mSnakeSpeedRSO.Value);

            //Debug.Log("New Movement");
            Vector3Int frontPosition = Vector3Int.zero;

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
                frontPosition.x = (int)part.Instance.transform.position.x;
                frontPosition.z = (int)part.Instance.transform.position.z;
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

    private void InitSnake()
    {
        // Initialize new Snake: Head + 3 body parts
        SnakePart headPart = new SnakePart(
            Instantiate(mSnakeHeadPrefab, sNewGameHeadPosition, Quaternion.Euler(0, (float)mSnakeDirectionRSO.Value, 0)),
            sNewGameHeadPosition,
            Quaternion.Euler(0, (float)mSnakeDirectionRSO.Value, 0));

        mSnakeParts.Add(headPart);

        for (int i = 1; i < 4; i++)
        {
            Vector3Int bodyPosition = sNewGameHeadPosition;
            bodyPosition.z -= i;
            SnakePart bodyPart = new SnakePart(
                Instantiate(mSnakeBodyPrefab, bodyPosition, Quaternion.identity),
                bodyPosition,
                Quaternion.identity);

            mSnakeParts.Add(bodyPart);
        }

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
        Vector3Int itemPosition = Vector3Int.zero;
        bool positionIsGood = false;
        int randomX;
        int randomZ;

        while (!positionIsGood)
        {
            randomX = UnityEngine.Random.Range(1, mSizeMap[0]);
            randomZ = UnityEngine.Random.Range(1, mSizeMap[1]);
            itemPosition = new Vector3Int(randomX, 0, randomZ);
            //Debug.Log("Random position: " + itemPosition);
            if (!mSnakePosRSO.Value.Contains(itemPosition))
            {
                //Debug.Log("Valid random postion");
                positionIsGood = true;
            }
        }

        Debug.Log("Item position: " + itemPosition);

        return itemPosition;
    }

}
