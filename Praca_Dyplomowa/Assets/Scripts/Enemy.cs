using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Entity
{
    public EnemyStats stats;

    public string enemyName;

    private Transform playerPosition;

    private bool duringMoving = false;

    protected override void Start()
    {
        if (!this.enemyName.Equals("Guardian"))
        {
            GameManager.instance.AddEnemyToList(this);
        }
        playerPosition = GameObject.FindGameObjectWithTag("Player").transform;
        duringMoving = false;
        base.Start();
    }

    protected override void AttemptMove<T>(int x, int y)
    {
        base.AttemptMove<T>(x, y);
    }

    public void EnemyMove()
    {
        int x = 0;
        int y = 0;
        if (
            Mathf.Abs(playerPosition.position.x - transform.position.x) <
            float.Epsilon
        )
            y = playerPosition.position.y > transform.position.y ? 1 : -1;
        else
            x = playerPosition.position.x > transform.position.x ? 1 : -1;
        AttemptMove<Player> (x, y);
        RaycastHit2D hit;
        if (!Move(x, y, out hit))
        {
            if (hit.transform.GetComponent<Player>() != null)
            {
                duringMoving = false;
                return;
            }
            else if (
                x == 0 &&
                Mathf.Abs(playerPosition.position.y - transform.position.y) >
                float.Epsilon
            )
            {
                y = 0;
                x = playerPosition.position.x > transform.position.x ? 1 : -1;
            }
            else if (
                y == 0 &&
                Mathf.Abs(playerPosition.position.x - transform.position.x) >
                float.Epsilon
            )
            {
                x = 0;
                y = playerPosition.position.y > transform.position.y ? 1 : -1;
            }
            else
            {
                duringMoving = false;
                return;
            }
        }
        AttemptMove<Player> (x, y);
        if (!Move(x, y, out hit))
        {
            duringMoving = false;
        }
    }

    public void startMoving()
    {
        duringMoving = true;
    }

    public bool IsDuringMovement()
    {
        return duringMoving;
    }

    public bool CheckIfMove()
    {
        if (Vector3.Distance(playerPosition.position, transform.position) > 2.9f
        )
        {
            return false;
        }
        return true;
    }

    protected override void SpecialAction<T>(T component)
    {
        Player hitPlayer = component as Player;

        FightManager.instance.StartFight(hitPlayer, this);
    }
}
