using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public InputActionReference moveUpActionRef;
    public InputActionReference moveDownActionRef;
    public InputActionReference moveRightActionRef;
    public InputActionReference moveLeftActionRef;

    private float mDefaultSpeed = 2.0f;

    [SerializeField] private RSO_Source mSourceRSO;
    [SerializeField] private RSO_Life mSnakeLifeRSO;
    [SerializeField] private RSO_Score mSnakeScoreRSO;
    [SerializeField] private RSO_Direction mSnakeDirectionRSO;
    [SerializeField] private RSO_Speed mSnakeSpeedRSO;
    [SerializeField] private RSO_HasMoved mSnakeHasMovedRSO;

    [SerializeField] private RSE_Collision mCollisionEvent;
    [SerializeField] private RSE_EndGame mEndEvent;

    private void OnEnable()
    {
        moveUpActionRef.action.performed += OnMoveUp;
        moveDownActionRef.action.performed += OnMoveDown;
        moveRightActionRef.action.performed += OnMoveRight;
        moveLeftActionRef.action.performed += OnMoveLeft;

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
                //mSnakeScoreRSO.Value += (int)(100 * mSnakeSpeedRSO.Value);
                //mSnakeSpeedRSO.Value += 0.5f;

                // Mini Game failed
                // mSnakeLifeRSO.Value--;
                break;
        }

        mCollisionEvent.Event += OnCollision;
        mEndEvent.Event += OnGameEnded;
    }

    private void OnDisable()
    {
        DisableMoveInputs();

        mCollisionEvent.Event -= OnCollision;
        mEndEvent.Event -= OnGameEnded;
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
        }
    }

    private void OnGameEnded()
    {
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
