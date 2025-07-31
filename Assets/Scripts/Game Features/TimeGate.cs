using UnityEngine;
using UnityEngine.Events;

public class TimeGate : MonoBehaviour
{
    #region Fields
    [Header("Fields")]
    [SerializeField] private float timeBonus = 5f;
    #endregion

    #region Variables
    [Header("Variables")]
    [SerializeField] private bool isActive = true;
    #endregion

    #region Public Variables
    [Header("Public Variables")]
    public UnityEvent<KartController> OnCartPass; // TODO: replace 'Transform' with the car component
    public UnityEvent OnActivate;
    public UnityEvent OnDeactivate;
    #endregion

    private void OnTriggerEnter(Collider other)
    {
        if (!isActive) return;
        if (!other.CompareTag("Player")) return;
        SetActive(false);

        OnCartPass?.Invoke(other.GetComponent<KartController>()); // TODO: replace 'Transform' with the car component

        // TODO: if the collider is a clone, do not add the time.
        //       So e.g. if (other.GetComponent<CARCOMPONENT>().isClone) return;

        TimerController.time += timeBonus;
    }

    public void SetActive(bool state = true)
    {
        bool oldState = state;
        isActive = state;
        if (oldState != state)
        {
            if (isActive) OnActivate?.Invoke();
            else OnDeactivate?.Invoke();
        }
    }
}
