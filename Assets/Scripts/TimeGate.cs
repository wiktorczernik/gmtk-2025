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
    public UnityEvent<Transform> OnCartPass; // TODO: replace 'Transform' with the car component
    public UnityEvent OnActivate;
    public UnityEvent OnDeactivate;
    #endregion

    private void OnTriggerEnter(Collider other)
    {
        if (!isActive) return;
        SetActive(false);

        OnCartPass?.Invoke(other.GetComponent<Transform>()); // TODO: replace 'Transform' with the car component

        // TODO: if the collider is a clone, do not add the time.
        //       So e.g. if (other.GetComponent<CARCOMPONENT>().isClone) return;

        TimerController.Instance.time += timeBonus;
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
