using UnityEngine;
using System.Collections;

public class OrcKingAI : EnemyAI
{
    [Header("OrcKing Variables")]
    [Tooltip("The position the main cam will be set to during the boss fight")]
    [SerializeField] Transform fightCamPos;
    [SerializeField] GameObject[] projSpawnPointList;
    [SerializeField] GameObject[] enemySpawnPointList;
    [SerializeField] GameObject[] enemySpawnList;
    [SerializeField] int melee0Damage;
    [SerializeField] int melee1Damage;
    [SerializeField] int melee2Damage;
    [SerializeField] float roarCooldown;
    [SerializeField] float projSpawnRate;
    public bool bossFightTriggered = false;

    int attackHitboxCounter;

    [Header("Effects")]
    [SerializeField] ParticleSystem summonRoarEffect;
    [SerializeField] AudioClip summonRoarSound;
    [SerializeField] float summonRoarVol;
    [SerializeField] ParticleSystem projRoarEffect;
    [SerializeField] AudioClip projRoarSound;
    [SerializeField] float projRoarVol;


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
                hitReact = false;
                animator.SetTrigger("Attack0");
                break;
            case 1:
                attackCalls = 0;
                hitReact = false;
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
        switch (attackHitboxCounter)
        {
            case 0:
                attackHitboxCounter++;
                hitBoxes[0].SetActive(true);
                hitBoxes[0].GetComponent<Damage>().damageammount = melee0Damage;
                break;
            case 1:
                attackHitboxCounter++;
                hitBoxes[0].SetActive(false);
                break;
            case 2:
                attackHitboxCounter++;
                hitBoxes[2].SetActive(true);
                hitBoxes[2].GetComponent<Damage>().damageammount = melee0Damage;
                break;
            case 3:
                attackHitboxCounter = 0;
                hitBoxes[2].SetActive(false);
                break;
        }
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
        gameManager.instance.orcKingRef = this;
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
        CameraController cam = Camera.main.GetComponent<CameraController>();
        if (cam != null)
        {
            cam.resetCam();
        }
        gameManager.instance.ShowWinScreen();
    }

    void UpdateUI()
    {
        gameManager.instance.UpdateBossBar(curHealth);
    }

    public void StartRoarEffect()
    {
        if (screenShake)
        {
            ScreenShake.instance.ShakeScreen(screenShakeDuration, screenShakeStrength);
        }

        switch (roarType)
        {
            case RoarType.Enemy:
                if (summonRoarEffect != null)
                    summonRoarEffect.Play();
                if (summonRoarSound != null)
                    PlaySoundClip(summonRoarSound, summonRoarVol);
                break;
            case RoarType.Proj:
                if (projRoarEffect != null)
                    projRoarEffect.Play();
                if (projRoarSound != null)
                    PlaySoundClip(projRoarSound, projRoarVol);
                break;
        }
    }

    public void StopRoarEffect()
    {
        if (summonRoarEffect != null && projRoarEffect != null)
        {
            summonRoarEffect.Stop();
            projRoarEffect.Stop();
        }
        if (audSource != null)
            audSource.Stop();
    }

    public override void PlayAttackSFX()
    {
        if (attackSounds.Length < 1)
            return;
        if (attackCalls == 1)
            PlaySoundClip(attackSounds[0], attackVol);
        else
            PlaySoundClip(attackSounds[1], attackVol);
    }

    protected override void HitReact()
    {
    }
}
