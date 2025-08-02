using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public static void Play()
    {
        SceneManager.LoadScene(1, LoadSceneMode.Single);
        FMODUnity.RuntimeManager.PlayOneShot("{670ed1ed-53de-4996-ad9c-75acb773b066}");
    }

    public static void ReturnToMenu()
    {
        SceneManager.LoadScene(0, LoadSceneMode.Single);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
