using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class DirectionManager : MonoBehaviour
{
    private bool canMove;

    [SerializeField] private InputActionReference m_UpInput;
    [SerializeField] private InputActionReference m_DownInput;
    [SerializeField] private InputActionReference m_RightInput;
    [SerializeField] private InputActionReference m_LeftInput;

    [SerializeField] private RSO_Direction m_DirectionRSO;

    [SerializeField] private RSE_NewGame m_NewGameRSE;
    [SerializeField] private RSE_Death m_DeathRSE;
    [SerializeField] private RSE_SceneLoaded m_SceneLoadedRSE;
    [SerializeField] private RSE_Move m_MoveRSE;

    private void OnEnable()
    {
        canMove = true;

        m_NewGameRSE.Event += OnNewGame;
        m_DeathRSE.Event += OnDeath;
        m_SceneLoadedRSE.Event += OnSceneLoaded;
        m_MoveRSE.Event += OnSnakeMove;
    }

    private void OnDisable()
    {
        m_NewGameRSE.Event -= OnNewGame;
        m_DeathRSE.Event -= OnDeath;
        m_SceneLoadedRSE.Event -= OnSceneLoaded;
        m_MoveRSE.Event -= OnSnakeMove;
    }

    private void OnNewGame()
    {
        m_DirectionRSO.Value = SnakeDirection.RIGHT;
    }

    private void OnDeath()
    {
        DisableMoveInputs();
    }

    private void OnSceneLoaded(string scene)
    {
        if (scene == "Arena")
            EnableMoveInputs();
        else
            DisableMoveInputs();
    }

    private void OnSnakeMove(List<SnakePart> newPositions)
    {
        canMove = true;
    }

    private void EnableMoveInputs()
    {
        m_UpInput.action.performed += OnMoveUp;
        m_DownInput.action.performed += OnMoveDown;
        m_RightInput.action.performed += OnMoveRight;
        m_LeftInput.action.performed += OnMoveLeft;
    }

    private void DisableMoveInputs()
    {
        m_UpInput.action.performed -= OnMoveUp;
        m_DownInput.action.performed -= OnMoveDown;
        m_RightInput.action.performed -= OnMoveRight;
        m_LeftInput.action.performed -= OnMoveLeft;
    }

    private void OnMoveUp(InputAction.CallbackContext ctx)
    {
        if (canMove
            && (m_DirectionRSO.Value == SnakeDirection.RIGHT
            || m_DirectionRSO.Value == SnakeDirection.LEFT))
        {
            m_DirectionRSO.Value = SnakeDirection.UP;
            canMove = false;
        }
    }

    private void OnMoveDown(InputAction.CallbackContext ctx)
    {
        if (canMove
            && (m_DirectionRSO.Value == SnakeDirection.RIGHT
            || m_DirectionRSO.Value == SnakeDirection.LEFT))
        {
            m_DirectionRSO.Value = SnakeDirection.DOWN;
            canMove = false;
        }
    }

    private void OnMoveRight(InputAction.CallbackContext ctx)
    {
        if (canMove
            && (m_DirectionRSO.Value == SnakeDirection.UP
            || m_DirectionRSO.Value == SnakeDirection.DOWN))
        {
            m_DirectionRSO.Value = SnakeDirection.RIGHT;
            canMove = false;
        }
    }

    private void OnMoveLeft(InputAction.CallbackContext ctx)
    {
        if (canMove
            && (m_DirectionRSO.Value == SnakeDirection.UP
            || m_DirectionRSO.Value == SnakeDirection.DOWN))
        {
            m_DirectionRSO.Value = SnakeDirection.LEFT;
            canMove = false;
        }
    }
}
