using System;
using UnityEngine;

public class Finish : MonoBehaviour
{
    public static event Action OnFinishReached;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Car vehicle))
        {
            OnFinishReached?.Invoke();
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
