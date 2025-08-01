using TMPro;
using UnityEngine;

public class LapManager : MonoBehaviour
{
    static LapManager main;

    #region Game Objects
    [Header("GameObjects")]
    [SerializeField] TextMeshProUGUI _lapText;
    #endregion

    #region Variables
    private int _currentLap = 1;
    #endregion

    #region Public Variables
    public static int currentLap
    {
        get => main._currentLap;
        private set => main._currentLap = value;
    }
    #endregion

    void Awake()
    {
        main = this;
    }

    // It's temporary option, because we don't have function when game starting
    private void Start()
    {
        _lapText.text = $"{currentLap}th Lap";
    }

    [ContextMenu("Increase Lap Counter")]
    public void IncreaseLapCounter()
    {
        currentLap++;

        _lapText.text = $"{currentLap}th Lap";
    }
}