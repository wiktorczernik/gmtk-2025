using System;
using UnityEngine;

public class checkpiont : MonoBehaviour
{
    public static event Action RaiseTimeEvent;


    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Car vehicle))
        {
            RaiseTimeEvent?.Invoke();
        }
    }
}
