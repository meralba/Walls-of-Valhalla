using UnityEngine;

using System.Collections;       //Allows us to use Lists. 
using System.Collections.Generic;

using UnityEngine.UI;

    public class GameManager : MonoBehaviour
    {
        // Hacemos la clase un singleton
        public static GameManager instance = null;

    public float turnDelay = 0.1f;
    public float levelStartDelay = 2f;
    public bool controlDisabled = true;

    public LayerMask blockingLayer;                         // Layer containing blocking objects

    //public GridMap nodeGrid = null;
    public Pathfinder pathFinder = null;

    private BoardManager boardScript;                       //Store a reference to our BoardManager which will set up the level.

    public int playerHealth = 100;
    [HideInInspector]
    public bool playersTurn = true;

    private int level = 1;                                  //Current level number
    private GameObject levelImage;
    private Text levelText;
    // Lista de enemigos
    private List<Enemy> enemies;
    private bool enemiesMoving;
    private bool doingSetup;

    //Awake is always called before any Start functions
    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
        //Get a component reference to the attached BoardManager script
        boardScript = GetComponent<BoardManager>();

        this.enemies = new List<Enemy>();

        // -1 and 22 because it has 2 
        //nodeGrid = new GridMap(-1, -1, 22, 22, 1, blockingLayer);

        //Attention;
        // Initialise the GridMap within the pathFinder if this design approach is finally taken
        pathFinder = new Pathfinder(new GridMap(-1, -1, 22, 22, 1, blockingLayer));


        //Call the InitGame function to initialize the first level 
        InitGame();
    }

    private void OnLevelWasLoaded(int index)
    {
        level++;
       // turnDelay = 0.1f / level;
        InitGame();
    }

        //Initializes the game for each level.
        void InitGame()
        {
        controlDisabled = true;
        doingSetup = true;

        levelImage = GameObject.Find("LevelImage");
        levelText = GameObject.Find("LevelText").GetComponent<Text>();
        levelText.text = "Level " + level;
        levelImage.SetActive(true);
        Invoke("HideLevelImage", levelStartDelay);

        enemies.Clear();
        //Call the SetupScene function of the BoardManager script, pass it current level number.
        boardScript.SetupScene(level);

            
        }

    private void HideLevelImage()
    {
        levelImage.SetActive(false);

        doingSetup = false;
        controlDisabled = false;
    }

    public void GameOver()
    {
        levelText.text = "After " + level + " levels, you die.";
        levelImage.SetActive(true);
        enabled = false;
    }

    //Update is called every frame.
    void Update()
    {
        if (playersTurn || enemiesMoving || doingSetup)
            return;

        StartCoroutine(MoveEnemies());
    }

    public void AddEnemyToList(Enemy script)
    {
        enemies.Add(script);
    }

    IEnumerator MoveEnemies()
    {
        enemiesMoving = true;
        yield return new WaitForSeconds(this.turnDelay);

        if(enemies.Count == 0)
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
    }
}