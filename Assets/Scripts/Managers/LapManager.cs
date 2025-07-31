using TMPro;
using UnityEngine;

public class LapManager : MonoBehaviour
{
    [SerializeField] GameObject LapsUI;
    [SerializeField] private TextMeshProUGUI _lapText;

    private int _currentLap = 1;
    public int CurrentLap
    {
        get => _currentLap;
        private set => _currentLap = value;
    }

    [SerializeField] private int TotalLaps = 3;

    void Awake()
    {
        //GameManager.Instance.OnLapCompleted.AddListener(IncreaseLapCounter);
        //GameManager.Instance.OnGameLoose.AddListener(TestGameLoose);
    }

    [ContextMenu("Increase Lap Counter")]
    public void IncreaseLapCounter()
    {
        CurrentLap++;

        if (CheckForLapCompletion())
            return;

        _lapText.text = $"{CurrentLap} / {TotalLaps}";
    }

    private bool CheckForLapCompletion()
    {
        if (CurrentLap > TotalLaps)
        {
            GameManager.Instance.AllLapsCompleted.Invoke();
            return true;

            //? Win The Game???
        }

        return false;
    }


    public void TestGameWin()
    {
        Debug.Log("Game Win");
    }

    public void TestGameLost()
    {
        Debug.Log("Game Lost");
    }
}