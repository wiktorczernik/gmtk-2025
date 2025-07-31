using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UItest : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI loopCountText;
    [SerializeField] private TextMeshProUGUI timerLeftText;

    [SerializeField] private float startTimer = 30;
    private int lapCount = 0;

    private void Start()
    {
        checkpiont.RaiseTimeEvent += Checkpiont_RaiseTimeEvent;
        Finish.OnFinishReached += Finish_OnFinishReached;
    }

    private void Finish_OnFinishReached()
    {
        lapCount += 1;
        loopCountText.text = lapCount.ToString();
    }

    private void Checkpiont_RaiseTimeEvent()
    {
        startTimer += 5;
    }

    private void Update()
    {
        startTimer -= Time.deltaTime;
        timerLeftText.text = startTimer.ToString("F0");

        if (startTimer <= 0)
        {
            SceneManager.LoadScene(0);
        }
    }
}
