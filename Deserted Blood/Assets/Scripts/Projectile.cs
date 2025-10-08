using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] float destroytime;
    [SerializeField] Rigidbody rb;
    public Damage dmg;
    private void Awake()
    {
        dmg=GetComponent<Damage>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Destroy(gameObject, destroytime);
        rb.linearVelocity = transform.forward * speed;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
