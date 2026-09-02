using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private InputActionReference moveUpActionRef;
    [SerializeField] private InputActionReference moveDownActionRef;
    [SerializeField] private InputActionReference moveRightActionRef;
    [SerializeField] private InputActionReference moveLeftActionRef;
    [SerializeField] private InputActionReference menuActionRef;

    [SerializeField] private AudioMixer mAudioMixer;

    private float mDefaultSpeed = 1.0f;
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

        menuActionRef.action.performed -= OnMenuAction;

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

        AdjustMusicPitch();


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

    private void AdjustMusicPitch()
    {
        float newPitch = 1 + ((mSnakeSpeedRSO.Value - 1) / 10);
        Debug.Log("GameManager.AdjustMusicPitch: New pitch = " + newPitch);
        mAudioMixer.SetFloat("MusicPitch", newPitch);
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
        else if (other.StartsWith("MiniGame"))
        {
            // Launch Mini Game
            SceneManager.LoadScene(other);
        }
    }

    private void OnGameEnded()
    {
        Debug.Log("GameManager: OnGameEnded");
        DisableMoveInputs();

        // Check for high score and save
        int currentHighScore = Int32.Parse(PlayerPrefs.GetString("HighScore", "0"));
        if (mSnakeScoreRSO.Value > currentHighScore)
        {
            string newHighScore = mSnakeScoreRSO.Value.ToString();
            PlayerPrefs.SetString("HighScore", newHighScore);
        }
        // Add listener on key to return to menu
        menuActionRef.action.performed += OnMenuAction;
    }

    private void OnMenuAction(InputAction.CallbackContext ctx)
    {
        SceneManager.LoadScene("MainMenu");
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
