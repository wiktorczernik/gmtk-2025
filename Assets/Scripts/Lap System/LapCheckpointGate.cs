using System.Collections;
using UnityEngine;

public class LapCheckpointGate : MonoBehaviour
{
    #region Variables
    [SerializeField] bool _isActive = true;
    #endregion

    #region Public Variables
    public bool isActive
    {
        get => _isActive;
        set => _isActive = value;
    }
    #endregion

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (!isActive) return;

        StartCoroutine(TriggerLapGate());
    }

    private IEnumerator TriggerLapGate()
    {
        LapManager.currentCheckpointGate++;

        isActive = false;
        yield return null;
    }
}
