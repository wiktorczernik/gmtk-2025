using UnityEngine;

public class PlayerCollisionTrigger : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("LapTrigger"))
        {
            GameManager.Instance.OnLapCompleted.Invoke();
        }
    }

    [ContextMenu("Test Functionality")]
    public void TestFunctionality()
    {
        GameManager.Instance.OnLapCompleted.Invoke();
    }
}