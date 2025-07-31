using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class KartController : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Rigidbody sphere;
    [SerializeField] private GameObject kartModel;
    [SerializeField] private float kartModelYModifier;
    [Header("Speed")]
    [SerializeField] private float maxSpeed;
    [SerializeField] private float maxReverseSpeed;
    [SerializeField] private float currentSpeed;
    [SerializeField] private float deceleration;
    [SerializeField] private float groundforce;
    [SerializeField] private float acceleration;
    [Header("Rotation")]
    [SerializeField] private float maxRotationAngle;
    [SerializeField] private float currentRotation;
    [SerializeField] private float rotationAcceleration;
    [SerializeField] private float direction;
    [Header("Other")]
    [SerializeField] private float gravity;
    void Update()
    {
        //Set kart model position
        kartModel.transform.position = sphere.transform.position - new Vector3(0, kartModelYModifier, 0);
        //Acceleration
        if (Input.GetKey(KeyCode.W))
        {
            currentSpeed += acceleration * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.S)) //Deceleration
        {
            currentSpeed -= deceleration * Time.deltaTime;
        }
        else //No input, car returns to 0 speed
        {
            if (currentSpeed < 0)
            {
                if (currentSpeed + groundforce * Time.deltaTime > 0)
                {
                    currentSpeed = 0;
                }
                else
                {
                    currentSpeed += groundforce * Time.deltaTime;
                }
            }
            else if (currentSpeed > 0)
            {
                if (currentSpeed - groundforce * Time.deltaTime < 0)
                {
                    currentSpeed = 0;
                }
                else
                {
                    currentSpeed -= groundforce * Time.deltaTime;
                }
            }
        }
        currentSpeed = Math.Clamp(currentSpeed, -maxReverseSpeed, maxSpeed);
        //Handling
        direction = Input.GetAxisRaw("Horizontal");
        currentRotation = Mathf.Lerp(currentRotation, maxRotationAngle * direction, rotationAcceleration * Time.deltaTime);


    }

    private void FixedUpdate()
    {
        //Apply rotation
        Vector3 forwardDirection = Quaternion.AngleAxis(currentRotation, Vector3.up) * kartModel.transform.forward;

        float rotationLerpSpeed = 0.1f;
        //Apply speed
        sphere.linearVelocity = Vector3.Lerp(kartModel.transform.forward * currentSpeed, forwardDirection * currentSpeed, rotationLerpSpeed);

        if (sphere.linearVelocity.magnitude > 0.1f)
        {
            kartModel.transform.eulerAngles = Quaternion.LookRotation(sphere.linearVelocity.normalized).eulerAngles;
        }
        
    }

    //Draw fortward d
    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Vector3 origin = kartModel.transform.position;

        Vector3 forwardDirection = Quaternion.AngleAxis(currentRotation, Vector3.up) * kartModel.transform.forward;
        // Draw angle arms
        Gizmos.DrawLine(origin, origin + forwardDirection * 10);
    }
}
