using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] float destroytime;
    [SerializeField] Rigidbody rb;
    public Damage dmg;
    private void Awake()
    {
        

    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Destroy(gameObject, destroytime);
        rb.linearVelocity = transform.forward * speed;
    }
    private void OnCollisionEnter(Collision collision)
    {
       if(collision.gameObject.CompareTag("Enemy")||collision.gameObject.CompareTag("Player"))
        {

        }
        else
            Destroy(gameObject);
        
    }
 
    // Update is called once per frame
    void Update()
    {
        
    }
}
