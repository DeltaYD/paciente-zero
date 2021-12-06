using UnityEngine;
using UnityEngine.SceneManagement;
//using static LoadingManager;

public class MainMenu : MonoBehaviour
{
    public Animator _anim;
    public int sceneToMoveOn;

    public void StartGame()
    {
        _anim.SetTrigger("fadeout");
    }

    public void OnFadeComplete()
    {
        SceneManager.LoadScene(sceneToMoveOn);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
