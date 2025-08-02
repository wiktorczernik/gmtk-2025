using System;
using TMPro;
using UnityEngine;

public class TimerController : MonoBehaviour
{
    static TimerController main;

    #region Components
    [Header("Components")]
    [SerializeField] private GameObject _timerUI;
    [SerializeField] private TextMeshProUGUI _timerText;
    #endregion

    #region Input State
    [Header("Input State")]
    [SerializeField] private double _defaultTime = 30;
    [SerializeField] private bool _defaultActive = false;
    #endregion

    public static event Action onEnd;

    #region State
    [Header("State")]
    [SerializeField] private double _time;
    [SerializeField] private bool _active;
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

        _timerUI.SetActive(_defaultActive);
        time = _defaultTime;
        _timerText.text = time + ".00";
    }

    void Update()
    {
        if (time >= 0 && active)
        {
            time -= Time.deltaTime;
            _timerText.text = string.Format("{0:0.00}", time).Replace(',', '.');
        }

        if (time <= 0 && active)
        {
            active = false;
            onEnd.Invoke();
            _timerUI.SetActive(false);
        }
    }
}