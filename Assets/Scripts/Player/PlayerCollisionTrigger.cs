using UnityEngine;

public class PlayerCollisionTrigger : MonoBehaviour
{

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("LapTrigger"))
        {
            GameManager.TriggerLapCompletion();
        }
    }
}