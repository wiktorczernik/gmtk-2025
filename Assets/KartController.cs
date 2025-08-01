using UnityEditor.U2D;
using UnityEngine;

public class KartController : MonoBehaviour, ICloneable
{
    [Header("Collision Settings")]
    public float colliderRadius = 1.2f;

    [Header("Steering Settings")]
    public AnimationCurve speedToSteeringCurve;
    public float steeringAngleSpeed;
    public float normalSteeringAngleSpeed;
    public float driftSteeringAngleSpeed;
    public float steeringLerp = 2f;
    [SerializeField] private bool isDrifting;
    [SerializeField] private float driftDir;
    [SerializeField] private float horizontalAxis;
    [SerializeField] private float driftAngle;
    [SerializeField] private float targetDriftAngle;
    [SerializeField] private float driftAngleLerp;

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
    public Vector3 spinAxis = Vector3.zero;
    public float spinSpeed = 0f;

    [Header("Components")]
    [SerializeField] private Rigidbody sphere;
    [SerializeField] private GameObject kartModel;
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



    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            sphere.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);
        }

        horizontalAxis = Input.GetAxisRaw("Horizontal");

        if (isGrounded && Input.GetKey(KeyCode.LeftShift) && !isDrifting && horizontalAxis != 0 && Input.GetKey(KeyCode.W))
        {
            isDrifting = true;
            driftDir = horizontalAxis;
        }

        if (!Input.GetKey(KeyCode.LeftShift) || !Input.GetKey(KeyCode.W))
        {
            isDrifting = false;
            driftDir = 0;
        }

        //Align kart to ground
        driftAngle = Mathf.Lerp(driftAngle, targetDriftAngle * driftDir, driftAngleLerp * Time.deltaTime);

        if (isDrifting)
        {
            steeringAngleSpeed = driftSteeringAngleSpeed;
        }
        else
        {
            steeringAngleSpeed = normalSteeringAngleSpeed;
        }

        float deg = spinSpeed * Mathf.Rad2Deg * Time.deltaTime;
        kartModel.transform.Rotate(spinAxis, deg, Space.World);
    }

    private void FixedUpdate()
    {
        float forwardSpeed = groundedForwardSpeed;
        RaycastHit hitInfo;
        bool hit = Physics.SphereCast(sphere.position + new Vector3(0, 0.1f), groundCheckRadius, Vector3.down, out hitInfo, groundMaxDistance);
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
            sphere.AddForce(Vector3.down * gravity, ForceMode.VelocityChange);
        }

        sphere.AddForce(-groundedForward * forwardSpeed, ForceMode.VelocityChange);

        if (isDrifting && horizontalAxis != driftDir)
        {
            horizontalAxis = 0;
        }

        Quaternion steeringTargetRot = Quaternion.Euler(horizontalAxis * steeringAngleSpeed * speedToSteeringCurve.Evaluate(forwardSpeed) * groundNormal * Time.fixedDeltaTime);
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

        sphere.AddForce(groundedForward * force, ForceMode.VelocityChange);

        if (isGrounded && Vector3.Angle(Vector3.up, groundNormal) > 0.1f)
        {
            sphere.AddForce(-Physics.gravity, ForceMode.Acceleration);
        }

        if (isGrounded)
        {
            Vector3 horVel = sphere.linearVelocity;
            horVel.y = 0;
            float horSpeed = horVel.magnitude;

            if (horSpeed > 0.01f)
            {
                spinAxis = Vector3.Cross(Vector3.up, horVel.normalized);
                spinSpeed = horSpeed / colliderRadius;
            }
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            sphere.linearVelocity = new Vector3(0, 0, 0);
            sphere.angularVelocity = new Vector3(0, 0, 0);
        }
    }
    public CloneFrameState GetFrameState()
    {
        CloneFrameState newFrame = new();

        newFrame.position = kartModel.transform.position;
        newFrame.rotation = kartModel.transform.rotation;

        return newFrame;
    }
}
