using UnityEngine;
using System.Collections;
using System;

public class Enemy : MovingObject {

    public int playerDamage;

    private Animator animator;
    private Transform target;
    private bool skipMove;

    public AudioClip enemyAttack1;
    public AudioClip enemyAttack2;

    // Use this for initialization
    protected override void Start () {
        animator = GetComponent<Animator>();

        GameManager.instance.AddEnemyToList(this);

        target = GameObject.FindGameObjectWithTag("Player").transform;
        base.Start();
	}
	
    protected override void AttemptMove<T>(int xDir, int yDir)
    {
        // Comprueba si se salta el turno, y si es cierto, lo desactiva y acaba
        if (skipMove)
        {
            skipMove = false;
            return;
        }
        else
            skipMove = true;

        base.AttemptMove<T>(xDir, yDir);
    }

    public void MoveEnemy()
    {
        int xDir = 0, yDir = 0;

        // Comprueba si está en la columna del jugador
        if (Mathf.Abs(target.position.x - transform.position.x) < float.Epsilon)
            // Nos movemos hacia la dirección vertical donde esté el jugador
            yDir = target.position.y > transform.position.y ? 1 : -1;
        // Si no, estará en la fila, y hacemos lo mismo para la dirección horizontal
        else
            xDir = target.position.x > transform.position.x ? 1 : -1;

        AttemptMove<Player>(xDir, yDir);
    }

    protected override void OnCantMove<T>(T component)
    {
        Player hitPlayer = component as Player;

        animator.SetTrigger("enemyAttack");
        SoundManager.instance.RandomizeSfx(enemyAttack1, enemyAttack2);

        hitPlayer.LoseFood(this.playerDamage);

        //throw new NotImplementedException();
    }

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);
        animator.SetTrigger("enemyHit");
    }

    protected override void Update()
    {
        if(health<=0)
            animator.SetTrigger("enemyDeath");
        base.Update();

    }
}
