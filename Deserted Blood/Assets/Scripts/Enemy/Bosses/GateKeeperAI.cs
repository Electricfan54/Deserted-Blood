using UnityEngine;
using System.Collections;
using System.Linq;

public class GateKeeperAI : EnemyAI
{

    [Header("GateKeeper Variables")]
    [SerializeField] GameObject[] projSpawnPointList;
    [SerializeField] int melee0Damage;
    [SerializeField] int melee1Damage;
    [SerializeField] int slamDamage;
    [SerializeField] float roarCooldown;
    [SerializeField] float jumpCooldown;

    bool canRoar;
    bool canJump;
    bool bossFightTriggered;

    int attackCalls;

    protected override void Awake()
    {
        base.Awake();
        if (projSpawnPointList.Length < 3)
            projSpawnPointList = new GameObject[3];
    }

    protected override void Start()
    {
        base.Start();
        attackCalls = 0;
        canRoar = true;
        canJump = true;
        bossFightTriggered = false;
    }


    protected override void Update()
    {
        if (!bossFightTriggered)
            return;
        else
            UpdateUI();

        if (fireDuration > 0)
            BurnEffect();

        if (!canUpdate && freezeDuration > 0)
        {
            hitStunned = false;
            FreezeEffect();
            return;
        }
        else if (!canUpdate)
        {
            return;
        }

        UpdateAnimations();

        if (projectileSpawn != null && inAttackAnim)
        {
            projectileSpawn.position = new Vector3(targetPoint.x, projectileSpawn.position.y, 0);
        }

        if (isFlying)
        {
            rig.linearVelocity = Vector3.zero;
        }
        else
            rig.linearVelocity = new Vector3(0, rig.linearVelocity.y, 0);

        if (hitStunned)
        {
            if (hitStunTimer >= hitStunDuration)
            {
                hitStunned = false;
                hitStunTimer = 0;
            }
            else
            {
                hitStunTimer += Time.deltaTime;
                return;
            }
        }

        GroundCheck();

        switch (curState)
        {
            case EnemyState.stopped:
                //for idle or spider
                Stopped();
                StoppedTransitionCheck();
                break;
            case EnemyState.chase:
                FaceTarget();
                ChaseTransitionCheck();
                break;
            case EnemyState.attacking:
                FaceTarget();
                AttackTransitionCheck();
                AttackState();
                break;
            case EnemyState.dead:
                AddToMilestone();
                DropAbility();
                Destroy(gameObject);
                break;
        }
    }
    protected override void FixedUpdate()
    {
        if (!bossFightTriggered)
            return;

        if (hitStunned || !canMove)
        {
            curSpeed = 0;
            return;
        }

        switch (curState)
        {
            case EnemyState.chase:
                targetPoint = player.transform.position;
                Move();
                break;
            case EnemyState.roaming:
                if (reachedRoamTarget == false)
                    Move();
                else
                    curSpeed = 0;
                break;
            case EnemyState.attacking:
                targetPoint = player.transform.position;
                break;
        }
    }

    protected override void AttackState()
    {
        if (inAttackAnim)
            return;

        attackTimer += Time.deltaTime;
        if (attackTimer >= attackRate)
        {
            inAttackAnim = true;
            attackTimer = 0;
            MeleeAttack();
        }
    }

    void MeleeAttack()
    {
        switch (attackCalls)
        {
            case 0:
                attackCalls++;
                animator.SetTrigger("Attack0");
                break;
            case 1:
                attackCalls++;
                attackCalls = 0;
                animator.SetTrigger("Attack1");
                break;
            case 2:
                hitReact = false;
                animator.SetTrigger("Attack2");
                break;

        }
    }

    void RangedAttack()
    {
        inAttackAnim = true;
        attackTimer = 0;
        rig.linearVelocity = Vector3.zero;
        curSpeed = 0;
        animator.SetTrigger("Attack3");
    }

    protected override void AttackTransitionCheck()
    {
        if (inAttackAnim)
            return;
        if (DistFromTarget() > chaseStopDist + 0.1f)//0.1 is a slight offset to prevent constant state changes
        {
            attackCalls = 0;
            curState = EnemyState.chase;
        }
    }

    protected override void ChaseTransitionCheck()
    {
        if (DistFromTarget() <= chaseStopDist)
        {
            curSpeed = 0;
            curState = EnemyState.attacking;
        }
        else if (DistFromTarget() > enemyAggroRange)
        {
            if (!inAttackAnim && canRoar)
            {
                canRoar = false;
                curSpeed = 0;
                StartCoroutine(RoarCooldown());
                RangedAttack();
                curState = EnemyState.attacking;
            }
            else if (!inAttackAnim && canJump)
            {
                canJump = false;
                StartCoroutine(JumpCooldown());
                JumpAttack();
                curState = EnemyState.attacking;
            }
        }
    }

    public override void Attack0()
    {
        if (hitStunned)
            return;
        //Toggle hitbox
        hitBoxes[0].SetActive(!hitBoxes[0].activeSelf);
        hitBoxes[0].GetComponent<Damage>().damageammount = melee0Damage;
    }

    public void Attack1()
    {
        if (hitStunned)
            return;
        //Toggle hitbox
        hitBoxes[1].SetActive(!hitBoxes[1].activeSelf);
        hitBoxes[1].GetComponent<Damage>().damageammount = melee1Damage;
    }

    public void Attack2()
    {
        if (hitStunned)
            return;
        //Toggle hitbox
        hitBoxes[2].SetActive(!hitBoxes[2].activeSelf);
        hitBoxes[2].GetComponent<Damage>().damageammount = slamDamage;
    }

    public override void RangedAttack0()
    {
        if (projSpawnPointList[0] == null)
            return;
        Projectile proj = Instantiate(projectiles[0], projSpawnPointList[0].transform.position, projSpawnPointList[0].transform.rotation).GetComponent<Projectile>();
        proj.speed = projSpeed;
        proj.destroytime = projDestroyTime;
        proj.dmg.damageammount = projDamage;
    }

    public void RangedAttack1()
    {
        if (projSpawnPointList[1] == null)
            return;
        Projectile proj = Instantiate(projectiles[1], projSpawnPointList[1].transform.position, projSpawnPointList[1].transform.rotation).GetComponent<Projectile>();
        proj.speed = projSpeed;
        proj.destroytime = projDestroyTime;
        proj.dmg.damageammount = projDamage;
    }

    public void RangedAttack2()
    {
        if (projSpawnPointList[2] == null)
            return;
        Projectile proj = Instantiate(projectiles[2], projSpawnPointList[2].transform.position, projSpawnPointList[2].transform.rotation).GetComponent<Projectile>();
        proj.speed = projSpeed;
        proj.destroytime = projDestroyTime;
        proj.dmg.damageammount = projDamage;
    }


    public override void AttackAnimEnd()
    {
        base.AttackAnimEnd();
        if (hitReact == false)
            hitReact = true;
    }

    IEnumerator RoarCooldown()
    {
        yield return new WaitForSeconds(roarCooldown);
        canRoar = true;
    }

    IEnumerator JumpCooldown()
    {
        yield return new WaitForSeconds(jumpCooldown);
        canJump = true;
    }

    public override void TakeDamage(int damageAmount)
    {
        base.TakeDamage(damageAmount);
    }

    void JumpAttack()
    {
        hitReact = false;
        //curSpeed = jumpAttackMoveSpeed;
        animator.SetTrigger("Attack2");
    }

    public void StopMovement()
    {
        curSpeed = 0;
        rig.linearVelocity = Vector3.zero;
        canMove = false;
    }

    public void ResumeMovement()
    {
        canMove = true;
    }

    public void StartBossFight()
    {
        gameManager.instance.ShowBossBar("Orc King", maxHealth);
        gameManager.instance.UpdateBossBar(curHealth);
        bossFightTriggered = true;
    }

    void UpdateUI()
    {
        gameManager.instance.UpdateBossBar(curHealth);
    }
}
