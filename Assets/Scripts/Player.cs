using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;


public class Player : MovingObject {

    public int wallDamage = 1, pointsPerFood = 20, pointsPerSoda = 20, currentOrientation = 1, lastOrientation = 1;
    public float restartLevelDelay = 1f;
    public Text foodText;

    public AudioClip moveSound1;
    public AudioClip moveSound2;
    public AudioClip eatSound1;
    public AudioClip eatSound2;
    public AudioClip drinkSound1;
    public AudioClip drinkSound2;
    public AudioClip gameOverSound;



    private Animator animator;
    private int food;



    // Use this for initialization
    protected override void Start () {
        animator = GetComponent<Animator>();

        food = GameManager.instance.playerFoodPoints;

        updateFoodText();

        base.Start();
	}

    private void updateFoodText()
    {
        foodText.text = "Food: " + food;
    }

    private void onDisable()
    {
        GameManager.instance.playerFoodPoints = this.food;
    }
	
	// Update is called once per frame
	void Update () {
        if (!GameManager.instance.playersTurn)
            return;

        int horizontal = 0, vertical = 0;

        // Código para el teclado
        horizontal = (int)Input.GetAxisRaw("Horizontal");
        vertical = (int)Input.GetAxisRaw("Vertical");

        if (horizontal != 0)
            vertical = 0;

        if ((horizontal + vertical) != 0)
            AttemptMove<Wall>(horizontal, vertical);
	}

    protected override void AttemptMove<T>(int xDir, int yDir)
    {
        this.food--;
        updateFoodText();

        base.AttemptMove<T>(xDir, yDir);

        RaycastHit2D hit;
        if(Move(xDir,yDir,out hit))
        {
            // Actualizamos la orientación actual
            if (xDir == 1)
                currentOrientation = 1;
            else if (xDir == -1)
                currentOrientation = 3;
            else if (yDir == 1)
                currentOrientation = 0;
            else if (yDir == -1)
                currentOrientation = 2;

            int diffOrientation;

            diffOrientation = lastOrientation - currentOrientation;

            // Codigo para rotar
            //gameObject.transform.Rotate(new Vector3(0, 0, 90* diffOrientation));

            lastOrientation = currentOrientation;

            SoundManager.instance.RandomizeSfx(moveSound1, moveSound2);
        }
            

        checkIfGameOver();

        GameManager.instance.playersTurn = false;
    }

    protected override void OnCantMove<T>(T component)
    {
        Wall hitWall = component as Wall;

        hitWall.damageWall(wallDamage);
        animator.SetTrigger("playerChop");
    }

    private void OnTriggerEnter2D (Collider2D other)
    {
        if (other.tag == "Exit")
        {
            Invoke("Restart", restartLevelDelay);
            enabled=false;
        }
        else if (other.tag == "Food")
        {
            this.food += this.pointsPerFood;
            foodText.text = "+" + pointsPerFood + " Food: " + food;
            SoundManager.instance.RandomizeSfx(eatSound1, eatSound2);
            other.gameObject.SetActive(false);
        }
        else if (other.tag == "Soda")
        {
            this.food += this.pointsPerSoda;
            foodText.text = "+" + pointsPerSoda + " Food: " + food;
            SoundManager.instance.RandomizeSfx(drinkSound1, drinkSound2);
            other.gameObject.SetActive(false);
        }
    }

    private void Restart()
    {
        // Esto hace que se cargue un nuevo nivel (como se generan proceduralmente, no pasamos otro nivel, sino que llamamos al cargador del mismo)
        Application.LoadLevel(Application.loadedLevel);
    }

    public void LoseFood(int loss)
    {
        animator.SetTrigger("playerHit");
        this.food -= loss;
        foodText.text = "-" + loss + " Food: " + food;
        checkIfGameOver();
    }

    private void checkIfGameOver()
    {
        if (food <= 0)
        {
            // Ponemos el sonido de fin de juego y paramos la música
            SoundManager.instance.PlaySingle(gameOverSound);
            SoundManager.instance.musicSource.Stop();
            GameManager.instance.GameOver();
        }
            
    }
}
