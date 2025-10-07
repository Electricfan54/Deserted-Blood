using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody))]
public class EnemyAI : MonoBehaviour
{
    protected enum EnemyState
    {
        stopped,
        roaming,
        chase,
        attacking,
    };

    Rigidbody rig;
    protected EnemyState curState;
    //Temporary serialize for testing
    [SerializeField]
    protected GameObject player;
    protected Vector3 targetPoint;

    [SerializeField] protected LayerMask groundLayer;
    [SerializeField] protected LayerMask lineOfSightIgnoreLayer;
    [SerializeField] protected Animator animator;
    [SerializeField] protected EnemyState startState;
    public bool isSpecial = false;

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
    [SerializeField] protected float chaseStopDist;

    [Header("Attack Variables")]
    [SerializeField] protected int enemyAggroRange;
    [SerializeField] protected float attackRate = 0.5f;
    [Tooltip("For melee enemies")]
    [SerializeField] protected List<GameObject> hitBoxes = new List<GameObject>();
    [Tooltip("For ranged enemies")]
    [SerializeField] protected List<GameObject> projectiles = new List<GameObject>();
    [SerializeField] protected Transform projectileSpawn;

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



    private void Awake()
    {
        rig = GetComponent<Rigidbody>();
    }

    void Start()
    {
        curState = startState;
        startPos = transform.position;
        reachedRoamTarget = true;
        if (isFlying)
        {
            rig.useGravity = false;
        }
    }

    void Update()
    {
        GroundCheck();

        switch (curState)
        {
            case EnemyState.stopped:
                StoppedTransitionCheck();
                //for idle or spider
                break;
            case EnemyState.roaming:
                RoamingTransitionCheck();
                FaceTarget();
                if (isFlying)
                    AirRoam();
                else
                    GroundRoam();
                    break;
            case EnemyState.chase:
                ChaseTransitionCheck();
                FaceTarget();
                break;
            case EnemyState.attacking:
                AttackTransitionCheck();
                FaceTarget();
                AttackState();
                break;
        }
    }

    private void FixedUpdate()
    {
        if (isFlying)
        {
            rig.linearVelocity = Vector3.zero;
        }
        else
            rig.linearVelocity = new Vector3(0, rig.linearVelocity.y, 0);

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
                }
                else
                    Move();
                break;
            case EnemyState.roaming:
                if (reachedRoamTarget == false)
                    Move();
                    break;
            case EnemyState.attacking:
                targetPoint = player.transform.position;
                break;
        }

    }

    protected void GroundRoam()
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
            roamPauseTime = 0;
            SetRoamTargetGround();
        }
        else if (reachedRoamTarget == true)
        {
            roamPauseTimer += Time.deltaTime;
        }
    }

    protected bool LedgeCheck()
    {
        // Offsets the ray position on the y and local x
        Vector3 rayPos = new Vector3(transform.position.x, transform.position.y + 0.1f, transform.position.z) + transform.right * .2f;
        if (Physics.Raycast(rayPos, Vector3.down, 0.2f, groundLayer))
        {
            return true;
        }
        return false;
    }

    protected void SetRoamTargetGround()
    {
        reachedRoamTarget = false;

        float xDir = startPos.x - transform.position.x;

        if (xDir != 0)
        {
            FlipDir();//Turn to face target
        }

        if (xDir >= 0)
        {
            targetPoint = startPos + (Vector3.right * roamDist);
        }
        else
            targetPoint = startPos + (-Vector3.right * roamDist);
    }

    protected void AirRoam()
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
    protected void SetRoamTargetAir()
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

    protected void GroundCheck()
    {
        Vector3 rayPos = new Vector3(transform.position.x, transform.position.y + 0.1f, transform.position.z);
        if (Physics.Raycast(rayPos, Vector3.down, 0.2f, groundLayer))
        {
            isGrounded = true;
        }
        else
            isGrounded = false;
    }

    protected void Move()
    {
        Vector3 dir = targetPoint - transform.position;

        if (isFlying)
        {
            dir = dir.normalized * moveSpeed;
        }
        else
        {
            dir.y = 0;
            dir = dir.normalized * moveSpeed;
            dir.y = rig.linearVelocity.y;
        }   

            rig.linearVelocity = dir;
    }

    protected void AttackState()
    {
        attackTimer += Time.deltaTime;
        if (attackTimer >= attackRate)
        {
            animator.SetTrigger("Attack0");
        }
    }

    // Will be called by the attack animation
    public void Attack0()
    {
        //Toggle hitbox
        hitBoxes[0].SetActive(!hitBoxes[0].activeSelf);
    }

    public void RangedAttack0()
    {
        //Spawns a projectile
        Instantiate(projectiles[0], projectileSpawn.transform.position, projectileSpawn.transform.rotation);
    }

    protected void StoppedTransitionCheck()
    {

    }

    protected void RoamingTransitionCheck()
    {
        if (CanSeePlayer())
        {
            curState = EnemyState.chase;
        }
    }

    protected float DistFromTarget()
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

    protected void ChaseTransitionCheck()
    {
        if (DistFromTarget() <= chaseStopDist)
        {
            if (isFlying)
            {
                rig.linearVelocity = Vector3.zero;
            }
            
            curState = EnemyState.attacking;
        }
    }

    protected void AttackTransitionCheck()
    {
        if (DistFromTarget() > chaseStopDist + 0.1f)//0.1 is a slight offset to prevent constant state changes
        {
            curState = EnemyState.chase;
        }
    }

    protected void FaceTarget()
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

    protected void FlipDir()
    {
        transform.Rotate(0, 180, 0);
    }

    protected bool CanSeePlayer()
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

    void DropAbility()
    {
        Instantiate(abilityDrop, transform.position, Quaternion.identity);
    }

}
