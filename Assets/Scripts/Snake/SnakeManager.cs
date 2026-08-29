using UnityEngine;

public class SnakeManager : MonoBehaviour
{
    [SerializeField] private RSE_Collision mCollisionEvent;
    private void OnTriggerEnter(Collider other)
    {
        // Send Event when colliding
        Debug.Log("Collision with " + other.tag);
        mCollisionEvent.Dispatch(other.tag);
    }
}
