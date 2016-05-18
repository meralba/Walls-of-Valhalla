using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class SceneManager : MonoBehaviour {

    public static SceneManager instance = null;

    public Scene menuScene, gameScene;

    public string game, menu;

	void Start () {
        // Singleton
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        // Don't destroy this object on load
        DontDestroyOnLoad(gameObject);
    }
	

    public void LoadGameScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(game);
    }

    public void LoadMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(menu);
    }
}
