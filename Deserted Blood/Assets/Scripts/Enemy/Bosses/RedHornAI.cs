using System.Collections;
using UnityEngine;

public class RedHornAI : EnemyAI
{
    [Header("RedHorn Variables")]
    [Tooltip("The position the main cam will be set to during the boss fight")]
    [SerializeField] Transform fightCamPos;
    [SerializeField] GameObject[] projSpawnList;
    [SerializeField] int melee0Damage;
    [SerializeField] int melee1Damage;
    [SerializeField] int melee2Damage;
    [SerializeField] float roarCooldown;

    bool canRoar;

    int attackCalls;
    public bool bossFightTriggered = false;

    [Header("Effect Variables")]
    [SerializeField] ParticleSystem roarEffect;
    [Header("Sound Effect Variables")]
    [SerializeField] AudioClip roarSound;
    [SerializeField] float roarVol;

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
                hitReact = false;
                animator.SetTrigger("Attack0");
                break;
            case 1:
                attackCalls++;
                hitReact = false;
                animator.SetTrigger("Attack1");
                break;
            case 2:
                hitReact = false;
                attackCalls = 0;
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

    public void Attack2()
    {
        //Toggle hitbox
        hitBoxes[2].SetActive(!hitBoxes[2].activeSelf);
        hitBoxes[2].GetComponent<Damage>().damageammount = melee2Damage;
    }

    public override void RangedAttack0()
    {
        Vector3 playerDir = new Vector3(targetPoint.x, targetPoint.y + 1.0f, targetPoint.z) - projSpawnList[0].transform.position;
        EnemyAI add = Instantiate(projectiles[0], projSpawnList[0].transform.position, Quaternion.LookRotation(playerDir)).GetComponent<EnemyAI>();
        add.startState = EnemyState.chase;
        add.enemyAggroRange = 50;
    }

    public void RangedAttack1()
    {
        Vector3 playerDir = new Vector3(targetPoint.x, targetPoint.y + 1.0f, targetPoint.z) - projSpawnList[1].transform.position;
        EnemyAI add = Instantiate(projectiles[1], projSpawnList[1].transform.position, Quaternion.LookRotation(playerDir)).GetComponent<EnemyAI>();
        add.startState = EnemyState.chase;
        add.enemyAggroRange = 50;
    }

    public void RangedAttack2()
    {
        Vector3 playerDir = new Vector3(targetPoint.x, targetPoint.y + 1.0f, targetPoint.z) - projSpawnList[2].transform.position;
        EnemyAI add = Instantiate(projectiles[2], projSpawnList[2].transform.position, Quaternion.LookRotation(playerDir)).GetComponent<EnemyAI>();
        add.startState = EnemyState.chase;
        add.enemyAggroRange = 50;
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
        gameManager.instance.ShowBossBar("Red Horn", maxHealth);
        gameManager.instance.UpdateBossBar(curHealth);
        bossFightTriggered = true;
        CameraController cam = Camera.main.GetComponent<CameraController>();
        if (cam != null && fightCamPos != null)
        {
            cam.SetBossDistance(fightCamPos);
        }
    }

    public void StopBossFight()
    {
        gameManager.instance.HideBossBar();
        gameManager.instance.cameraScript.resetCam();
        curHealth = maxHealth;
        transform.position = startPos;
        bossFightTriggered = false;
    }

    protected override void OnDeath()
    {
        base.OnDeath();
        gameManager.instance.HideBossBar();
        gameManager.instance.playerScript.RedHornAbilityCheck = true;
        CameraController cam = Camera.main.GetComponent<CameraController>();
        if (cam != null)
        {
            cam.resetCam();
        }
    }

    void UpdateUI()
    {
        gameManager.instance.UpdateBossBar(curHealth);
    }

    public void StartRoarEffect()
    {
        if (roarEffect != null)
        {
            roarEffect.Play();
        }
        if (roarSound != null && audSource != null)
            PlaySoundClip(roarSound, roarVol);
    }

    public void StopRoarEffect()
    {
        if (roarEffect != null)
        {
            roarEffect.Stop();
        }
        if (audSource != null)
            audSource.Stop();
    }

    protected override void HitReact()
    {
    }
}
