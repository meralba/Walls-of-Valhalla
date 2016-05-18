using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;


public class Player : MovingObject {

    public int wallDamage = 1, pointsPerHealthPack = 20, pointsPerSoda = 20;
    public float restartLevelDelay = 1f;
    public Text healthText;

    public AudioClip moveSound1;
    public AudioClip moveSound2;
    public AudioClip eatSound1;
    public AudioClip eatSound2;
    public AudioClip attackSound;

    public AudioClip gameOverSound;

    // Use this for initialization
    protected override void Start () {
        base.Start();

        healthText = GameObject.Find("HealthText").GetComponent<Text>();
        animator = GetComponent<Animator>();
        health = GameManager.instance.playerHealth;
        updateHealthText();
    }

    private void updateHealthText()
    {
        healthText.text = "Health: " + health;
    }

    public void setHealth(int hp)
    {
        this.health = hp;
    }

    private void OnDisable()
    {
        GameManager.instance.playerHealth = this.health;
    }
	
	// Update is called once per frame
	protected override void Update () {

        if (!GameManager.instance.playersTurn || GameManager.instance.controlDisabled)
            return;

        int horizontal = 0, vertical = 0;

        // Código para el teclado
        horizontal = (int)Input.GetAxisRaw("Horizontal");
        vertical = (int)Input.GetAxisRaw("Vertical");

        if (horizontal != 0)
            vertical = 0;

        if ((horizontal + vertical) != 0)
        {
          //AttemptMove<Wall>(horizontal, vertical);
            // Casteamos un rayo para ver el tipo de elemento que tenemos en la dirección en la que queremos movernos
            RaycastHit2D hit;

            // Puntos de inicio y fin del rayo
            Vector2 start = transform.position;
            Vector2 end = start + new Vector2(horizontal, vertical);

            // Desactivamos el collider del jugador
            boxCollider.enabled = false;
            // Lanzamos el rayo
            hit = Physics2D.Linecast(start, end,this.blockingLayer);
            // Acitvamos el collider del jugador
            boxCollider.enabled = true;


            if(hit.transform != null)
            {
                if(hit.collider.tag == "Wall")
                    AttemptMove<Wall>(horizontal, vertical);
                else if(hit.collider.tag == "Enemy")
                    AttemptMove<Enemy>(horizontal, vertical);
            }
            else
                AttemptMove<Wall>(horizontal, vertical);
        }
	}

    protected override void AttemptMove<T>(int xDir, int yDir)
    {
        this.health--;
        updateHealthText();

        base.AttemptMove<T>(xDir, yDir);

        RaycastHit2D hit;
        if(Move(xDir,yDir,out hit))
            SoundManager.instance.RandomizeSfx(moveSound1, moveSound2);


        checkIfGameOver();

        GameManager.instance.playersTurn = false;

    }

    protected override void OnCantMove<T>(T component)
    {
        if( typeof(T) == typeof(Wall))
        {
            Wall hitWall = (Wall) (object) component;

            hitWall.damageWall(wallDamage);
            //animator.SetTrigger("playerChop");
            StartCoroutine(PlayAnimation("playerChop"));
        }
        if( typeof(T) == typeof(Enemy))
        {
            Enemy hitEnemy = (Enemy) (object) component;

            hitEnemy.TakeDamage(damage);

            StartCoroutine(PlayAnimation("playerChop"));
            SoundManager.instance.RandomizeSfx(attackSound);
        }
        
    }

    private void OnTriggerEnter2D (Collider2D other)
    {
        if (other.tag == "Exit")
        {
            GameManager.instance.controlDisabled = true;
            GameManager.instance.mainCamera.transform.SetParent(null);
            Invoke("Restart", restartLevelDelay);
        }
        else if (other.tag == "Health")
        {
            this.health += this.pointsPerHealthPack;
            healthText.text = "+" + pointsPerHealthPack + " Health: " + health;
            SoundManager.instance.RandomizeSfx(eatSound1, eatSound2);
            other.gameObject.SetActive(false);
        }
    }

    private void Restart()
    {
        GameManager.instance.NextLevel();
        // Esto hace que se cargue un nuevo nivel (como se generan proceduralmente, no pasamos otro nivel, sino que llamamos al cargador del mismo)
        //Application.LoadLevel(Application.loadedLevel);
    }

    public void LoseHealth(int loss)
    {
        //animator.SetTrigger("playerHit");
        StartCoroutine(PlayAnimation("playerHit"));
        this.health -= loss;
        healthText.text = "-" + loss + " Health: " + health;
        checkIfGameOver();
    }

    private void checkIfGameOver()
    {
        if (health <= 0)
        {
            // Ponemos el sonido de fin de juego y paramos la música
            SoundManager.instance.PlaySingle(gameOverSound);
            SoundManager.instance.musicSource.Stop();
            StartCoroutine(GameManager.instance.GameOver());
        }      
    }
}
