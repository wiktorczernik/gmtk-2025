using System.Collections;
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
    public static readonly List<ClonePlayback> currentlyPlayed = new();

    static CloneUtils main;


    public static ClonePlayback PlayLooped(CloneRecording recording)
    {
        ClonePlayback playback = new();
        playback.recording = recording;
        playback.cloneInstance = Instantiate(clonePrefab);
        main.StartCoroutine(main.PlayLoopedCoroutine(playback));
        return playback;
    }
    IEnumerator PlayLoopedCoroutine(ClonePlayback playback)
    {
        playback.state = ClonePlaybackState.Playing;
        currentlyPlayed.Add(playback);

        Clone clone = playback.cloneInstance;
        List<CloneFrameState> frames = playback.recording.frames;
        int framesCount = frames.Count;

        clone.SetFrameState(frames[0]);

        yield return new WaitForEndOfFrame();
        
        while (true)
        {
            playback.timePlayed += Time.deltaTime;
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

            yield return new WaitForEndOfFrame();
        }

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
            recordingState = RecordingState.Recording;
            currentlyRecorded.duration -= recordingDeltaTime;
            InvokeRepeating(nameof(RecordingTick), 0, recordingDeltaTime);
        }
        else if (recordingState == RecordingState.AwaitsEnd)
        {
            CancelInvoke(nameof(RecordingTick));
            recordingState = RecordingState.Not;
            currentlyRecorded.finished = true;
            currentlyRecorded = null;
        }
    }

    private void Awake()
    {
        main = this;
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
