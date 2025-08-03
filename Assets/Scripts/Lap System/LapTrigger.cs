using UnityEngine;

public class LapTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (LapManager.currentCheckpointGate != LapManager.maxCheckpointGate) return;
        FMODUnity.RuntimeManager.PlayOneShot("{82f43cde-efd8-4b17-8836-22aca763656d}");
        GameManager.TriggerLapCompletion();
    }
}
