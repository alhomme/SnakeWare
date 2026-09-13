using UnityEngine;
using UnityEngine.SceneManagement;

public class SnakeHead : MonoBehaviour
{
    [SerializeField] private RSE_Collision m_CollisionRSE;
    [SerializeField] private RSE_LoadScene m_LoadSceneRSE;

    private void OnTriggerEnter(Collider other)
    {
        Utils.Log("SnakeHead.OnTriggerEnter");
        // Send Event when colliding
        Utils.Log("Collision with " + other.tag);
        m_CollisionRSE.Dispatch(other.tag);
    }
}
