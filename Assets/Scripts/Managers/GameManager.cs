using System;
using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static event Action onGameStart; //* Called When Game Starts
    public static event Action onLapCompleted; //* Called After Finishing One Lap, Check -> PlayerCollisionTrigger.cs
    public static event Action onGameEnd; //* Called When Game Is Lost -> For Now By Timer

    public GameLapSettings[] allLapSettings = new GameLapSettings[1];
    public GameLapSettings activeLapSettings;
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

    void ActivateLapSettings(int lapIndex, bool deactivatePrevious = true)
    {
        if (deactivatePrevious)
        {
            foreach (var timeGate in activeLapSettings.activeGates)
            {
                timeGate.SetActive(false);
            }
        }

        lapIndex %= allLapSettings.Length;
        activeLapSettings = allLapSettings[lapIndex];

        foreach (var timeGate in activeLapSettings.activeGates)
        {
            timeGate.SetActive(true);
        }
    }

    void OnGameStart()
    {
        kartControllerInstance = FindFirstObjectByType<KartController>();
        kartControllerInstance.ApplyLapConfig(0);
        CloneUtils.clonePrefab = kartClonePrefab;
        CloneUtils.recordingTarget = kartControllerInstance;
        ActivateLapSettings(lapIndex: 0, deactivatePrevious: false);

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
        kartControllerInstance.ApplyLapConfig(LapManager.currentLap - 1);
        ActivateLapSettings(lapIndex: LapManager.currentLap - 1, deactivatePrevious: true);
    }
}

[Serializable]
public class GameLapSettings
{
    public TimeGate[] activeGates;
}