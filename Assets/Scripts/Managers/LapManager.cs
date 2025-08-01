using TMPro;
using UnityEngine;

public class LapManager : MonoBehaviour
{
    [SerializeField] GameObject LapsUI;
    [SerializeField] private TextMeshProUGUI _lapText;

    private int _currentLap = 1;
    public static int currentLap
    {
        get => main._currentLap;
        private set => main._currentLap = value;
    }

    static LapManager main;

    void Awake()
    {
        main = this;
    }

    [ContextMenu("Increase Lap Counter")]
    public void IncreaseLapCounter()
    {
        currentLap++;

        _lapText.text = $"{currentLap}th Lap";
    }
}