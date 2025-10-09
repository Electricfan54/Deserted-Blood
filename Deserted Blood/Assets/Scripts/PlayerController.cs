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

    [SerializeField] int Speed;

    int JumpCount;
    Vector3 MoveDirection;
    Vector3 playerVel;

    public bool isGrounded;
    bool isWallSliding;

    bool isInvinc = false;

    public bool hasThirdAbility = false;


   public List<Ability> abilities = new List<Ability>();
   public int listpos;
   public playerablities PlayerAbilites;

    int origBloodMeter = 50;
    int origHP;


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

        if (CharController.isGrounded)
        {
            isGrounded = true;
            playerVel.y = -2;
            JumpCount = 0;
        }
        else
        {
            isGrounded = false;
            playerVel.y -= gravityStrength * Time.deltaTime;
        }
        RaycastHit CeilingCheck;

        if (Physics.Raycast(gameObject.transform.position + new Vector3(0, 1.5f, 0), gameObject.transform.up, out CeilingCheck, 2.5f))
        {
            playerVel.y = 0;
        }

        Movement();
    }

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

        if(InputUp && playerVel.y > 0)
        {
            playerVel.y = 0;
        }
    }

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
