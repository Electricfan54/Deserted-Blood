using UnityEngine;
using System.Collections;

public class OrcKingAI : EnemyAI
{
    [Header("RedHorn Variables")]
    [SerializeField] Vector3 cameraFocusPos;
    [SerializeField] Vector3 camDistance;
    [SerializeField] GameObject[] projSpawnPointList;
    [SerializeField] GameObject[] enemySpawnPointList;
    [SerializeField] GameObject[] enemySpawnList;
    [SerializeField] int melee0Damage;
    [SerializeField] int melee1Damage;
    [SerializeField] int melee2Damage;
    [SerializeField] float roarCooldown;
    [SerializeField] float projSpawnRate;
    bool bossFightTriggered = false;


    enum RoarType
    {
        Enemy = 0,
        Proj,
    }
    RoarType roarType;

    bool canRoar;

    int attackCalls;

    protected override void Start()
    {
        base.Start();
        attackCalls = 0;
        canRoar = true;
        bossFightTriggered = false;
    }


    protected override void Update()
    {
        if (!bossFightTriggered)
            return;
        else
            UpdateUI();
        base.Update();
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
                hitReact = false;
                attackCalls = 0;
                animator.SetTrigger("Attack1");
                break;

        }
    }

    void RangedAttack()
    {
        inAttackAnim = true;
        attackTimer = 0;
        rig.linearVelocity = Vector3.zero;
        curSpeed = 0;
        int rand = Random.Range(0, 2);
        roarType = (RoarType)rand;

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
                StartCoroutine(RoarCooldown());
                RangedAttack();
                curState = EnemyState.attacking;
            }
        }
    }

    public override void Attack0()
    {
        //Toggle hitbox
        hitBoxes[0].SetActive(!hitBoxes[0].activeSelf);
        hitBoxes[0].GetComponent<Damage>().damageammount = melee0Damage;
    }

    public void Attack1()
    {
        //Toggle hitbox
        hitBoxes[1].SetActive(!hitBoxes[1].activeSelf);
        hitBoxes[1].GetComponent<Damage>().damageammount = melee1Damage;
    }

    public override void RangedAttack0()
    {
        
    }

    public void SpawnEnemies()
    {
        for (int i = 0; i < enemySpawnList.Length && i < enemySpawnPointList.Length; i++)
        {
            EnemyAI add = Instantiate(enemySpawnList[i], enemySpawnPointList[i].transform.position, transform.rotation).GetComponent<EnemyAI>();
            add.startState = EnemyState.chase;
            add.enemyAggroRange = 50;
        }
    }

    IEnumerator SpawnProjectiles()
    {
        for (int i = 0; i < projSpawnPointList.Length; i++)
        {
            int x = i % projectiles.Count;
            Projectile proj = Instantiate(projectiles[x], projSpawnPointList[i].transform.position, projSpawnPointList[i].transform.rotation).GetComponent<Projectile>();
            proj.speed = projSpeed;
            proj.destroytime = projDestroyTime;
            proj.dmg.damageammount = projDamage;
            yield return new WaitForSeconds(projSpawnRate);
        }
    }

    public void Roar()
    {
        //Use the right roar
        switch (roarType)
        {
            case RoarType.Enemy:
                SpawnEnemies(); 
                break;
            case RoarType.Proj:
                StartCoroutine(SpawnProjectiles());
                break;
        }
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

    public override void TakeDamage(int damageAmount)
    {
        base.TakeDamage(damageAmount);
    }

    public void StartBossFight()
    {
        gameManager.instance.ShowBossBar("Orc King", maxHealth);
        gameManager.instance.UpdateBossBar(curHealth);
        bossFightTriggered = true;
        CameraController cam = Camera.main.GetComponent<CameraController>();
        if (cam != null && camDistance != Vector3.zero)
        {
            cam.SetBossDistance(camDistance);
        }
    }

    void UpdateUI()
    {
        gameManager.instance.UpdateBossBar(curHealth);
    }
}
