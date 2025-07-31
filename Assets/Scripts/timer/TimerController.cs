using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class TimerController : MonoBehaviour
{
    #region GameObjects
    [Header("Objects")]
    [SerializeField] private GameObject _timerUI;
    [SerializeField] private TextMeshProUGUI _timerText;
    #endregion

    #region Fields
    [Header("Fields")]
    [SerializeField] private double _defaultTime = 30;
    [SerializeField] private bool _defaultActive = false;
    #endregion

    #region UnityEvents
    [Header("UnityEvents")]
    [SerializeField] private UnityEvent _timeEnd;
    #endregion

    #region Variables
    [Header("Variables")]
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
    public bool active
    {
        get
        {
            return _active;
        }
        set
        {
            _active = value;
        }
    }
    #endregion

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
            _timeEnd.Invoke();
            _timerUI.SetActive(false);
        }
    }
}