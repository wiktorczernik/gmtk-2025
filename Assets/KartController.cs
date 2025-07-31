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
    [SerializeField] private float jumpForce;
    [SerializeField] private bool isDrifting;


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            sphere.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);
        }

        isDrifting = (Input.GetKey(KeyCode.LeftShift) && isGrounded);

        //Align kart to ground
        kartModel.transform.position = transform.position - new Vector3(0, kartModelYModifier);
        kartModel.transform.rotation = Quaternion.Lerp(kartModel.transform.rotation, Quaternion.FromToRotation(kartModel.transform.up, groundNormal) * kartModel.transform.rotation, 0.1f);
        kartModel.transform.localEulerAngles = new Vector3(kartModel.transform.localEulerAngles.x, 0, kartModel.transform.localEulerAngles.z);
    }

    private void FixedUpdate()
    {
        float forwardSpeed = groundedForwardSpeed;
        RaycastHit hitInfo;
        Debug.Log(isGrounded);
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

        //Apply gravity if not grounded
        if (!isGrounded)
        {
            sphere.AddForce(Vector3.down*gravity,ForceMode.VelocityChange);
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
    }
}
