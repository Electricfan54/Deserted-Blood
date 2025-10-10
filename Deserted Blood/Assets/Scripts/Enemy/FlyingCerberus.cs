using UnityEngine;

public class FlyingCerberus : EnemyAI
{
    [Header("Flying Cerberus Variables")]
    [SerializeField] GameObject model;
    [SerializeField] ParticleSystem[] fireEffects;
    [SerializeField] float fireTick = 0.2f;
    float flameTimer;
    bool isFiring;

    // Flying anims and get hit have different animation Y values
    // This is for offseting the model position to fix the difference in Y values
    Vector3 origModelPos;

    //Local variables made global for performance
    Vector3 offsetPos;
    Quaternion rot;

    protected override void Start()
    {
        base.Start();
        origModelPos = model.transform.localPosition;
    }

    protected override void Update()
    {
        base.Update();

        if (inAttackAnim)
        {
            offsetPos = new Vector3(targetPoint.x, targetPoint.y + 1.0f, targetPoint.z);
            rot = Quaternion.LookRotation(offsetPos - projectileSpawn.position);
            projectileSpawn.rotation = rot;

            if (flameTimer >= fireTick && isFiring)
            {
                flameTimer = 0;
                Projectile fire = Instantiate(projectiles[0], projectileSpawn.transform).GetComponent<Projectile>();
                fire.speed = projSpeed;
                fire.destroytime = projDestroyTime;
                fire.dmg.damageammount = projDamage;
            }
            else
            {
                flameTimer += Time.deltaTime;
            }
        }

    }

    protected override void AttackState()
    {
        base.AttackState();
    }

    public override void RangedAttack0()
    {
        if (projectileSpawn == null)
            return;
        // The flamethrower particle effect has multiple particle systems
        foreach (var fireEffect in fireEffects)
        {
            if (fireEffect.isPlaying)
            {
                fireEffect.Stop();
                isFiring = false;
            }
            else
            {
                fireEffect.Play();
                isFiring = true;
            }
        }
    }

    public override void AttackAnimEnd()
    {
        if (model.transform.localPosition != origModelPos)
        {
            model.transform.localPosition = origModelPos;
        }
        base.AttackAnimEnd();
    }

    protected override void HitReact()
    {
        base.HitReact();
        model.transform.localPosition = new Vector3(origModelPos.x, origModelPos.y - origModelPos.y, origModelPos.z);
    }
}
