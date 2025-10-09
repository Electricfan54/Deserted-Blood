
using UnityEngine;

public class SpiderEnemy : EnemyAI
{
    RaycastHit hit;

    protected override void Stopped()
    {
        if (rig.useGravity)
        {
            rig.useGravity = false;
            rig.linearVelocity = Vector3.zero;
        }
    }

    protected override void StoppedTransitionCheck()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 50.0f, ~lineOfSightIgnoreLayer))
        {
            if (hit.collider.CompareTag("Player"))
            {
                rig.useGravity = true;
                transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, 0);
                curState = EnemyState.chase;
            }
        }
    }
}
