using UnityEngine;
using System.Collections;

public class Menu : MonoBehaviour {
    public void StartGameButton()
    {
        SceneManager.instance.LoadGameScene();
    }

    public void ExitGameButton()
    {
        UnityEngine.Application.Quit();
    }

    public void ExitToMenu()
    {
        GameManager.instance.GameOver();
        SceneManager.instance.LoadMenu();
        Time.timeScale = 1;
        GameManager.instance.isPaused = false;
        
    }

}
