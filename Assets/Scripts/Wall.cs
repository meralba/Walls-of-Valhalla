using UnityEngine;
using System.Collections;

public class Wall : MonoBehaviour {

	public Sprite dmgSprite;
	public int hp = 4;
    public AudioClip chopSound1;
    public AudioClip chopSound2;


    private SpriteRenderer spriteRenderer;

	// Use this for initialization
	void Start () {
		spriteRenderer = GetComponent<SpriteRenderer>();
	}

	public void damageWall(int loss){
        
        spriteRenderer.sprite = dmgSprite;
        SoundManager.instance.RandomizeSfx(chopSound1, chopSound2);
        this.hp -= loss;

		if(this.hp <= 0)
			gameObject.SetActive(false);
	}
}
