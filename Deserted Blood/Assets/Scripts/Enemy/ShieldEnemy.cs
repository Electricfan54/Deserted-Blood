using Unity.VisualScripting;
using UnityEngine;

public class ShieldEnemy : EnemyAI
{

    protected override void FixedUpdate()
    {
        if (hitStunned || !canMove)
        {
            return;
        }


        // Physic updates are fixed
        // fixed update is called before the physics sytem updates
        switch (curState)
        {
            case EnemyState.roaming:
                if (reachedRoamTarget == false)
                    Move();
                else
                    curSpeed = 0;
                break;
            case EnemyState.chase:
                curSpeed = 0;
                break;
            case EnemyState.attacking:
                targetPoint = player.transform.position;
                break;
            default:
                curSpeed = 0;
                break;
        }
    }

    protected override void UpdateAnimations()
    {
        base.UpdateAnimations();
        if (curState == EnemyState.attacking)
        {
            animator.SetBool("isGuarding", true);
        }
        else
        {
            animator.SetBool("isGuarding", false);
        }

    }

    protected override void ChaseTransitionCheck()
    {
        curState = EnemyState.attacking;
    }

    protected override void AttackTransitionCheck()
    {
        if (!CanSeePlayer() && DistFromTarget() > enemyAggroRange + 0.5f)//Slight offset to prevent weird behavior
        {
            curState = EnemyState.roaming;
        }
    }

    protected override void OnDeath()
    {
        base.OnDeath();
    }

}
