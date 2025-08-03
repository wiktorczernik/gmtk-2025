using FMOD.Studio;
using FMODUnity;
using System;
using Unity.VisualScripting;
using UnityEngine;

public class KartController : MonoBehaviour, ICloneable
{
    [Header("Audio")]
    public EventReference kartSoundRef;
    public EventReference kartDriftSoundRef;
    public EventReference kartImpactSoundRef;

    [Header("Lap Settings")]
    public KartLapConfig[] lapConfigs;

    [Header("Steering Settings")]
    public AnimationCurve speedToSteeringCurve;
    public float steeringAngleSpeed;
    public float normalSteeringAngleSpeed;
    public float driftSteeringAngleSpeed;
    public float steeringLerp = 2f;
    [SerializeField] private bool isDrifting;
    [SerializeField] private float driftDir;
    [SerializeField] private float modelYawTilt;

    [Header("Model Settings")]
    [SerializeField] private float targetSteerAngle = 15;
    [SerializeField] private float targetDriftAngle = 45;
    [SerializeField] private float yawTiltLerp = 2;

    [Header("Control Settings")]
    public KeyCode driftKey = KeyCode.Space;

    [Header("Ground Checking Settings")]
    public float groundMaxDistance = 0.5f;
    public float groundCheckRadius = 1.2f;

    [Header("Input State")]
    public float steeringInput = 0.0f;
    public float throttleInput = 0.0f;
    public bool driftInput = false;

    private EventInstance kartAudioInstance;
    private EventInstance kartDriftAudioInstance;

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
    [SerializeField] private TrailRenderer leftTrail;
    [SerializeField] private TrailRenderer rightTrail;
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
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float bumpForce;
    [SerializeField] private bool ableToDrive = true;
    private Vector3 ogPos;

    private void Awake()
    {
        ogPos = transform.position;
    }

    void Update()
    {
        if (!GameManager.isGameOver)
        {
            if (ableToDrive)
            {
                steeringInput = Input.GetAxisRaw("Horizontal");
                throttleInput = Input.GetAxisRaw("Vertical");
                driftInput = Input.GetKey(driftKey);
            }
            else
            {
                steeringInput = 0;
                throttleInput = 0;
                driftInput = false;
            }

            if (throttleInput < 0)
            {
                steeringInput *= -1;
            }


            if (isGrounded && driftInput && !isDrifting && Mathf.Abs(steeringInput) > float.Epsilon && throttleInput > float.Epsilon)
            {
                isDrifting = true;
                driftDir = steeringInput;
                kartDriftAudioInstance.start();
            }

            if (!driftInput || throttleInput < float.Epsilon)
            {
                isDrifting = false;
                driftDir = 0;
                kartDriftAudioInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            }

            //Align kart to ground
            kartModel.transform.position = transform.position - new Vector3(0, kartModelYModifier);
            kartModel.transform.rotation = Quaternion.Lerp(kartModel.transform.rotation, Quaternion.FromToRotation(kartModel.transform.up, groundNormal) * kartModel.transform.rotation, 0.1f);
            kartModel.transform.localEulerAngles = new Vector3(kartModel.transform.localEulerAngles.x, 0, kartModel.transform.localEulerAngles.z);

            float targetAngle = isDrifting ? targetDriftAngle : targetSteerAngle;
            float targetDir = isDrifting ? driftDir : steeringInput * currentSpeed;
            modelYawTilt = Mathf.Lerp(modelYawTilt, targetAngle * targetDir, yawTiltLerp * Time.deltaTime);
            kartModel.transform.eulerAngles += new Vector3(0, modelYawTilt, 0);

            if (isDrifting)
            {
                leftTrail.emitting = true;
                rightTrail.emitting = true;
                steeringAngleSpeed = driftSteeringAngleSpeed;
            }
            else
            {
                leftTrail.emitting = false;
                rightTrail.emitting = false;
                steeringAngleSpeed = normalSteeringAngleSpeed;
            }
        }

        kartAudioInstance.setParameterByName("speed", Mathf.Clamp01(Mathf.Abs(groundedForwardSpeed / maxSpeed)));
    }

    private void FixedUpdate()
    {
        if (CountdownController.isCountdownEnd && !GameManager.isGameOver)
        {
            float forwardSpeed = groundedForwardSpeed;
            RaycastHit hitInfo;
            bool hit = Physics.SphereCast(sphere.position + new Vector3(0, 0.1f), groundCheckRadius, Vector3.down, out hitInfo, groundMaxDistance, groundLayer);
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

            if (isDrifting && steeringInput != driftDir)
            {
                steeringInput = 0;
            }

            Quaternion steeringTargetRot = Quaternion.Euler(steeringInput * steeringAngleSpeed * speedToSteeringCurve.Evaluate(Mathf.Abs(forwardSpeed)) * Mathf.Sign(forwardSpeed) * groundNormal * Time.fixedDeltaTime);

            // limit steering to the y axis
            Vector3 steeringDeltaAngles = steeringTargetRot.eulerAngles;
            steeringDeltaAngles.x = 0;
            steeringDeltaAngles.z = 0;

            steeringTargetRot = Quaternion.Euler(steeringDeltaAngles);
            steeringCurrentRot = Quaternion.Lerp(steeringCurrentRot, steeringTargetRot, steeringLerp * Time.fixedDeltaTime);

            sphere.MoveRotation(sphere.rotation * steeringCurrentRot);

            sphere.AddForce(groundedForward * forwardSpeed, ForceMode.VelocityChange);

            bool canAccelerate = groundedForwardSpeed < maxSpeed;
            bool canDecelerate = groundedForwardSpeed > maxReverseSpeed;

            float force = 0;
            if (throttleInput > 0 && canAccelerate)
                force = acceleration;
            else if (throttleInput < 0 && canDecelerate)
                force = deceleration;
            force *= Time.fixedDeltaTime;

            sphere.AddForce(groundedForward * force, ForceMode.VelocityChange);

            if (isGrounded && Vector3.Angle(Vector3.up, groundNormal) > 0.1f)
            {
                sphere.AddForce(-Physics.gravity, ForceMode.Acceleration);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Wall"))
        {
            RuntimeManager.PlayOneShot(kartImpactSoundRef);
            Vector3 dir = Vector3.Normalize(transform.position - collision.contacts[0].point);
            dir = new Vector3(dir.x, 0, dir.z);
            sphere.AddForce(dir * 30, ForceMode.VelocityChange);
        }
    }

    public CloneFrameState GetFrameState()
    {
        CloneFrameState newFrame = new();

        newFrame.position = kartModel.transform.position;
        newFrame.rotation = kartModel.transform.rotation;

        return newFrame;
    }
    public void ApplyLapConfig(int lapIndex)
    {
        /*if (lapIndex >= lapConfigs.Length)
            lapIndex = lapConfigs.Length - 1;
        KartLapConfig config = lapConfigs[lapIndex];*/
        maxSpeed += 3;
        //acceleration += 3;
        //deceleration -= 3;
    }
    void Start()
    {
        leftTrail.emitting = false;
        rightTrail.emitting = false;
        kartAudioInstance = RuntimeManager.CreateInstance(kartSoundRef);
        RuntimeManager.AttachInstanceToGameObject(kartAudioInstance, gameObject);
        kartAudioInstance.start();
        kartDriftAudioInstance = RuntimeManager.CreateInstance(kartDriftSoundRef);
        RuntimeManager.AttachInstanceToGameObject(kartDriftAudioInstance, gameObject);
    }
    private void OnDestroy()
    {
        kartAudioInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        kartAudioInstance.release();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Catcher"))
        {
            transform.position = ogPos;
        }
    }
}

[Serializable]
public struct KartLapConfig
{
    public float maxSpeed;
    public float acceleration;
    public float deceleration;
}