using UnityEngine;

public class Projectile : MonoBehaviour
{
   public float speed;
    public float destroytime;
    [SerializeField] Rigidbody rb;
    public Damage dmg;
    private void Awake()
    {
        dmg = GetComponent<Damage>();

    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Destroy(gameObject, destroytime);
        rb.linearVelocity = transform.forward * speed;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (gameObject.CompareTag(other.tag))
            return;
        else if (other.gameObject.CompareTag("Enemy") || other.gameObject.CompareTag("Player"))
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
