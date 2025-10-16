using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Events;
using UnityEditorInternal;
using Unity.VisualScripting;

[RequireComponent(typeof(Rigidbody))]
public class EnemyAI : MonoBehaviour, Idamage, IEffect
{
    public enum EnemyState
    {
        stopped,
        roaming,
        chase,
        attacking,
        dead,
    };

    protected Rigidbody rig;
    protected EnemyState curState;
    protected GameObject player;
    protected Vector3 targetPoint;

    public UnityEvent onDeathEvent;

    [SerializeField] protected Renderer meshRenderer;
    [SerializeField] protected LayerMask groundLayer;
    [SerializeField] protected LayerMask lineOfSightIgnoreLayer;
    [SerializeField] protected Animator animator;
    public EnemyState startState;
    public bool isSpecial = false;
    [SerializeField]
    bool noDeathAnim = false;
    [SerializeField]
    float destroyTime = 5.0f;

    Color origColor;

    //The type for passive milestones
    public enum EnemyType
    {
        none,
        golem,
        flying,
        cerberus,
    };
    [SerializeField] protected EnemyType enemyType;
    [SerializeField] protected GameObject abilityDrop;

    [Header("Move Variables")]
    public bool isFlying = false;
    [SerializeField] protected float moveSpeed;
    [SerializeField] protected float acceleration;
    protected float curSpeed;
    [SerializeField] protected float chaseStopDist;

    [Header("Attack Variables")]
    [SerializeField] protected ParticleSystem attackEffect;
    [SerializeField] protected float effectDuration;
    protected bool shouldPlayEffect = false;
    [SerializeField] protected int maxHealth;
    protected int curHealth;
    public int enemyAggroRange;
    [SerializeField] protected float attackRate = 0.5f;
    [SerializeField] protected float hitStunDuration = 0.5f;
    protected float hitStunTimer = 0;
    protected bool hitStunned = false;

    [Header("Melee Variables")]
    [SerializeField] protected List<GameObject> hitBoxes = new List<GameObject>();
    public int meleeDamage;

    [Header("Ranged Variables")]
    [SerializeField] protected List<GameObject> projectiles = new List<GameObject>();
    [SerializeField] protected Transform projectileSpawn;
    public int projDamage;
    [SerializeField] protected float projSpeed;
    [SerializeField] protected float projDestroyTime;


    protected bool inAttackAnim = false;

    protected float attackTimer = 0.0f;

    protected bool isGrounded;

    //For roaming logic
    protected Vector3 startPos;
    protected bool reachedRoamTarget = true;

    [Header("Roaming Variables")]
    [Tooltip("For flying enemies")] [SerializeField] protected LayerMask roamIgnoreLayer;
    [SerializeField] protected float roamDist;
    [SerializeField] protected float roamStopDist;
    [SerializeField] protected float roamPauseTime;
    protected float roamPauseTimer;


    [Header("Player Blood Meter")]
    [SerializeField] int bloodAddAmount;

    // Status Effect variables
    protected float fireDuration;
    protected int fireTickDamage;
    protected float fireTimer;
    protected float fireTickRate;

    protected float freezeDuration;
    protected float origAnimSpeed;
    protected Color beforeFreezeColor;

    protected float StunDuration;
    protected Color beforestunColor;

    protected bool canUpdate = true; //for stopping enemy update
    protected bool canMove = true; //for stopping enemy movement

    //To toggle hit react off for one call of take damage
    protected bool hitReact = true;


    protected virtual void Awake()
    {
        rig = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    protected virtual void Start()
    {
        player = gameManager.instance.player;
        curHealth = maxHealth;
        curState = startState;
        startPos = transform.position;
        reachedRoamTarget = true;
        if (isFlying)
        {
            rig.useGravity = false;
        }
        curSpeed = 0;
        origColor = meshRenderer.material.color;

        attackTimer = attackRate;//When the enemy goes to attack for the first time they dont wait

        canUpdate = true;
        canMove = true;
    }

    protected virtual void Update()
    {
        
        if (fireDuration > 0)
            BurnEffect();

        if (!canUpdate && freezeDuration > 0)
        {
            hitStunned = false;
            FreezeEffect();
            return;
        }  
        else if (!canUpdate && StunDuration > 0)
        {
            hitStunned = false;
            StunEffect();
            return;
        }
        else if (!canUpdate)
        {
            return;
        }

      
       

        UpdateAnimations();

        if (isFlying && curState != EnemyState.dead)
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
            case EnemyState.roaming:
                FaceTarget();
                if (isFlying)
                    AirRoam();
                else
                    GroundRoam();
                RoamingTransitionCheck();
                    break;
            case EnemyState.chase:
                FaceTarget();
                ChaseTransitionCheck();
                break;
            case EnemyState.attacking:
                FaceTarget();
                AttackState();
                AttackTransitionCheck();
                break;
            case EnemyState.dead:
                //OnDeath();
                break;
        }
    }

    protected virtual void FixedUpdate()
    { 
        if (hitStunned || !canMove)
        {
            curSpeed = 0;
            return;
        }


        // Physic updates are fixed
        // fixed update is called before the physics sytem updates
        switch (curState)
        {
            case EnemyState.chase:
                targetPoint = player.transform.position;
                if (!isFlying)
                {
                    //Ground enemy logic
                    if (LedgeCheck())
                    {
                        Move();
                    }
                    else
                        curSpeed = 0;
                }
                else
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

    protected virtual void UpdateAnimations()
    {
        animator.SetFloat("curSpeed", curSpeed / moveSpeed);
    }

    protected virtual void GroundRoam()
    {
        if (LedgeCheck())
        {
            if (DistFromTarget() <= roamStopDist)
            {
                reachedRoamTarget = true;
            }
        }
        else
        {
            reachedRoamTarget = true;
        }

        if (reachedRoamTarget == true && roamPauseTimer >= roamPauseTime)
        {
            roamPauseTimer = 0;
            SetRoamTargetGround();
        }
        else if (reachedRoamTarget == true)
        {
            roamPauseTimer += Time.deltaTime;
        }
    }

    protected virtual bool LedgeCheck()
    {
        // Offsets the ray position on the y and local x
        Vector3 rayPos = new Vector3(transform.position.x, transform.position.y + 0.1f, transform.position.z) + transform.right * .2f;
        if (Physics.Raycast(rayPos, Vector3.down, 0.5f, groundLayer))
        {
            return true;
        }
        return false;
    }

    protected virtual void SetRoamTargetGround()
    {
        reachedRoamTarget = false;

        float xDir = startPos.x - transform.position.x;

        if (xDir != 0)
        {
            FlipDir();//Turn to face target
        }

        RaycastHit hit;
        if (xDir >= 0)
        {
            if (Physics.Raycast(new Vector3(startPos.x, startPos.y + 0.1f, startPos.z), Vector3.right, out hit, roamDist, ~roamIgnoreLayer))
            {
                targetPoint = hit.point;
            }
            else
                targetPoint = startPos + (Vector3.right * roamDist);
        }
        else
        {
            if (Physics.Raycast(new Vector3(startPos.x, startPos.y + 0.1f, startPos.z), -Vector3.right, out hit, roamDist, ~roamIgnoreLayer))
            {
                targetPoint = hit.point;
            }
            else
                targetPoint = startPos + (-Vector3.right * roamDist);
        }
    }

    protected virtual void AirRoam()
    {
        if (DistFromTarget() <= roamStopDist)
        {
            reachedRoamTarget = true;
        }

        if (reachedRoamTarget == true && roamPauseTimer >= roamPauseTime)
        {
            roamPauseTimer = 0;
            SetRoamTargetAir();
        }
        else if (reachedRoamTarget == true)
        {
            roamPauseTimer += Time.deltaTime;
        }
    }
    protected virtual void SetRoamTargetAir()
    {
        reachedRoamTarget = false;

        //Get a point within a circle with radius of roamDist
        Vector3 pos = Random.insideUnitSphere * roamDist;
        pos = startPos + pos;
        pos.z = 0;

        //Check if enemy has line of sight
        Vector3 dir = pos - transform.position;
        RaycastHit hit;
        if (Physics.Raycast(transform.position, dir, out hit, roamDist, ~roamIgnoreLayer))
        {
            targetPoint = hit.point;
        }
        else
        {
            targetPoint = pos;
        }
    }

    protected virtual void GroundCheck()
    {
        Vector3 rayPos = new Vector3(transform.position.x, transform.position.y + 0.1f, transform.position.z);
        if (Physics.Raycast(rayPos, Vector3.down, 0.2f, groundLayer))
        {
            isGrounded = true;
        }
        else
            isGrounded = false;
    }

    protected virtual void Move()
    {

        curSpeed = Mathf.Lerp(curSpeed, moveSpeed, acceleration * Time.fixedTime);
        Vector3 dir = targetPoint - transform.position;

        if (isFlying)
        {
            dir = dir.normalized * curSpeed;
        }
        else
        {
            dir.y = 0;
            dir = dir.normalized * curSpeed;
            dir.y = rig.linearVelocity.y;
        }   

            rig.linearVelocity = dir;
    }

    protected virtual void AttackState()
    {
        if (inAttackAnim)
            return;
        attackTimer += Time.deltaTime;
        if (attackTimer >= attackRate)
        {
            inAttackAnim = true;
            attackTimer = 0;
            animator.SetTrigger("Attack0");
        }
    }

    // Will be called by the attack animation
    public virtual void Attack0()
    {
        if (hitStunned)
            return;

        //toggles effect
        shouldPlayEffect = !shouldPlayEffect;
        if (shouldPlayEffect)
            StartCoroutine(PlayAttackEffect());

        //Toggle hitbox
        hitBoxes[0].SetActive(!hitBoxes[0].activeSelf);
        hitBoxes[0].GetComponent<Damage>().damageammount = meleeDamage;
    }

    public virtual void RangedAttack0()
    {
        if (projectileSpawn == null)
            return;

        //toggles effect
        shouldPlayEffect = !shouldPlayEffect;
        if (shouldPlayEffect)
            StartCoroutine(PlayAttackEffect());

        Vector3 playerDir = new Vector3(targetPoint.x, targetPoint.y + 1.0f, targetPoint.z) - projectileSpawn.transform.position;
        GameObject proj = Instantiate(projectiles[0], projectileSpawn.transform.position, Quaternion.LookRotation(playerDir));
        Projectile projScript = proj.GetComponent<Projectile>();
        projScript.dmg.damageammount = projDamage;
        projScript.speed = projSpeed;
        projScript.destroytime = projDestroyTime;
    }

    protected virtual void HitReact()
    {
        hitStunned = true;
        animator.SetTrigger("hit");
        //Make shure there is no active hitboxes
        for (int i = 0; i < hitBoxes.Count; i++)
        {
            hitBoxes[i].SetActive(false);
        }
    }

    public virtual void AttackAnimEnd()
    {
        inAttackAnim = false;
    }

    protected virtual void StoppedTransitionCheck()
    {

    }

    protected virtual void RoamingTransitionCheck()
    {
        if (CanSeePlayer())
        {
            curState = EnemyState.chase;
        }
    }

    protected virtual float DistFromTarget()
    {
        if (isFlying)
        {
            return (targetPoint - transform.position).magnitude;
        }
        else
        {
            return Mathf.Abs(targetPoint.x - transform.position.x);
        }
    }

    protected virtual void ChaseTransitionCheck()
    {
        if (DistFromTarget() <= chaseStopDist)
        {
            curSpeed = 0;
            curState = EnemyState.attacking;
        }
        else if (DistFromTarget() > enemyAggroRange)
        {
            curState = EnemyState.roaming;
        }
    }

    protected virtual void AttackTransitionCheck()
    {
        if (inAttackAnim)
            return;
        if (DistFromTarget() > chaseStopDist + 0.1f)//0.1 is a slight offset to prevent constant state changes
        {
            curState = EnemyState.chase; 
        }
    }

    protected virtual void FaceTarget()
    {
        float xDir = targetPoint.x - transform.position.x;
        if (xDir >= 0)
        {
            transform.rotation = Quaternion.identity;
        }
        else
        {
            transform.eulerAngles = new Vector3(0,180,0);
        }
    }

    protected virtual void FlipDir()
    {
        transform.Rotate(0, 180, 0);
    }

    protected virtual bool CanSeePlayer()
    {
        Vector3 playerDir = player.transform.position - transform.position;
        RaycastHit hit;
        if (Physics.Raycast(transform.position, playerDir, out hit, enemyAggroRange, ~lineOfSightIgnoreLayer))
        {
            if (hit.collider.CompareTag("Player"))
            {
                return true;
            }
        }
        return false;
    }

    protected void DropAbility()
    {
        if (abilityDrop != null)
            Instantiate(abilityDrop, transform.position, Quaternion.identity);
    }

    protected void AddToMilestone()
    {
        PlayerPassive.Instance.AddToMilestone(enemyType, isSpecial);
    }

    public virtual void TakeDamage(int damageAmount)
    {
        curHealth -= damageAmount;
        if (curHealth <= 0)
        {
            curHealth = 0;
            curState = EnemyState.dead;
            OnDeath();
            return;
        }
        StartCoroutine(FlashRed());

        if (hitReact)
            HitReact();
        else
            hitReact = true;
    }

    protected virtual void OnDeath()
    {
        AddToMilestone();
        DropAbility();
        onDeathEvent?.Invoke();
        gameManager.instance.player.GetComponent<PlayerController>().AddBloodAmount(bloodAddAmount);
        gameObject.layer = LayerMask.NameToLayer("Dead");
        if (noDeathAnim)
        {
            Destroy(gameObject, destroyTime);
            return;
        }
        if (isFlying)
        {
            rig.useGravity = true;
            rig.linearVelocity = new Vector3(0, 5, 0);
        }
        animator.SetTrigger("dead");
        Destroy(gameObject, destroyTime);
    }

    protected virtual void Stopped()
    {

    }

    protected virtual IEnumerator FlashRed()
    {
        meshRenderer.material.color = Color.red;
        meshRenderer.material.color = new Color(meshRenderer.material.color.r, meshRenderer.material.color.g, meshRenderer.material.color.b, 0.5f);
        yield return new WaitForSeconds(.1f);
        meshRenderer.material.color = origColor;
    }

    public virtual void ApplyBurnEffect(float duration, int tickDamage, float tickrate)
    {
        fireDuration = duration;
        fireTickDamage = tickDamage;
        fireTickRate = tickrate;
        fireTimer = 0;
    }

    public virtual void ApplyFreezeEffect(float duration)
    {
        freezeDuration = duration;
        canMove = false;
        canUpdate = false;

        if (animator.speed != 0)
        {
            origAnimSpeed = animator.speed;
            animator.speed = 0;// Pause animation
            DeactivateHitboxes();
        }

        if (meshRenderer.material.color != Color.blue)
        {
            if (meshRenderer.material.color == Color.red)
            {
                beforeFreezeColor = origColor;
                origColor = Color.blue;
            }
            else
                beforeFreezeColor = meshRenderer.material.color;
            meshRenderer.material.color = Color.blue;
        }
    }
    public void ApplyStunEffect(float duration)
    {
        StunDuration = duration;
        canMove = false;
        canUpdate = false;

        if (animator.speed != 0)
        {
            origAnimSpeed = animator.speed;
            animator.speed = 0;
            DeactivateHitboxes();
        }

        if (meshRenderer.material.color != Color.yellow)
        {
            if (meshRenderer.material.color == Color.red)
            {
                beforestunColor = origColor;
                origColor = Color.yellow;
            }
            else
                beforestunColor = meshRenderer.material.color;
            meshRenderer.material.color = Color.yellow;
        }
    }
    protected virtual void BurnEffect()
    {
        fireDuration -= Time.deltaTime;
        fireTimer += Time.deltaTime;
        if (fireTimer >= fireTickRate)
        {
            fireTimer = 0;
            hitReact = false;
            TakeDamage(fireTickDamage);
        }
    }

    protected virtual void FreezeEffect()
    {
        freezeDuration -= Time.deltaTime;
        if (freezeDuration <= 0)
        {
            freezeDuration = 0;
            canMove = true;
            canUpdate = true;
            animator.speed = origAnimSpeed;
            meshRenderer.material.color = beforeFreezeColor;
        }
    }

    protected virtual void StunEffect()
    {
        StunDuration -= Time.deltaTime;
        if (StunDuration <= 0)
        {
            StunDuration = 0;
            canMove = true;
            canUpdate = true;
            animator.speed = origAnimSpeed;
            meshRenderer.material.color = beforestunColor;
        }
    }

    protected virtual IEnumerator PlayAttackEffect()
    {
        if (attackEffect != null)
        {
            attackEffect.Play();
            yield return new WaitForSeconds(effectDuration);
            attackEffect.Stop();
        }
    }

    protected virtual void DeactivateHitboxes()
    {
        for (int i = 0; i < hitBoxes.Count; i++)
        {
            hitBoxes[i].SetActive(false);
        }
    }
}
