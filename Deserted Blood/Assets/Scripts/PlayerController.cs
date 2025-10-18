using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour, Idamage, IPickup, IEffect
{
    [SerializeField] CharacterController CharController;
    [SerializeField] Animator PlayerAnimator;
    [SerializeField] Renderer meshRenderer;
    [SerializeField] int[] BasicAttackAnimations;
    [SerializeField] LayerMask Groundlayer;

    public int HP;
    public int MaxHP;
    [SerializeField] int BloodMeter;
    [SerializeField] int MaxBloodMeter;

    [Tooltip("Used for regular attacks")][SerializeField] GameObject LightAttackHitbox;
    [SerializeField] GameObject[] BaseAttacks;
    [SerializeField] int BasePlayerDamage;

    [SerializeField] int MaxJumps;
    [SerializeField] int JumpStrength;
    [SerializeField] int gravityStrength;
    [SerializeField] int SlideGravity;

    [SerializeField] int Speed;
    [SerializeField] int animTranSpeed;

    int JumpCount;
    Vector3 MoveDirection;
    Vector3 playerVel;

    int CurrentMoveAnimationIndex;


    public bool isGrounded;
    bool hasWallJumped = false;
    bool WallInRange;

    public  bool isInvinc = false;
    bool isAttacking = false;
    bool canAttack = true;

    public bool GateKeeperAbilityCheck = false;
    public bool RedHornAbilityCheck = false;
    public bool hasThirdAbility = false;


    public List<Ability> abilities = new List<Ability>();
    public int listpos;
    public playerablities PlayerAbilites;

    int origBloodMeter = 50;
    int origHP;
    float bloodTimer;
    float M1CDtimer;
    float MapPunchTimer;

    int playerXPush = 3;

    //Status effect variables
    protected float burnDuration;
    protected int burnTickDamage;
    protected float burnTimer;
    protected float burnTickRate;

    protected float freezeDuration;
    protected float origAnimSpeed;
    protected Color beforeFreezeColor;

    protected float StunDuration;
    protected Color beforeStunColor;
    List<int> stuncharges = new List<int>();

    protected bool canUpdate = true; //for stopping player input update
    protected bool canMove = true; //for stopping player movement

    GameObject CurrentAttack;

    private void Awake()
    {
        PlayerAbilites = GetComponent<playerablities>();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        origHP = HP;
        MaxHP = origHP;
        origBloodMeter = MaxBloodMeter;
        gameManager.instance.UpdateHPBar(MaxHP, HP);
        gameManager.instance.UpdateBloodMeter(MaxBloodMeter, BloodMeter);

    }

    // Update is called once per frame
    void Update()
    {

        if (burnDuration > 0)
            BurnEffect();
        if (freezeDuration > 0)
            FreezeEffect();
        if (StunDuration > 0)
            StunEffect();
        //Debug.DrawRay(gameObject.transform.position + new Vector3(0, 1.5f, 0), gameObject.transform.up, Color.red);
        //Debug.DrawRay(gameObject.transform.position + new Vector3(0,1.5f,0), gameObject.transform.forward * .7f, Color.red);
        if (CharController.isGrounded)
        {
            isGrounded = true;
            playerVel.y = -2;
            JumpCount = 0;
            if (hasWallJumped == true)
            {
                playerVel.x = 0;
                hasWallJumped = false;
            }
        }
        else
        {
            isGrounded = false;
            if (WallInRange && playerVel.y <= 0)
            {
                playerVel.y -= SlideGravity * Time.deltaTime;
            }
            else
            {
                playerVel.y -= gravityStrength * Time.deltaTime;
            }
        }

        RaycastHit CeilingCheck;
        if (Physics.Raycast(gameObject.transform.position + new Vector3(0, 1.5f, 0), gameObject.transform.up, out CeilingCheck, .8f, Groundlayer))
        {
            playerVel.y = -2;
        }

        Movement();
    
    }

    private void LateUpdate()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, 0);
    }
    // Movement stuff
    void Movement()
    {
        float Horizantol = 0;
        if (canMove)
            Horizantol = Input.GetAxis("Horizontal");

        MoveDirection = new Vector3(Horizantol, 0, 0);

        if (Horizantol > 0)
        {
            transform.rotation = Quaternion.Euler(0, 90, 0);
        }
        else if (Horizantol < 0)
        {
            transform.rotation = Quaternion.Euler(0, -90, 0);
        }


        CharController.Move(MoveDirection * Speed * Time.deltaTime);

        if (canMove)
            Jump();

        BaseAbilityInputCheck();
        RunningAnimaiton();
        CharController.Move(playerVel * Time.deltaTime);
    }

    void RunningAnimaiton()
    {
        if (!isGrounded)
        {
            PlayerAnimator.SetFloat("RunningSpeed", 0);
            return;
        }
        float currentAnimSpeed = PlayerAnimator.GetFloat("RunningSpeed");
        Vector3 flat = new Vector3(MoveDirection.x, 0, 0);
        float speed = flat.normalized.magnitude;

        PlayerAnimator.SetFloat("RunningSpeed", Mathf.Lerp(currentAnimSpeed, speed, Time.deltaTime * animTranSpeed));
        if(PlayerAnimator.GetFloat("RunningSpeed") < 0)
        {
            PlayerAnimator.SetFloat("RunningSpeed", 0);
        }
    }

    void Jump()
    {
        var InputDown = Input.GetButtonDown("Jump");
        var InputUp = Input.GetButtonUp("Jump");
        if (InputDown && JumpCount < MaxJumps)
        {
            playerVel.y = JumpStrength;
            JumpCount++;
        }

        if (InputUp && playerVel.y > 0 && WallInRange == false)
        {
            playerVel.y = 0;
        }



        WallJump();
    }

    void WallJump()
    {
        RaycastHit WallCheck;
        if (Physics.Raycast(gameObject.transform.position, gameObject.transform.forward, out WallCheck, .7f))
        {
            WallInRange = true;
        }
        else
        {
            WallInRange = false;
        }

        if (WallInRange && Input.GetButtonDown("Jump") && !isGrounded)
        {
            hasWallJumped = true;
            JumpCount = 0;
            playerVel = new Vector3(-transform.forward.x * 5, JumpStrength * 1.5f, 0);
            transform.rotation = Quaternion.Euler(0, -transform.forward.x > 0 ? 90 : -90, 0);
        }

        if (playerVel.x != 0)
        {
            if (playerVel.x > 0)
            {
                playerVel.x -= playerXPush * Time.deltaTime;
            }
            else
            {
                playerVel.x += playerXPush * Time.deltaTime;
            }
        }

    }


    // Ability or enemy related stuff
    public void TakeDamage(int DamageAmount)
    {
        if (isInvinc == false)
        {
            HP -= DamageAmount;
            gameManager.instance.UpdateHPBar(MaxHP, HP);
            if (HP <= 0)
            {
                // call game lose
                gameManager.instance.GameOver();
            }
            StartCoroutine(IFrames());
        }
    }

    public void AddBloodAmount(int amount)
    {
        BloodMeter += amount;
        if (BloodMeter > MaxBloodMeter)
        {
            BloodMeter = MaxBloodMeter;
        }

        gameManager.instance.UpdateBloodMeter(MaxBloodMeter, BloodMeter);
    }

    public void abilitystats(Ability ability)
    {
        if (abilities.Count == 0)
        {
            abilities.Add(ability);
            listpos = abilities.Count - 1;
            gameManager.instance.UpdateAbilityUI();
            gameManager.instance.AssignAbility();
        }
        else if (abilities.Count == 3)
        {
            for (int i = 0; i < abilities.Count; i++)
            {
                if (abilities[i].type == ability.type)
                {
                    int temp=listpos;
                    abilities[i].currentcharge = abilities[i].maxCharge;
                    listpos = i;

                    gameManager.instance.UpdateCharges();
                    gameManager.instance.AssignAbility();
                    listpos = temp;
                    return;
                }
            }
            abilities[listpos] = ability;
            gameManager.instance.UpdateCharges();
            gameManager.instance.AssignAbility();
            return;
        }
        else
        {
            for (int i = 0; i < abilities.Count; i++)
            {
                if (abilities[i].type == ability.type)
                {
                    abilities[i].currentcharge = abilities[i].maxCharge;
                    gameManager.instance.UpdateCharges();
                    gameManager.instance.AssignAbility();
                    return;
                }



            }
            abilities.Add(ability);
            listpos = abilities.Count - 1;
            gameManager.instance.UpdateAbilityUI();
            gameManager.instance.AssignAbility();
        }


    }

    IEnumerator IFrames()
    {
        isInvinc = true;
        yield return new WaitForSeconds(0.4f);
        isInvinc = false;
    }

    public void AddBloodMeterMilestone(int AmounttoAdd)
    {
        MaxBloodMeter += AmounttoAdd;
    }

    public void AddPlayerDamageMilestone(int Amount)
    {
        BasePlayerDamage += Amount;
    }

    public void AddHPMilestone(int amount)
    {
        MaxHP += amount;
    }

    public void ApplyBurnEffect(float duration, int tickDamage, float tickRate)
    {
        burnDuration = duration;
        burnTickDamage = tickDamage;
        burnTickRate = tickRate;
        burnTimer = 0;
    }

    public void ApplyFreezeEffect(float duration)
    {
        if (freezeDuration > 0)
            return;
        freezeDuration = duration;
        canMove = false;
        canUpdate = false;
        canAttack = false;

        StopAttack();

        if (PlayerAnimator.speed != 0)
        {
            origAnimSpeed = PlayerAnimator.speed;
            PlayerAnimator.speed = 0;// Pause animation
        }

        if (meshRenderer.material.color == Color.yellow)
        {
            beforeFreezeColor = beforeStunColor;
            beforeStunColor = Color.blue;
        }
        else
            beforeFreezeColor = meshRenderer.material.color;
        meshRenderer.material.color = Color.blue;
    }

    void BurnEffect()
    {
        burnDuration -= Time.deltaTime;
        burnTimer += Time.deltaTime;
        if (burnTimer >= burnTickRate)
        {
            burnTimer = 0;
            TakeDamage(burnTickDamage);
        }
    }

    void FreezeEffect()
    {
        freezeDuration -= Time.deltaTime;
        if (freezeDuration <= 0)
        {
            freezeDuration = 0;
            canMove = true;
            canUpdate = true;
            canAttack = true;
            PlayerAnimator.speed = origAnimSpeed;
            meshRenderer.material.color = beforeFreezeColor;
        }
    }

    void StunEffect()
    {
        StunDuration -= Time.deltaTime;
        if (StunDuration <= 0)
        {
            StunDuration = 0;
            for (int i = 0; i < abilities.Count; i++)
            {

                abilities[i].currentcharge = stuncharges[i];
            }
            stuncharges.Clear();
            meshRenderer.material.color = beforeStunColor;
        }
    }
    public void ApplyStunEffect(float duration)
    {
        StunDuration = duration;
        for (int i = 0; i < abilities.Count; i++)
        {
            stuncharges.Add(abilities[i].currentcharge);
            abilities[i].currentcharge = 0;
        }

        if (meshRenderer.material.color != Color.yellow)
        {

            beforeStunColor = meshRenderer.material.color;
            meshRenderer.material.color = Color.yellow;
        }
    }

    // attack stuff
    void BaseAbilityInputCheck()
    {
        M1CDtimer += Time.deltaTime;
        MapPunchTimer += Time.deltaTime;
        if (isAttacking == false && Input.GetKeyDown(KeyCode.B) && MapPunchTimer > 2.3f)
        {
            StartCoroutine(MappaPunch());
        }

        if (isAttacking == false && Input.GetKeyDown(KeyCode.N) && GateKeeperAbilityCheck == true)
        {
            StartCoroutine(FallingPunch());
        }
        if(isAttacking == false && Input.GetKeyDown(KeyCode.Z) && RedHornAbilityCheck == true)
        {

        }
        if (isAttacking == false && Input.GetKeyDown(KeyCode.J) && canAttack == true)
        {
            // play animation using the animation index after making the stuff   for it
            // left, right, hook, right
            if (CurrentMoveAnimationIndex > BasicAttackAnimations.Length - 1 || M1CDtimer > 2)
                CurrentMoveAnimationIndex = 0;

            if(M1CDtimer > .7f)
            {
                StartCoroutine(BasicAttack());
            }
        }

        RegenHealth();
        
    }

    void RegenHealth()
    {
        bloodTimer += Time.deltaTime;
        if (isAttacking == false && Input.GetKey(KeyCode.H) && bloodTimer > 0.2f)
        {
            if (BloodMeter > 0&HP!=MaxHP)
            {
                bloodTimer = 0;
                BloodMeter--;
                HP++;
                gameManager.instance.UpdateHPBar(MaxHP, HP);
                gameManager.instance.UpdateBloodMeter(MaxBloodMeter, BloodMeter);
            }
        }
    }

    IEnumerator MappaPunch()
    {
        MapPunchTimer = 0;
        canMove = false;
        isAttacking = true;
        isInvinc = true;
        PlayerAnimator.SetBool("MappaPunchActive", true);
        yield return new WaitForSeconds(0.2f);
        playerVel.x = transform.forward.x * 20;
        BaseAttacks[0].GetComponent<Damage>().damageammount = BasePlayerDamage * 2;
        BaseAttacks[0].SetActive(true);
        yield return new WaitForSeconds(.4f);
        BaseAttacks[0].SetActive(false);
        playerVel.x = 0;
        isAttacking = false;
        canMove = true;
        PlayerAnimator.SetBool("MappaPunchActive", false);
        isInvinc = false;
    }

    IEnumerator FallingPunch()
    {
        isAttacking = true;
        isInvinc = true;
        playerVel = new Vector3(transform.forward.x * 10, JumpStrength * 1.2f, 0);
        yield return new WaitForSeconds(.5f);
        BaseAttacks[1].GetComponent<Damage>().damageammount = BasePlayerDamage * 2;
        BaseAttacks[1].SetActive(true);
        playerVel = new Vector3(transform.forward.x * 10, -50, 0);
        yield return new WaitForSeconds(.2f);
        playerVel.x = 0;
        BaseAttacks[1].SetActive(false);
        isAttacking = false;
        isInvinc = false;
        // cool
    }

    IEnumerator BasicAttack()
    {
        if(isGrounded)
            canMove = false;

        M1CDtimer = 0;
        PlayerAnimator.SetBool("M1", true);
        PlayerAnimator.SetFloat("M1Count", CurrentMoveAnimationIndex);
        isAttacking = true;
        LightAttackHitbox.GetComponent<Damage>().damageammount = BasePlayerDamage;
        LightAttackHitbox.SetActive(true);
        yield return new WaitForSeconds(.35f);
        LightAttackHitbox.SetActive(false);
        isAttacking = false;
        CurrentMoveAnimationIndex += 1;
        PlayerAnimator.SetBool("M1", false);
        canMove = true; 
    }

    IEnumerator RedHornAbility()
    {
        isAttacking = true;
        isInvinc = true;
        yield return new WaitForSeconds(0.1f);
        
    }

    void StopAttack()
    {
        if(LightAttackHitbox.activeSelf)
        {
            LightAttackHitbox.SetActive(false);
        }

        if (BaseAttacks[0].activeSelf) // mappa Punch
        {
            BaseAttacks[0].SetActive(false);
        }

        if(BaseAttacks[1].activeSelf) //  falling punch
        {
            BaseAttacks[1].SetActive(false);
        }
    }


}
