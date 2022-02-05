using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    // Start is called before the first frame update
    public float moveTime = 0.1f;

    public LayerMask blockingLayer;

    private BoxCollider2D boxCollider;

    private Rigidbody2D rb2D;

    private float inverseMoveTime;

    protected bool CR_running=false;

    protected virtual void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        rb2D = GetComponent<Rigidbody2D>();
        inverseMoveTime = 1f / moveTime;
    }

    protected bool Move(int x, int y, out RaycastHit2D hit)
    {
        Vector2 start = transform.position;
        Vector2 end = start + new Vector2(x, y);
        boxCollider.enabled = false;
        hit = Physics2D.Linecast(start, end, blockingLayer);
        boxCollider.enabled = true;

        if (hit.transform == null)
        {
            StartCoroutine(SmoothMovement(end));
            return true;
        }
        return false;
    }

    protected virtual void AttemptMove<T>(int x, int y)
        where T : Component
    {
        RaycastHit2D hit;
        bool canMove = Move(x, y, out hit);

        if (hit.transform == null)
        {
            return;
        }
        T hitComponent = hit.transform.GetComponent<T>();

        if (!canMove && hitComponent != null){
            SpecialAction(hitComponent);
        }
    }

    protected abstract void SpecialAction<T> (T component) where T: Component;

    protected IEnumerator SmoothMovement(Vector3 end)
    {
        CR_running=true;
        float sqrRemainingDistance = (transform.position - end).sqrMagnitude;

        while (sqrRemainingDistance > float.Epsilon)
        {
            Vector3 newPosition =
                Vector3
                    .MoveTowards(rb2D.position,
                    end,
                    inverseMoveTime * Time.deltaTime);
            rb2D.MovePosition (newPosition);
            sqrRemainingDistance = (transform.position - end).sqrMagnitude;
            yield return null;
        }
        CR_running=false;
    }

    // Update is called once per frame
    void Update()
    {
    }
}
