using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloneUtils : MonoBehaviour
{
    public const int recordingFrameRate = 60;
    public const float recordingDeltaTime = 1.0f / recordingFrameRate;

    public static ICloneable recordingTarget { get; set; } = null;
    public static RecordingState recordingState { get; private set; } = RecordingState.Not;
    public static Clone clonePrefab { get; set; }
    public static CloneRecording currentlyRecorded { get; private set; } = null;
    public static readonly List<ClonePlayback> currentlyPlayed = new();

    static CloneUtils main;


    public static ClonePlayback PlayLooped(CloneRecording recording)
    {
        ClonePlayback playback = new();
        playback.recording = recording;
        playback.cloneInstance = Instantiate(clonePrefab);
        playback.state = ClonePlaybackState.Playing;
        currentlyPlayed.Add(playback);
        return playback;
    }
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

        recordingState = RecordingState.AwaitsEnd;

        return currentlyRecorded;
    }

    private void FixedUpdate()
    {
        if (recordingState == RecordingState.AwaitsStart)
        {
            recordingState = RecordingState.Recording;
            currentlyRecorded.duration -= recordingDeltaTime;
            var frameState = recordingTarget.GetFrameState();
            currentlyRecorded.frames.Add(frameState);
        }
        else if (recordingState == RecordingState.AwaitsEnd)
        {
            var frameState = recordingTarget.GetFrameState();
            currentlyRecorded.frames.Add(frameState);
            currentlyRecorded.duration += Time.fixedDeltaTime;
            recordingState = RecordingState.Not;
        }
        else if (recordingState == RecordingState.Recording)
        {
            var frameState = recordingTarget.GetFrameState();
            currentlyRecorded.frames.Add(frameState);
            currentlyRecorded.duration += Time.fixedDeltaTime;
        }

        foreach(var playback in currentlyPlayed)
        {
            Clone clone = playback.cloneInstance;
            List<CloneFrameState> frames = playback.recording.frames;
            int framesCount = frames.Count;

            playback.timePlayed += Time.fixedDeltaTime;
            float frameLerp = playback.timePlayed / recordingDeltaTime;
            int startFrameIndex = (int)frameLerp;
            frameLerp -= startFrameIndex;
            startFrameIndex %= framesCount;
            int endFrameIndex;

            if (startFrameIndex == framesCount - 1)
                endFrameIndex = 0;
            else
                endFrameIndex = startFrameIndex + 1;

            CloneFrameState startFrame = frames[startFrameIndex];
            CloneFrameState endFrame = frames[endFrameIndex];

            CloneFrameState interpolatedFrame = startFrame;
            interpolatedFrame.position = Vector3.Lerp(startFrame.position, endFrame.position, frameLerp);
            interpolatedFrame.rotation = Quaternion.Lerp(startFrame.rotation, endFrame.rotation, frameLerp);
            clone.SetFrameState(interpolatedFrame);
        }
    }

    private void Awake()
    {
        main = this;
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
    }
    private void LateUpdate()
    {
        
    }

    public enum RecordingState
    {
        Not,
        AwaitsStart,
        Recording,
        AwaitsEnd
    }
}
