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

    #region Components
    [Header("Components")]
    public KartClone kartClonePrefab;
    public LapManager lapManager;
    public GameObject timerUI;
    public GameObject countdownUI;
    public GameObject pauseUI;
    public GameObject gameOverUI;
    #endregion

    KartController _kartControllerInstance;

    public static KartController kartControllerInstance
    {
        get => main._kartControllerInstance;
        private set => main._kartControllerInstance = value;
    }

    static GameManager main;

    private bool _isPaused = false;
    public static bool isGameOver = false;

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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !gameOverUI.activeSelf && CountdownController.isCountdownEnd)
        {
            OnPause();
        }
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
        isGameOver = false;
        pauseUI.SetActive(false);
        gameOverUI.SetActive(false);
        countdownUI.SetActive(true);
        kartControllerInstance = FindFirstObjectByType<KartController>();
        kartControllerInstance.ApplyLapConfig(0);
        CloneUtils.clonePrefab = kartClonePrefab;
        CloneUtils.recordingTarget = kartControllerInstance;
        ActivateLapSettings(lapIndex: 0, deactivatePrevious: false);

        TimerController.onEnd += OnTimeEnd;
        lapManager._lapText.text = $"{LapManager.currentLap}st Lap";
        //CloneUtils.RequestStartRecording(kartControllerInstance);
    }

    void OnPause()
    {
        _isPaused = !_isPaused;

        if (_isPaused)
        {
            Time.timeScale = 0f;
            timerUI.SetActive(false);
            pauseUI.SetActive(true);
        }
        else
        {
            Time.timeScale = 1f;
            timerUI.SetActive(true);
            pauseUI.SetActive(false);
        }
    }

    void OnTimeEnd()
    {
        gameOverUI.SetActive(true);
        isGameOver = true;
        TimerController.onEnd -= OnTimeEnd;
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