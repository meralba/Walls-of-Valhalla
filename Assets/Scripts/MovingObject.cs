using UnityEngine;
using System.Collections;

public abstract class MovingObject : MonoBehaviour {

    public float moveTime = 0.1f;
    public LayerMask blockingLayer;
    public int maxHealth=100;
    public int damage = 0;

    protected BoxCollider2D boxCollider;
    private Rigidbody2D rb2D;
    private float inverseMoveTime;
    protected int health;
    

	// Use this for initialization
	protected virtual void Start () {
        this.boxCollider = GetComponent<BoxCollider2D>();
        this.rb2D = GetComponent<Rigidbody2D>();
        this.inverseMoveTime = 1f / this.moveTime;
        this.health = this.maxHealth;
	}

    protected virtual void Update()
    {
        if (health <= 0)
        {
          // StartCoroutine( animationDeath());
            Destroy(gameObject, 5);
        }
    }

    IEnumerator animationDeath()
    {
        yield return new WaitForSeconds(2);
        Destroy(gameObject);
    }
    // Echarle un vistazo a qué es IEnumeratork
    protected IEnumerator SmoothMovement(Vector3 end)
    {
        float sqrRemainingDistance = (transform.position - end).sqrMagnitude;

        while (sqrRemainingDistance > float.Epsilon)
        {
            // Echarle un vistazo a qué es deltatime
            Vector3 newPosition = Vector3.MoveTowards(rb2D.position, end, inverseMoveTime * Time.deltaTime);
            rb2D.MovePosition(newPosition);

            sqrRemainingDistance = (transform.position - end).sqrMagnitude;
            // Esto hace que esperemos un frame para volver a ejecutar el ciclo while
            yield return null;
        }
    }

    public virtual void TakeDamage(int damage)
    {
        this.health-=damage;
    }

    protected bool Move ( int xDir, int yDir, out RaycastHit2D hit)
    {
        Vector2 start = transform.position;
        Vector2 end = start + new Vector2(xDir, yDir);

        boxCollider.enabled = false;
        hit = Physics2D.Linecast(start, end,this.blockingLayer);
        boxCollider.enabled = true;

        if(hit.transform == null)
        {
            StartCoroutine(SmoothMovement(end));
            return true;
        }

        return false;
    }

    protected virtual void AttemptMove <T> (int xDir, int yDir )
        where T : Component
    {
        RaycastHit2D hit;
        bool canMove = Move(xDir, yDir, out hit);

        if (hit.transform == null)
            return;

        T hitComponent = hit.transform.GetComponent<T>();

        if (!canMove && hitComponent != null)
            OnCantMove(hitComponent);
    }

    protected abstract void OnCantMove<T>(T component)
        where T : Component;


}
