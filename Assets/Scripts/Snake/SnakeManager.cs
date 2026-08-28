using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class SnakeManager : MonoBehaviour
{
    public InputActionReference moveUpActionRef;
    public InputActionReference moveDownActionRef;
    public InputActionReference moveRightActionRef;
    public InputActionReference moveLeftActionRef;

    public GameObject snakeBodyPrefab;

    [SerializeField]
    private RSO_SnakeBody mSnakeBody;

    private bool mMove = false;

    private void Awake()
    {
        // Début du jeu
        // Créer un snakeBody avec la tête
        SnakeBody head = new SnakeBody(transform.position, true);
        head.SetGameObject(this.gameObject);
        mSnakeBody.AddBody(head);

        // Instancie 2 body parts derrière la tête
        SnakeBody body1 = new SnakeBody(new Vector3(transform.position.x - 1, transform.position.y, transform.position.z));
        mSnakeBody.AddBody(body1);

        SnakeBody body2 = new SnakeBody(new Vector3(transform.position.x - 2, transform.position.y, transform.position.z));
        mSnakeBody.AddBody(body2);
    }



    private void OnEnable()
    {
        Debug.Log("SnakeManager OnEnable");
        moveUpActionRef.action.performed += OnMoveUp;
        moveDownActionRef.action.performed += OnMoveDown;
        moveRightActionRef.action.performed += OnMoveRight;
        moveLeftActionRef.action.performed += OnMoveLeft;

        mMove = true;

        // Déplacer la tête à sa position enregistrée
        // Instantiate les body parts ici
        foreach(SnakeBody b in mSnakeBody.Body)
        {
            if (b.IsHead())
            {
                transform.position = b.GetPosition();
            }
            else
            {
                b.SetGameObject(Instantiate(snakeBodyPrefab, b.GetPosition(), b.GetRotation()));
            }
        }
    }

    private void OnDisable()
    {
        moveUpActionRef.action.performed -= OnMoveUp;
        moveDownActionRef.action.performed -= OnMoveDown;
        moveRightActionRef.action.performed -= OnMoveRight;
        moveLeftActionRef.action.performed -= OnMoveLeft;
    }

    private void Update()
    {
        int count = 0;
        if (mMove)
        {
            //Vector3 snakeDirection = new Vector3(1,0,0);
            //transform.Translate(1.5f * Time.deltaTime * snakeDirection);
            //Move from last body part to head
            for (LinkedListNode<SnakeBody> node = mSnakeBody.Body.First; node != null; node = node.Next)
            {
                Debug.Log("count = " + count);
                count++;
                if (node.Next != null)
                    node.Value.Move(2f, node.Next.Value);
                else
                    node.Value.Move(2f, null);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger");
        mMove = false;
    }

    private void OnMoveUp(InputAction.CallbackContext ctx)
    {
        if (mSnakeBody.HeadDirection == SnakeDirection.RIGHT
            || mSnakeBody.HeadDirection == SnakeDirection.LEFT)
        {
            mSnakeBody.Body.Last.Value.SetDirection(SnakeDirection.UP);
        }
    }

    private void OnMoveDown(InputAction.CallbackContext ctx)
    {
        if (mSnakeBody.HeadDirection == SnakeDirection.RIGHT
            || mSnakeBody.HeadDirection == SnakeDirection.LEFT)
        {
            mSnakeBody.Body.Last.Value.SetDirection(SnakeDirection.DOWN);
        }
    }

    private void OnMoveRight(InputAction.CallbackContext ctx)
    {
        if (mSnakeBody.HeadDirection == SnakeDirection.UP
            || mSnakeBody.HeadDirection == SnakeDirection.DOWN)
        {
            mSnakeBody.Body.Last.Value.SetDirection(SnakeDirection.RIGHT);
        }
    }

    private void OnMoveLeft(InputAction.CallbackContext ctx)
    {
        if (mSnakeBody.HeadDirection == SnakeDirection.UP
            || mSnakeBody.HeadDirection == SnakeDirection.DOWN)
        {
            mSnakeBody.Body.Last.Value.SetDirection(SnakeDirection.LEFT);
        }
    }
}
