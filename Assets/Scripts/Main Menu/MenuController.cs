using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public void Play()
    {
        FMODUnity.RuntimeManager.PlayOneShot("{670ed1ed-53de-4996-ad9c-75acb773b066}");
        SceneManager.LoadScene(1, LoadSceneMode.Single);
       
    }

    public void ReturnToMenu()
    {
        SceneManager.LoadScene(0, LoadSceneMode.Single);
    }

    public void Retry()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
