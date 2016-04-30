using UnityEngine;
using System.Collections;

public class Enemy : MovingEntity {

	// Use this for initialization
	protected override void Start () {
        base.Start();
	}
	
	// Update is called once per frame
	protected override void Update () {
        base.Update();
	}


    protected override void OnCantMove<T>(T component)
    {

    }

    public void MoveEnemy()
    {
    }
}