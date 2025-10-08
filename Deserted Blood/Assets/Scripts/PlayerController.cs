using UnityEngine;

public class PlayerController : MonoBehaviour
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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
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
    }
}
