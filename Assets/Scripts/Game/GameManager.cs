using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private InputActionReference moveUpActionRef;
    [SerializeField] private InputActionReference moveDownActionRef;
    [SerializeField] private InputActionReference moveRightActionRef;
    [SerializeField] private InputActionReference moveLeftActionRef;

    private float mDefaultSpeed = 2.0f;
    // RSO
    [SerializeField] private RSO_Source mSourceRSO;
    [SerializeField] private RSO_MG_Result mResultRSO;
    [SerializeField] private RSO_Life mSnakeLifeRSO;
    [SerializeField] private RSO_Score mSnakeScoreRSO;
    [SerializeField] private RSO_Direction mSnakeDirectionRSO;
    [SerializeField] private RSO_Speed mSnakeSpeedRSO;
    [SerializeField] private RSO_HasMoved mSnakeHasMovedRSO;
    // RSE
    [SerializeField] private RSE_Collision mCollisionRSE;
    [SerializeField] private RSE_EndGame mEndRSE;
    [SerializeField] private RSE_NewRound mNewRoundRSE;

    private void OnEnable()
    {
        Debug.Log("GameManager: OnEnable");

        moveUpActionRef.action.performed += OnMoveUp;
        moveDownActionRef.action.performed += OnMoveDown;
        moveRightActionRef.action.performed += OnMoveRight;
        moveLeftActionRef.action.performed += OnMoveLeft;

        mCollisionRSE.Event += OnCollision;
        mEndRSE.Event += OnGameEnded;
    }

    private void OnDisable()
    {
        DisableMoveInputs();

        mCollisionRSE.Event -= OnCollision;
        mEndRSE.Event -= OnGameEnded;
    }

    private void Start()
    {
        Debug.Log("GameManager: Start");
        bool growSnake = false;

        switch (mSourceRSO.Value)
        {
            case GameSource.NewGame:
                // Initialize all values
                mSnakeDirectionRSO.Value = SnakeDirection.RIGHT;
                mSnakeSpeedRSO.Value = mDefaultSpeed;
                mSnakeLifeRSO.Value = 3;
                mSnakeScoreRSO.Value = 0;
                break;
            case GameSource.MiniGame:
                // Retrieve result
                // Mini Game succeeded
                Debug.Log("Source Mini Game, " + mResultRSO.Value);
                if (mResultRSO.Value == MG_Result.Success)
                {
                    mSnakeScoreRSO.Value += 1; // (int)(100 * mSnakeSpeedRSO.Value);
                    mSnakeSpeedRSO.Value += 0.5f;
                    growSnake = true;
                }
                else if (mResultRSO.Value == MG_Result.Fail)
                {
                    // Mini Game failed
                    mSnakeLifeRSO.Value--;
                }

                // TO REMOVE, FOR TESTING PURPOSE
                // USED TO AVOID STARTING FROM MAIN MENU EVERY TIME
                mSourceRSO.Value = GameSource.NewGame;
                break;
        }


        StartCoroutine(StartRoundCoroutine(growSnake));
    }

    private IEnumerator StartRoundCoroutine(bool growSnake)
    {
        // Wait 0.5 seconds before starting the round
        // Allows player to get ready
        // TODO: Add a "Ready ? GO!" Panel
        yield return new WaitForSeconds(0.5f);
        if (mSnakeLifeRSO.Value > 0)
            mNewRoundRSE.Dispatch(growSnake);
    }



    private void DisableMoveInputs()
    {
        moveUpActionRef.action.performed -= OnMoveUp;
        moveDownActionRef.action.performed -= OnMoveDown;
        moveRightActionRef.action.performed -= OnMoveRight;
        moveLeftActionRef.action.performed -= OnMoveLeft;
    }

    private void OnCollision(string other)
    {
        Debug.Log("Event of collision with " + other);

        if (other == "Wall" || other == "Snake")
        {
            // Set life to 0
            mSnakeLifeRSO.Value = 0;
        }
        else if (other.StartsWith("Item"))
        {
            // Launch Mini Game
            switch (other)
            {
                case "ItemBar":
                    SceneManager.LoadScene(2);
                    break;
            }
        }
    }

    private void OnGameEnded()
    {
        Debug.Log("GameManager: OnGameEnded");
        DisableMoveInputs();

        // Check for high score and save

        // Add listener on key to return to menu
    }

    private void OnMoveUp(InputAction.CallbackContext ctx)
    {
        if (mSnakeHasMovedRSO.Value
            && (mSnakeDirectionRSO.Value == SnakeDirection.RIGHT
            || mSnakeDirectionRSO.Value == SnakeDirection.LEFT))
        {
            mSnakeDirectionRSO.Value = SnakeDirection.UP;
        }
    }

    private void OnMoveDown(InputAction.CallbackContext ctx)
    {
        if (mSnakeHasMovedRSO.Value
            && (mSnakeDirectionRSO.Value == SnakeDirection.RIGHT
            || mSnakeDirectionRSO.Value == SnakeDirection.LEFT))
        {
            mSnakeDirectionRSO.Value = SnakeDirection.DOWN;
        }
    }

    private void OnMoveRight(InputAction.CallbackContext ctx)
    {
        if (mSnakeHasMovedRSO.Value
            && (mSnakeDirectionRSO.Value == SnakeDirection.UP
            || mSnakeDirectionRSO.Value == SnakeDirection.DOWN))
        {
            mSnakeDirectionRSO.Value = SnakeDirection.RIGHT;
        }
    }

    private void OnMoveLeft(InputAction.CallbackContext ctx)
    {
        if (mSnakeHasMovedRSO.Value
            && (mSnakeDirectionRSO.Value == SnakeDirection.UP
            || mSnakeDirectionRSO.Value == SnakeDirection.DOWN))
        {
            mSnakeDirectionRSO.Value = SnakeDirection.LEFT;
        }
    }
}
