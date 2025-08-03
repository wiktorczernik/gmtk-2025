using FMOD.Studio;
using FMODUnity;
using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static event Action onGameStart; //* Called When Game Starts
    public static event Action onLapCompleted; //* Called After Finishing One Lap, Check -> PlayerCollisionTrigger.cs
    public static event Action onGameEnd; //* Called When Game Is Lost -> For Now By Timer

    public GameLapSettings[] allLapSettings = new GameLapSettings[1];
    public GameLapSettings activeLapSettings;

    [Header("Audio References")]
    public EventReference mainAudioEventRef;

    public EventInstance mainAudioEventIns;

    #region Components
    [Header("Components")]
    public KartClone kartClonePrefab;
    public LapManager lapManager;
    public GameObject timerUI;
    public GameObject countdownUI;
    public GameObject pauseUI;
    public GameObject gameOverUI;
    public TextMeshProUGUI gameOverScoreText;
    #endregion

    KartController _kartControllerInstance;

    public static KartController kartControllerInstance
    {
        get => main._kartControllerInstance;
        private set => main._kartControllerInstance = value;
    }

    static GameManager main;

    public static bool isPaused = false;
    public static bool isGameOver = false;

    public int highScore;

    private void Awake()
    {
        main = this;
    }

    private void OnDestroy()
    {
        main = null;
        mainAudioEventIns.release();
    }
    
    IEnumerator Start()
    {
        yield return null;

        mainAudioEventIns = RuntimeManager.CreateInstance(mainAudioEventRef);

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
        mainAudioEventIns.start();
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
    }

    void OnPause()
    {
        isPaused = !isPaused;

        mainAudioEventIns.setPaused(isPaused);

        if (isPaused)
        {
            Time.timeScale = 0f;

            if (GameObject.FindGameObjectWithTag("TimeAdder"))
                GameObject.FindGameObjectWithTag("TimeAdder").transform.GetChild(0).gameObject.SetActive(false);
            timerUI.SetActive(false);
            pauseUI.SetActive(true);
        }
        else
        {
            Time.timeScale = 1f;
            timerUI.SetActive(true);
            if (GameObject.FindGameObjectWithTag("TimeAdder"))
                GameObject.FindGameObjectWithTag("TimeAdder").transform.GetChild(0).gameObject.SetActive(true);
            pauseUI.SetActive(false);
        }
    }

    void OnTimeEnd()
    {
        int currentLap = LapManager.currentLap;

        if (currentLap > highScore)
        {
            highScore = currentLap;
        }

        gameOverUI.SetActive(true);
        gameOverScoreText.text = "Score: " + currentLap + " Laps\nHigh Score: " + highScore + " Laps";
        isGameOver = true;
        TimerController.onEnd -= OnTimeEnd;
        mainAudioEventIns.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
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