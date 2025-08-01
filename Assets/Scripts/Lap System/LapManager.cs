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