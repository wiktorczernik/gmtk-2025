using System;
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
    [SerializeField] private float maxRotationSpeed;
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
        currentRotation = Mathf.Lerp(currentRotation, maxRotationSpeed*direction, rotationAcceleration * Time.deltaTime);
    }

    private void FixedUpdate()
    {
        //Apply speed
        sphere.linearVelocity = kartModel.transform.forward * currentSpeed;
        //Apply rotation
        kartModel.transform.eulerAngles += new Vector3(0,currentRotation,0);
    }
}
