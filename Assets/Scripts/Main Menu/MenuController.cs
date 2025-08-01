using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public void Play()
    {
        SceneManager.LoadScene(1, LoadSceneMode.Single);
    }

    public void ReturnToMenu()
    {
        SceneManager.LoadScene(0, LoadSceneMode.Single);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
