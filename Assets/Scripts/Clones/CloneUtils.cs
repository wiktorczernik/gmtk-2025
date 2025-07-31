using System.Collections.Generic;
using UnityEngine;

public class CloneUtils : MonoBehaviour
{
    public const int recordingFrameRate = 30;
    public const float recordingDeltaTime = 1.0f / recordingFrameRate;

    public static ICloneable recordingTarget { get; set; } = null;
    public static RecordingState recordingState { get; private set; } = RecordingState.Not;
    public static Clone clonePrefab { get; set; }
    public static CloneRecording currentlyRecorded { get; private set; } = null;
    public static readonly List<CloneRecording> currentlyPlayed = new();


    public static CloneRecording RequestStartRecording(ICloneable cloneable)
    {
        if (recordingState != RecordingState.Not) return null;

        currentlyRecorded = new CloneRecording();
        recordingState = RecordingState.AwaitsStart;

        return currentlyRecorded;
    }
    public static CloneRecording RequestStopRecording()
    {
        if (recordingState != RecordingState.Recording) return null;
        
        return currentlyRecorded;
    }

    private void RecordingTick()
    {
        var frameState = recordingTarget.GetFrameState();
        currentlyRecorded.frames.Add(frameState);
        currentlyRecorded.duration += recordingDeltaTime;
    }

    private void FixedUpdate()
    {
        if (recordingState == RecordingState.AwaitsStart)
        {
            currentlyRecorded.duration -= recordingDeltaTime;
            InvokeRepeating(nameof(RecordingTick), 0, recordingDeltaTime);
        }
        else if (recordingState == RecordingState.AwaitsEnd)
        {
            CancelInvoke(nameof(RecordingTick));
        }
    }

    public enum RecordingState
    {
        Not,
        AwaitsStart,
        Recording,
        AwaitsEnd
    }
}
