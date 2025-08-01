using UnityEngine;

public class TimeGateTrigger : MonoBehaviour
{
    TimeGate parent;
    
    private void Awake()
    {
        parent = GetComponentInParent<TimeGate>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!parent.isActive) return;
        if (!other.CompareTag("Player")) return;
        parent.OnCartPass(other.GetComponent<KartController>());
    }
}
