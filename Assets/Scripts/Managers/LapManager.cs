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
    private int _currentLap = 0;
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

    private void OnTriggerEnter(Collider collision)
    {
        if (!collision.CompareTag("Player")) return;

        IncreaseLapCounter();
        GameManager.TriggerLapCompletion();
    }

    [ContextMenu("Increase Lap Counter")]
    public void IncreaseLapCounter()
    {
        currentLap++;

        _lapText.text = $"{currentLap}th Lap";
    }
}