using UnityEngine;

public class SnakeManager : MonoBehaviour
{
    [SerializeField] private RSE_Collision mCollisionRSE;
    private void OnTriggerEnter(Collider other)
    {
        // Send Event when colliding
        Debug.Log("Collision with " + other.tag);
        mCollisionRSE.Dispatch(other.tag);
    }
}
