using System.Collections;
using UnityEngine;
public class LightningRhino : EnemyAI
{
    [Header("Lightning Rhino")]
    [SerializeField] float chargeTime;
    [SerializeField] float lightningRange;
    [SerializeField] ParticleSystem[] chargeEffect;
    float chargeTimer;
    bool isCharging = false;


    protected override void Update()
    {
        base.Update();
        Debug.DrawRay(projectileSpawn.position, projectileSpawn.forward * lightningRange);
        if (!inAttackAnim)
            attackTimer += Time.deltaTime;
    }

    protected override void AttackState()
    {
        if (inAttackAnim && isCharging)
        {
            ChargeAttack();
        }
        else if (!inAttackAnim)
        {
            if (attackTimer >= attackRate)
            {
                attackTimer = 0;
                inAttackAnim = true;
                isCharging = true;
                for (int i = 0; i < chargeEffect.Length; i++)
                    chargeEffect[i].Play();
            }
        }

    }

    void ChargeAttack()
    {


        if (chargeTimer >= chargeTime)
        {
            chargeTimer = 0;
            isCharging = false;
            for (int i = 0; i < chargeEffect.Length; i++)
                chargeEffect[i].Stop();

            animator.SetTrigger("Attack0");
        }
        else
        {
            chargeTimer += Time.deltaTime;
        }

    }

    public override void RangedAttack0()
    {
        if (projectileSpawn == null)
            return;
        Vector3 offsetPos = new Vector3(targetPoint.x, targetPoint.y + 1.0f, 0);
        Quaternion rot = Quaternion.LookRotation(offsetPos - projectileSpawn.position);
        projectileSpawn.rotation = rot;
        StartCoroutine(PlayAttackEffect());
        RaycastHit hit;
        if (Physics.Raycast(projectileSpawn.position, projectileSpawn.forward, out hit, lightningRange, ~lineOfSightIgnoreLayer))
        {
            if (hit.collider.CompareTag("Player"))
            {
                Idamage dmg = hit.collider.GetComponent<Idamage>();
                if (dmg != null)
                    hit.collider.GetComponent<Idamage>().TakeDamage(projDamage);
            }
        }
    }

    protected override void HitReact()
    {
        base.HitReact();
        if (isCharging)
        {
            isCharging = false;
            chargeTimer = 0;
            for (int i = 0; i < chargeEffect.Length; i++)
                chargeEffect[i].Stop();
        }
    }

    protected override void AttackTransitionCheck()
    {
        if (inAttackAnim && !isCharging)
            return;

        if (DistFromTarget() > chaseStopDist + 0.1f)//0.1 is a slight offset to prevent constant state changes
        {

            if (isCharging)
            {
                isCharging = false;
                chargeTimer = 0;
                for (int i = 0; i < chargeEffect.Length; i++)
                    chargeEffect[i].Stop();
                inAttackAnim = false;
            }
            curState = EnemyState.chase;
        }
    }
}
