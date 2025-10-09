using UnityEngine;

public class PlayerController : MonoBehaviour, Idamage
{
    [SerializeField] CharacterController CharController;
    [SerializeField] Animator PlayerAnimator;

    [SerializeField] int HP;

    [SerializeField] int MaxJumps;
    [SerializeField] int JumpStrength;
    [SerializeField] int gravityStrength;

    [SerializeField] int Speed;

    int JumpCount;
    Vector3 MoveDirection;
    Vector3 playerVel;

    public bool isGrounded;
    bool isWallSliding;

    public bool hasThirdAbility = false;


    int origHP;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        origHP = HP;
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
        HP -= DamageAmount;
    }


}
