using UnityEngine;

public class TestCloneable : MonoBehaviour, ICloneable
{
    public Clone clonePrefab;
    public float moveSpeed = 5f;

    
    public CloneFrameState GetFrameState()
    {
        CloneFrameState frame = new();

        frame.position = transform.position;
        frame.rotation = transform.rotation;
        
        return frame;
    }


    void Start()
    {
        CloneUtils.clonePrefab = clonePrefab;
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (CloneUtils.recordingState == CloneUtils.RecordingState.Not)
            {
                CloneUtils.recordingTarget = this;
                CloneUtils.RequestStartRecording(this);
            }
            else if (CloneUtils.recordingState == CloneUtils.RecordingState.Recording)
            {
                var recording = CloneUtils.RequestStopRecording();
                CloneUtils.PlayLooped(recording);
            }
        }

        transform.position += moveSpeed * Vector3.right * Input.GetAxisRaw("Horizontal") * Time.deltaTime;
        transform.position += moveSpeed * Vector3.up * Input.GetAxisRaw("Vertical") * Time.deltaTime;
    }
}
