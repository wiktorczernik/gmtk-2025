using TMPro;
using UnityEngine;

public class CloneRecordingDebug_GUI : MonoBehaviour
{
    TMP_Text text;

    private void Awake()
    {
        text = GetComponent<TMP_Text>();
    }
    private void Update()
    {
        text.text = CloneUtils.recordingState == CloneUtils.RecordingState.Recording ? "Recording" : "Awaiting";
        text.color = CloneUtils.recordingState == CloneUtils.RecordingState.Recording ? Color.red : Color.white;
    }
}
