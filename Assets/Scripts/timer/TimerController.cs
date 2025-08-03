using FMODUnity;
using System;
using TMPro;
using UnityEngine;

public class TimerController : MonoBehaviour
{
    static TimerController main;

    [Header("Audio")]
    public EventReference timeLowAudio;

    #region Components
    [Header("Components")]
    [SerializeField] private GameObject _timerUI;
    [SerializeField] private TextMeshProUGUI _timerText;
    #endregion

    #region Input State
    [Header("Input State")]
    [SerializeField] private double _defaultTime = 30;
    #endregion

    public static event Action onEnd;

    #region State
    [Header("State")]
    [SerializeField] private double _time;
    [SerializeField] private bool _active = false;
    #endregion

    #region Public Variables
    public double time
    {
        get
        {
            return _time;
        }
        set
        {
            _time = value;
        }
    }
    public static bool active
    {
        get
        {
            return main._active;
        }
        set
        {
            main._active = value;
        }
    }
    #endregion

    private void Awake()
    {
        main = this;
    }
    
    void Start()
    {
        if (_timerText == null || _timerUI == null)
        {
            Debug.LogWarning("Objects was not setup!");
            return;
        }

        _timerUI.SetActive(active);
        time = _defaultTime;
        _timerText.text = string.Format("{0:0}", time);
    }

    void Update()
    {
        if (time >= 0 && active)
        {
            time -= Time.deltaTime;
            _timerText.text = string.Format("{0:0.00}", time).Replace(',', '.');
        }

        if (time == 5 && active)
        {
            FMODUnity.RuntimeManager.PlayOneShot(timeLowAudio);
        }

        if (time <= 0 && active)
        {
            active = false;
            onEnd.Invoke();
            _timerUI.SetActive(false);
        }
    }
}