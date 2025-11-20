using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameMainMenuManager : MonoBehaviour
{

    public void goPlay()
    {
        SceneManager.LoadScene("Level1");
    }

    public void quitGame()
    {
        Application.Quit();
    }

}
