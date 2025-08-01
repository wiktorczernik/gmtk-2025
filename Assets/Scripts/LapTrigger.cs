using UnityEngine;

public class LapTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        GameManager.TriggerLapCompletion();
    }
}
