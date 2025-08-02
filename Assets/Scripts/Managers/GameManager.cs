using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static event Action onGameStart; //* Called When Game Starts
    public static event Action onLapCompleted; //* Called After Finishing One Lap, Check -> PlayerCollisionTrigger.cs
    public static event Action onGameEnd; //* Called When Game Is Lost -> For Now By Timer
    
    public KartClone kartClonePrefab;
    public LapManager lapManager;

    KartController kartControllerInstance;

    static GameManager main;

    private void Awake()
    {
        main = this;
    }
    IEnumerator Start()
    {
        yield return null;
        onGameStart?.Invoke();
        OnGameStart();
    }

    public static void TriggerLapCompletion() => main.StartCoroutine(main.OnLapCompletion());

    void OnGameStart()
    {
        kartControllerInstance = FindFirstObjectByType<KartController>();
        CloneUtils.clonePrefab = kartClonePrefab;
        CloneUtils.recordingTarget = kartControllerInstance;

        TimerController.onEnd += OnTimeEnd;
        lapManager._lapText.text = $"{LapManager.currentLap}th Lap";
        TimerController.active = true;
        CloneUtils.RequestStartRecording(kartControllerInstance);
    }

    void OnTimeEnd()
    {
        MenuController.ReturnToMenu();
    }
    IEnumerator OnLapCompletion()
    {
        lapManager.IncreaseLapCounter();
        Debug.Log("Completion 0");
        CloneRecording recording = CloneUtils.RequestStopRecording();
        CloneUtils.PlayLooped(recording);
        Debug.Log("Completion 1");
        yield return new WaitUntil(() => CloneUtils.recordingState == CloneUtils.RecordingState.Not);
        Debug.Log("Completion 2");
        CloneUtils.RequestStartRecording(kartControllerInstance);
        Debug.Log("Completion 3");
    }
}
