using UnityEngine;

public class KartController : MonoBehaviour
{
    [Header("Steering Settings")]
    public AnimationCurve speedToSteeringCurve;
    public float steeringAngleSpeed = 0.2f;
    public float steeringLerp = 2f;

    [Header("Ground Checking Settings")]
    public float groundMaxDistance = 0.5f;
    public float groundCheckRadius = 1.2f;

    [Header("State")]
    public Quaternion steeringCurrentRot;
    public Quaternion steeringTargetRot;
    public float yaw => sphere.transform.eulerAngles.y;
    public float groundedForwardSpeed => Vector3.Dot(sphere.linearVelocity, groundedForward);
    public Vector3 worldForward => Quaternion.Euler(0, yaw, 0) * Vector3.forward;
    public Vector3 groundedForward => Vector3.ProjectOnPlane(worldForward, groundNormal);
    public Vector3 groundNormal = Vector3.up;
    public bool isGrounded = true;

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
        /*
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
        */
    }

    private void FixedUpdate()
    {
        float forwardSpeed = groundedForwardSpeed;
        RaycastHit hitInfo;

        bool hit = Physics.SphereCast(sphere.position, groundCheckRadius, Vector3.down, out hitInfo, groundMaxDistance);
        if (hit)
        {
            isGrounded = true;
            groundNormal = hitInfo.normal;
        }
        else
        {
            isGrounded = false;
            groundNormal = Vector3.up;
        }

        sphere.AddForce(-groundedForward * forwardSpeed, ForceMode.VelocityChange);

        Quaternion steeringTargetRot = Quaternion.Euler(Input.GetAxisRaw("Horizontal") * steeringAngleSpeed * speedToSteeringCurve.Evaluate(forwardSpeed) * groundNormal * Time.fixedDeltaTime);
        Vector3 steeringDeltaAngles = steeringTargetRot.eulerAngles;
        steeringDeltaAngles.x = 0;
        steeringDeltaAngles.z = 0;
        steeringTargetRot = Quaternion.Euler(steeringDeltaAngles);
        steeringCurrentRot = Quaternion.Lerp(steeringCurrentRot, steeringTargetRot, steeringLerp * Time.fixedDeltaTime);
        sphere.MoveRotation(sphere.rotation * steeringCurrentRot);

        sphere.AddForce(groundedForward * forwardSpeed, ForceMode.VelocityChange);

        bool canAccelerate = groundedForwardSpeed < maxSpeed;
        bool canDecelerate = groundedForwardSpeed > maxReverseSpeed;

        float verticalInput = Input.GetAxisRaw("Vertical");
        float force = 0;
        if (verticalInput > 0 && canAccelerate)
            force = acceleration;
        else if (verticalInput < 0 && canDecelerate)
            force = deceleration;
        force *= Time.fixedDeltaTime;

        Debug.Log($"Normal: {groundNormal}; Forward: {groundedForward}; VInput: {verticalInput};");
        sphere.AddForce(groundedForward * force, ForceMode.VelocityChange);

        if (isGrounded && Vector3.Angle(Vector3.up, groundNormal) > 0.1f)
        {
            sphere.AddForce(-Physics.gravity, ForceMode.Acceleration);
        }
        /*
        //Apply rotation
        Vector3 forwardDirection = Quaternion.AngleAxis(currentRotation, Vector3.up) * kartModel.transform.forward;

        float rotationLerpSpeed = 0.1f;
        //Apply speed
        Vector3 newVelocity = Vector3.Lerp(kartModel.transform.forward * currentSpeed, forwardDirection * currentSpeed, rotationLerpSpeed);
        newVelocity.y = sphere.linearVelocity.y;
        sphere.linearVelocity = newVelocity;

        if (sphere.linearVelocity.magnitude > 0.1f)
        {
            kartModel.transform.eulerAngles = Quaternion.LookRotation(sphere.linearVelocity.normalized).eulerAngles;
        }
        */
    }

    //Draw fortward d
    void OnDrawGizmos()
    {
        /*
        Gizmos.color = Color.green;
        Vector3 origin = kartModel.transform.position;

        Vector3 forwardDirection = Quaternion.AngleAxis(currentRotation, Vector3.up) * kartModel.transform.forward;
        // Draw angle arms
        Gizmos.DrawLine(origin, origin + forwardDirection * 10);
        */
    }
}
