using System.Collections.Generic;
using UnityEngine;

public class CloneRecording
{
    public float duration = 0;
    public float lapCompleteTime = 0f;
    public bool finished = false;
    public List<CloneFrameState> frames = new();
    public LineRenderer pathLine;
}
