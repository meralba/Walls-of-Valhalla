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
}
