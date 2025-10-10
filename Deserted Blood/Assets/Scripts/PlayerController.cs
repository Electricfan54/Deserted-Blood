using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour, Idamage,IPickup
{
    [SerializeField] CharacterController CharController;
    [SerializeField] Animator PlayerAnimator;

    [SerializeField] int HP;
    [SerializeField] int MaxHP;
    [SerializeField] int BloodMeter;
    [SerializeField] int MaxBloodMeter;

    [SerializeField] int BasePlayerDamage;

    [SerializeField] int MaxJumps;
    [SerializeField] int JumpStrength;
    [SerializeField] int gravityStrength;
    [SerializeField] int SlideGravity;

    [SerializeField] int Speed;

    int JumpCount;
    Vector3 MoveDirection;
    Vector3 playerVel;

    public bool isGrounded;
    bool isWallSliding;
    bool WallInRange;

    bool isInvinc = false;

    public bool hasThirdAbility = false;


   public List<Ability> abilities = new List<Ability>();
   public int listpos;
   public playerablities PlayerAbilites;

    int origBloodMeter = 50;
    int origHP;

    int playerXPush = 3;

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
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(gameObject.transform.position + new Vector3(0, 1.5f, 0), gameObject.transform.up, Color.red);
        Debug.DrawRay(gameObject.transform.position + new Vector3(0,1.5f,0), gameObject.transform.forward * .7f, Color.red);
        if (CharController.isGrounded)
        {
            isGrounded = true;
            playerVel.y = -2;
            playerVel.x = 0;
            JumpCount = 0;
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

        //RaycastHit CeilingCheck;
        //if (Physics.Raycast(gameObject.transform.position + new Vector3(0, 1.5f, 0), gameObject.transform.up, out CeilingCheck, .8f))
        //{
        //    playerVel.y = -2;
        //}

        Movement();
    }

    // Movement stuff
    void Movement()
    {
        float Horizantol = Input.GetAxis("Horizontal");

        MoveDirection = new Vector3(Horizantol, 0, 0);

        if(Horizantol > 0)
        {
            transform.rotation = Quaternion.Euler(0,90,0);
        }
        else if (Horizantol < 0)
        {
            transform.rotation = Quaternion.Euler(0,-90,0);
        }

        CharController.Move(MoveDirection * Speed * Time.deltaTime);

        Jump();

        CharController.Move(playerVel * Time.deltaTime);
    }

    void Jump()
    {
        var InputDown = Input.GetButtonDown("Jump");
        var InputUp = Input.GetButtonUp("Jump");
        if(InputDown && JumpCount <  MaxJumps)
        {
            playerVel.y = JumpStrength;
            JumpCount++;
        }

        if(InputUp && playerVel.y > 0 && WallInRange == false)
        {
            playerVel.y = 0;
        }

        

        WallJump();
    }

    void WallJump()
    {
        RaycastHit WallCheck;
        if(Physics.Raycast(gameObject.transform.position, gameObject.transform.forward, out WallCheck, .7f))
        {
            WallInRange = true;
        }
        else
        {
            WallInRange = false;
        }

        if(WallInRange && Input.GetButtonDown("Jump") && !isGrounded)
        {
            playerVel = new Vector3(-transform.forward.x * 5, JumpStrength * 1.5f, 0);
            transform.rotation = Quaternion.Euler(0, -transform.forward.x > 0 ? 90 : -90, 0);
        }

        if(playerVel.x != 0)
        {
            if(playerVel.x > 0)
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
        if(isInvinc == false)
        {
            HP -= DamageAmount;
            StartCoroutine(IFrames());
        }
    }

    public void abilitystats(Ability ability)
    {
        abilities.Add(ability);
        listpos = abilities.Count - 1;
    }

    IEnumerator IFrames()
    {
        isInvinc = true;
        yield return new WaitForSeconds(0.4f);
        isInvinc = false;
    }

    public void AddBloodMeter(int amount)
    {
        BloodMeter += amount;
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



}
