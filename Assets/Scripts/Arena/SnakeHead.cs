using UnityEngine;
using UnityEngine.SceneManagement;

public class SnakeHead : MonoBehaviour
{
    [SerializeField] private RSE_Collision m_CollisionRSE;
    [SerializeField] private RSE_LoadScene m_LoadSceneRSE;

    private void OnTriggerEnter(Collider other)
    {
        // Send Event when colliding
        Debug.Log("Collision with " + other.tag);
        m_CollisionRSE.Dispatch(other.tag);

        if (other.tag.StartsWith("MiniGame"))
        {
            // Launch Mini Game
            
        }
    }
}
