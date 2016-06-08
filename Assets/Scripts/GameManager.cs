using UnityEngine;

using System.Collections;       //Allows us to use Lists. 
using System.Collections.Generic;

using UnityEngine.UI;

    public class GameManager : MonoBehaviour
    {
    // ShowExploration 2
    // Use this to show the explored nodes in A*
    // Other comments here and in Pathfinder.cs
    public GameObject marker;
        // Hacemos la clase un singleton
        public static GameManager instance = null;

    public float turnDelay = 0.2f;
    public float levelStartDelay = 2f;
    public bool controlDisabled = true;

    public LayerMask blockingLayer;                         // Layer containing blocking objects

    //public GridMap nodeGrid = null;
    public Pathfinder pathFinder = null;

    private BoardManager boardScript;                       //Store a reference to our BoardManager which will set up the level.

    public int maxPlayerHealth = 100;
    public int playerHealth;
    [HideInInspector]
    public bool playersTurn = true;

    public int level = 1;                                  //Current level number
    public int levelsPerStage = 2;
    public int stages = 2;

    public GameObject levelImage;
    public Text levelText;
    // Lista de enemigos
    public List<Enemy> enemies;
    private bool enemiesMoving;
    private bool doingSetup;

    public bool isPaused=false;
    public GameObject pauseMenu;
    private GameObject menuInstance;

    //Awake is always called before any Start functions
    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);


        //Get a component reference to the attached BoardManager script
        boardScript = GetComponent<BoardManager>();

        this.enemies = new List<Enemy>();

        levelText = GameObject.Find("LevelText").GetComponent<Text>();
        levelImage = GameObject.Find("LevelImage");


        // This seems stupid, but with the first sentence the camera's reference is null.
        //mainCamera = (Instantiate(Cam, new Vector3(5.5f, 5.5f, -1f), Quaternion.identity) as GameObject);
        //mainCamera = GameObject.FindGameObjectWithTag("MainCamera");


        //Call the InitGame function to initialize the first level 
        InitGame();
    }

    private void OnLevelWasLoaded(int index)
    {
        //level++;

        InitGame();
    }

        //Initializes the game for each level.
    void InitGame()
    {
        controlDisabled = true;
        doingSetup = true;

        

        pathFinder = new Pathfinder(new GridMap(-1, -1, 22, 22, 1, blockingLayer));
        // ShowExploration 2
        // Change this to show the explored nodes in A*
        // pathFinder.marker = this.marker;



        //levelText = GameObject.Find("LevelText").GetComponent<Text>();

        levelText.text = "Level " + level;

        levelImage.SetActive(true);
        Invoke("HideLevelImage", levelStartDelay);

        enemies.Clear();
        //Call the SetupScene function of the BoardManager script, pass it current level number.
        boardScript.SetupScene(level);

        // Set the player's health
        GameObject.FindGameObjectWithTag("Player").GetComponent<Player>().setHealth(this.playerHealth);

        // Abstract this into the camera class
        //mainCamera.transform.SetParent(null);
        //mainCamera.GetComponent<Camera>().orthographicSize = 7;
        //mainCamera.transform.position = new Vector3(5.5f, 5.5f, -1f);
    }

    private void HideLevelImage()
    {
        levelImage.SetActive(false);

        doingSetup = false;
        controlDisabled = false;
    }

    public IEnumerator GameOver()
    {
        levelText.text = "After " + level + " levels, you die.";
        levelImage.SetActive(true);
        controlDisabled = true;
        yield return new WaitForSeconds(2f);
        Destroy(gameObject);

        SceneManager.instance.LoadMenu();
    }

    public void NextLevel()
    {
        level++;
        this.enemies = new List<Enemy>();

        InitGame();
    }

    //Update is called every frame.
    void Update()
    {
        if( Input.GetKeyDown(KeyCode.Escape) )
            Pause();


        if (playersTurn || enemiesMoving || doingSetup)
            return;

        StartCoroutine(MoveEnemies());
    }

    // Eliminated for the sake of playability
    /*
    public void ToogleCamera()
    {
        // This should be moved onto another script that handles cameras
        if (mainCamera.transform.parent == null)
        {
            Transform player;
            player = GameObject.FindGameObjectWithTag("Player").transform;
            mainCamera.transform.position = new Vector3(player.position.x, player.position.y, -1f);
            mainCamera.GetComponent<Camera>().orthographicSize = 4;
            mainCamera.transform.SetParent(GameObject.FindGameObjectWithTag("Player").transform);
        }
        else
        {
            mainCamera.transform.SetParent(null);
            mainCamera.GetComponent<Camera>().orthographicSize = 7;
            mainCamera.transform.position = new Vector3(5.5f, 5.5f, -1f);
        }
    }*/

    public void Pause()
    {
        if (isPaused == true)
        {
            Time.timeScale = 1;
            isPaused = false;
            ToggleInterface();
        }
        else
        {
            Time.timeScale = 0;
            isPaused = true;
            ToggleInterface();
        }
    }

    public void ToggleInterface()
    {
        if (isPaused == true)
        {
            menuInstance = Instantiate(pauseMenu) as GameObject;
        }
        else
        {
            if (menuInstance != null)
            {
                Destroy(menuInstance);
            }
        }
  
    }

    public void AddEnemyToList(Enemy script)
    {
        enemies.Add(script);
    }

    IEnumerator MoveEnemies()
    {
        enemiesMoving = true;
        controlDisabled = true;
        yield return new WaitForSeconds(this.turnDelay);
        

        for (int i = 0; i < enemies.Count; i++)
        {
            if (enemies[i] != null)
            {
                enemies[i].MoveEnemy();
            }
            yield return new WaitForSeconds(this.turnDelay);

           
        }
        playersTurn = true;
        enemiesMoving = false;
        controlDisabled = false;
    }
}