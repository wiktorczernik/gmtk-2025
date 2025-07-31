using TMPro;
using UnityEngine;

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
    [SerializeField] private bool _defaultActive = true;
    #endregion

    #region Variables
    [Header("Variables")]
    [SerializeField] private double _time;
    #endregion

    #region Public Variables
    public static TimerController Instance;
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
    public TextMeshProUGUI timerText
    {
        get
        {
            return _timerText;
        }
        set
        {
            _timerText = value;
        }
    }
    public bool defaultActive
    {
        get
        {
            return _defaultActive;
        }
        set
        {
            _defaultActive = value;
        }
    }
    #endregion

    void Start()
    {
        if (Instance != null)
        {
            Debug.LogWarning("More than one instance of TimerController!");
            Destroy(gameObject);
        }
        else Instance = this;

        if (timerText == null || _timerUI == null)
        {
            Debug.LogWarning("Objects was not setup!");
            return;
        }

        _timerUI.SetActive(defaultActive);

        if (defaultActive)
        {
            time = _defaultTime;
            timerText.text = time + ".00";
        }
    }

    void Update()
    {
        if (time >= 0)
        {
            time -= Time.deltaTime;
            timerText.text = string.Format("{0:0.00}", time).Replace(',', '.');
        }
    }
}