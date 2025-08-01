using UnityEngine;
using UnityEngine.Events;

public class TimeGate : MonoBehaviour
{
    public bool isActive = true;
    
    [Header("Components")]
    public Animator animator;

    #region Unity Events
    [Header("UnityEvents")]
    public UnityEvent<KartController> onCartPass; // TODO: replace 'Transform' with the car component
    public UnityEvent onActivate;
    public UnityEvent onDeactivate;
    #endregion

    public void OnCartPass(KartController controller)
    {
        SetActive(false);
        onCartPass?.Invoke(controller);
    }

    public void SetActive(bool state = true)
    {
        bool oldState = state;
        isActive = state;
        animator.SetBool("isActive", state);
        if (oldState != state)
        {
            if (isActive) onActivate?.Invoke();
            else onDeactivate?.Invoke();
        }
    }
}
