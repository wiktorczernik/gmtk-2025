using UnityEngine;

public class ClonePlayback
{
    public ClonePlaybackState state;
    public CloneRecording recording;
    public Clone cloneInstance;
    public LineRenderer pathLine => recording.pathLine;
    public float timePlayed = 0f;
}
public enum ClonePlaybackState
{
    Not,
    AwaitsStart,
    Playing,
    AwaitsStop
}