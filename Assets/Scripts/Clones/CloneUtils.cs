using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CloneUtils : MonoBehaviour
{
    public const int recordingFrameRate = 60;
    public const float recordingDeltaTime = 1.0f / recordingFrameRate;
    public const int trimFrameCount = 20; // frames to trim at start and end of recording

    public static ICloneable recordingTarget { get; set; } = null;
    public static RecordingState recordingState { get; private set; } = RecordingState.Not;
    public static Clone clonePrefab { get; set; }
    public static CloneRecording currentlyRecorded { get; private set; } = null;
    public static readonly List<ClonePlayback> currentlyPlayed = new();

    [Header("Clone Settings")]
    public float cloneShowDuration = 0.25f;
    public float cloneHideDuration = 0.25f;

    [Header("Path Settings")]
    public float pathWidth = 0.5f;
    public float pathYoffset = -0.2f;
    public float pathAppearDuration = 1f;
    public Material pathMaterial;

    static CloneUtils main;


    public static ClonePlayback PlayLooped(CloneRecording recording)
    {
        ClonePlayback playback = new();
        playback.recording = recording;
        playback.cloneInstance = Instantiate(clonePrefab);
        playback.cloneInstance.showDuration = main.cloneShowDuration;
        playback.cloneInstance.hideDuration = main.cloneHideDuration;
        playback.cloneInstance.Show();
        playback.state = ClonePlaybackState.Playing;
        currentlyPlayed.Add(playback);
        main.StartCoroutine(main.AppearPath(playback));

        return playback;
    }

    public static CloneRecording RequestStartRecording(ICloneable cloneable)
    {
        if (recordingState != RecordingState.Not) return null;

        currentlyRecorded = new CloneRecording();
        recordingState = RecordingState.AwaitsStart;

        return currentlyRecorded;
    }
    public static void ResetAllPlayedTime()
    {
        main.StartCoroutine(main.ResetPlayedTimeSeq());
    }
    IEnumerator ResetPlayedTimeSeq()
    {
        foreach (var playback in currentlyPlayed)
        {
            playback.cloneInstance.Hide();
        }
        yield return new WaitForSeconds(cloneHideDuration);

        foreach (var playback in currentlyPlayed)
        {
            playback.timePlayed = 0f;
        }

        foreach (var playback in currentlyPlayed)
        {
            playback.cloneInstance.Show();
        }
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
            if (currentlyRecorded == null) return;
            recordingState = RecordingState.Recording;
            currentlyRecorded.duration = 0f;
            var frameState = recordingTarget.GetFrameState();
            currentlyRecorded.frames.Add(frameState);
        }
        else if (recordingState == RecordingState.AwaitsEnd)
        {
            if (currentlyRecorded == null) return;
            var finalState = recordingTarget.GetFrameState();
            currentlyRecorded.frames.Add(finalState);
            currentlyRecorded.duration += recordingDeltaTime;

            int total = currentlyRecorded.frames.Count;
            int trim = Mathf.Min(trimFrameCount, total / 2 - 1);
            var core = currentlyRecorded.frames.Skip(trimFrameCount).ToList();

            float coreDuration = (core.Count - 1) * recordingDeltaTime;
            currentlyRecorded.duration = coreDuration;
            currentlyRecorded.frames = core;
            recordingState = RecordingState.Not;

            currentlyRecorded.pathLine = new GameObject().AddComponent<LineRenderer>();
            List<Vector3> positions = new();
            foreach (var frame in core)
                positions.Add(frame.position + Vector3.up * pathYoffset);
            currentlyRecorded.pathLine.positionCount = positions.Count;
            currentlyRecorded.pathLine.SetPositions(positions.ToArray());
            currentlyRecorded.pathLine.loop = true;
            currentlyRecorded.pathLine.startWidth = 0f;
        }
        else if (recordingState == RecordingState.Recording)
        {
            if (currentlyRecorded == null) return;
            var state = recordingTarget.GetFrameState();
            currentlyRecorded.frames.Add(state);
            currentlyRecorded.duration += recordingDeltaTime;
        }

        foreach (var pb in currentlyPlayed)
        {
            var clone = pb.cloneInstance;
            var frames = pb.recording.frames;
            int count = frames.Count;


            pb.timePlayed += recordingDeltaTime;

            if (pb.timePlayed >= pb.recording.duration - cloneHideDuration - 0.5f)
            {
                pb.cloneInstance.Hide();
            }

            float lerpT = pb.timePlayed / recordingDeltaTime;
            int idx = (int)lerpT;
            float frac = lerpT - idx;
            idx %= count;
            int next = (idx == count - 1) ? 0 : idx + 1;

            var a = frames[idx];
            var b = frames[next];
            var outFrame = a;
            outFrame.position = Vector3.Lerp(a.position, b.position, frac);
            outFrame.rotation = Quaternion.Lerp(a.rotation, b.rotation, frac);
            clone.SetFrameState(outFrame);
        }
    }

    IEnumerator AppearPath(ClonePlayback playback)
    {
        yield return new WaitUntil(() => playback.recording.pathLine != null);
        playback.recording.pathLine.material = pathMaterial;
        float time = 0f;
        while (time <= pathAppearDuration)
        {
            time += Time.deltaTime;
            float fraction = Mathf.Clamp01(time / pathAppearDuration);
            playback.recording.pathLine.startWidth = fraction * pathWidth;
            yield return new WaitForEndOfFrame();
        }
    }

    private void Awake()
    {
        main = this;
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = recordingFrameRate;
    }
    private void OnDestroy()
    {
        recordingTarget = null;
        recordingState = RecordingState.Not;
        clonePrefab = null;
        currentlyRecorded = null;
        currentlyPlayed.Clear();
        main = null;
    }

    public enum RecordingState { Not, AwaitsStart, Recording, AwaitsEnd }
}
