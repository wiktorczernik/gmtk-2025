using System.Collections;
using TMPro;
using UnityEngine;

public class CountdownController : MonoBehaviour
{
    static CountdownController main;

    [Header("Components")]
    [SerializeField] GameObject _timerUI;
    [SerializeField] GameObject _countdownUI;
    [SerializeField] TextMeshProUGUI _countdownText;

    [Header("Input State")]
    [SerializeField] double _countdownDefaultTime = 3;

    [Header("State")]
    [SerializeField] double _countdownTime;
    [SerializeField] bool _isCountdownEnd = false;

    public static bool isCountdownEnd
    {
        get => main._isCountdownEnd;
        private set => main._isCountdownEnd = value;
    }

    private void Awake()
    {
        main = this;
    }

    void Start()
    {
        _countdownText.color = new Color(1, 0.007843138f, 0);
        _countdownText.text = string.Format("{0:0}", _countdownDefaultTime);
        _countdownTime = _countdownDefaultTime;
        FMODUnity.RuntimeManager.PlayOneShot("{3c58a345-3da1-479a-9a3c-10a91f92ac2a}");
    }

    void Update()
    {
        if (_countdownTime >= 0 && !isCountdownEnd)
        {
            _countdownTime -= Time.deltaTime;
            _countdownText.text = string.Format("{0:0}", _countdownTime);
        }

        if (_countdownTime <= 1 && !isCountdownEnd)
        {
            StartCoroutine(GameStart());
        }
    }

    IEnumerator GameStart()
    {
        yield return null;
        CloneUtils.RequestStartRecording(GameManager.kartControllerInstance);
        isCountdownEnd = true;

        yield return new WaitForSeconds(.15f);
        _countdownText.color = new Color(0, 1, 0.1137255f);
        _countdownText.text = "Start!";

        yield return new WaitForSeconds(.8f);
        _countdownUI.SetActive(false);
        _timerUI.SetActive(true);
        TimerController.active = true;
       
    }
}
