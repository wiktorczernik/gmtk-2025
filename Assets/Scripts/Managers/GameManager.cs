using System;
using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static event Action onGameStart; //* Called When Game Starts
    public static event Action onLapCompleted; //* Called After Finishing One Lap, Check -> PlayerCollisionTrigger.cs
    public static event Action onGameEnd; //* Called When Game Is Lost -> For Now By Timer

    public static void TriggerLapCompletion()
    {
        GameObject.FindGameObjectWithTag("Manager").GetComponent<LapManager>().IncreaseLapCounter();
    }

    IEnumerator Start()
    {
        yield return null;
        onGameStart?.Invoke();
        TimerController.onEnd += OnTimeEnd;
        //TimerController.active = true;
    }

    void OnTimeEnd()
    {
        MenuController.ReturnToMenu();
    }
}
