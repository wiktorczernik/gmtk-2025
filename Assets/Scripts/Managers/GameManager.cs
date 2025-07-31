using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    public UnityEvent OnGameStart; //* Called When Game Starts
    public UnityEvent OnLapCompleted; //* Called After Finishing One Lap, Check -> PlayerCollisionTrigger.cs
    public UnityEvent OnGameLoose; //* Called When Game Is Lost -> For Now By Timer

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

    }

    void Start()
    {
        OnGameStart.Invoke(); //* For Now, Game Starts Automatically
    }
}
