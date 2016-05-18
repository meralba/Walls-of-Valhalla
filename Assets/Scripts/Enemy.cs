using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Enemy : MovingObject {

    public Transform target;
    private bool skipMove;

    public AudioClip enemyAttack1;
    public AudioClip enemyAttack2;

    // Use this for initialization
    protected override void Start () {
        base.Start();
        animator = GetComponent<Animator>();

        GameManager.instance.AddEnemyToList(this);

        target = null;
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
        if (health <= 0)
            return;
        int xDir = 0, yDir = 0;

        Node currentNode, nextNode, targetNode;
        List<Node> path;

        currentNode = GameManager.instance.pathFinder.gridMap.GetNode(transform);

        if(target == null)
            target = GameObject.FindGameObjectWithTag("Player").transform;

        targetNode = GameManager.instance.pathFinder.gridMap.GetNode(target);


        GameManager.instance.pathFinder.UpdateGrid();

        path = GameManager.instance.pathFinder.FindPath(currentNode, targetNode);

        if (path != null)
        {
            nextNode = path[0];

            xDir = nextNode.x - currentNode.x;
            if (xDir != 0)
                xDir /= Math.Abs(xDir);
            yDir = nextNode.y - currentNode.y;
            if (yDir != 0)
                yDir /= Math.Abs(yDir);
        }


        //Node n = GameManager.instance.pathFinder.gridMap.GetNode(transform);

        AttemptMove<Player>(xDir, yDir);
    }

    protected override void OnCantMove<T>(T component)
    {
        Player hitPlayer = component as Player;

        //animator.SetTrigger("enemyAttack");
        StartCoroutine(PlayAnimation("enemyAttack"));
        SoundManager.instance.RandomizeSfx(enemyAttack1, enemyAttack2);

        hitPlayer.LoseHealth(this.damage);

        //throw new NotImplementedException();
    }

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);
        //animator.SetTrigger("enemyHit");
        if (health <= 0)
        {
            StartCoroutine(PlayAnimation("enemyDeath"));
        }
            
        else
            StartCoroutine(PlayAnimation("enemyHit"));
    }

    protected override void Update()
    {

    }
}
