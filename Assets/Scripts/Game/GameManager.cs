using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public InputActionReference moveUpActionRef;
    public InputActionReference moveDownActionRef;
    public InputActionReference moveRightActionRef;
    public InputActionReference moveLeftActionRef;

    public float defaultSpeed = 1.0f;

    [SerializeField] private RSO_Source mSourceRSO;
    [SerializeField] private RSO_Direction mSnakeDirectionRSO;
    [SerializeField] private RSO_Speed mSnakeSpeedRSO;

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
                mSnakeSpeedRSO.Value = defaultSpeed;

                break;
            case GameSource.MiniGame:
                // Retrieve result
                break;
        }
    }

    private void OnDisable()
    {
        moveUpActionRef.action.performed -= OnMoveUp;
        moveDownActionRef.action.performed -= OnMoveDown;
        moveRightActionRef.action.performed -= OnMoveRight;
        moveLeftActionRef.action.performed -= OnMoveLeft;
    }

    private void OnMoveUp(InputAction.CallbackContext ctx)
    {
        if (mSnakeDirectionRSO.Value == SnakeDirection.RIGHT
            || mSnakeDirectionRSO.Value == SnakeDirection.LEFT)
        {
            mSnakeDirectionRSO.Value = SnakeDirection.UP;
        }
    }

    private void OnMoveDown(InputAction.CallbackContext ctx)
    {
        if (mSnakeDirectionRSO.Value == SnakeDirection.RIGHT
            || mSnakeDirectionRSO.Value == SnakeDirection.LEFT)
        {
            mSnakeDirectionRSO.Value = SnakeDirection.DOWN;
        }
    }

    private void OnMoveRight(InputAction.CallbackContext ctx)
    {
        if (mSnakeDirectionRSO.Value == SnakeDirection.UP
            || mSnakeDirectionRSO.Value == SnakeDirection.DOWN)
        {
            mSnakeDirectionRSO.Value = SnakeDirection.RIGHT;
        }
    }

    private void OnMoveLeft(InputAction.CallbackContext ctx)
    {
        if (mSnakeDirectionRSO.Value == SnakeDirection.UP
            || mSnakeDirectionRSO.Value == SnakeDirection.DOWN)
        {
            mSnakeDirectionRSO.Value = SnakeDirection.LEFT;
        }
    }
}
