using TMPro;
using UnityEngine;

public class LapManager : MonoBehaviour
{
    static LapManager main;
    static GameObject[] checkpointGates;

    #region Game Objects
    [Header("GameObjects")]
    [SerializeField] public TextMeshProUGUI _lapText;
    #endregion

    #region Variables
    private int _maxCheckpointGate;
    private int _currentCheckpointGate = 0;
    private int _currentLap = 1;
    #endregion

    #region Public Variables
    public static int maxCheckpointGate
    {
        get => main._maxCheckpointGate;
        private set => main._maxCheckpointGate = value;
    }

    public static int currentLap
    {
        get => main._currentLap;
        private set => main._currentLap = value;
    }

    public static int currentCheckpointGate
    {
        get => main._currentCheckpointGate;
        set => main._currentCheckpointGate = value;
    }
    #endregion

    void Awake()
    {
        main = this;
        checkpointGates = GameObject.FindGameObjectsWithTag("LapCheckpoint");
        maxCheckpointGate = checkpointGates.Length;
    }

    [ContextMenu("Increase Lap Counter")]
    public void IncreaseLapCounter()
    {
        ResetCheckpoints();

        currentLap++;

        if (currentLap.ToString().EndsWith('1') && currentLap != 11)
            _lapText.text = $"{currentLap}st Lap";
        else if (currentLap.ToString().EndsWith('2') && currentLap != 12 || currentLap == 2)
            _lapText.text = $"{currentLap}nd Lap";
        else if (currentLap.ToString().EndsWith('3') && currentLap != 13 || currentLap == 3)
            _lapText.text = $"{currentLap}rd Lap";
        else
            _lapText.text = $"{currentLap}th Lap";
    }

    private void ResetCheckpoints()
    {
        while(currentCheckpointGate > 0)
        {
            checkpointGates[currentCheckpointGate - 1].GetComponent<LapCheckpointGate>().isActive = true;

            currentCheckpointGate--;
        }
    }
}