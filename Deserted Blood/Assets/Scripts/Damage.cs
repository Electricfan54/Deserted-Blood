using UnityEngine;

public class Damage : MonoBehaviour
{
    enum DamageType
    {
        deletable, nondeletable,fireball, iceball
    }
    public int damageammount;
    [SerializeField] DamageType damagetype;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
       Idamage dmg = other.GetComponent<Idamage>();
        if(dmg != null)
        {
            dmg.TakeDamage(damageammount);
        }
        if(other.gameObject.CompareTag("Enemy")&&damagetype==DamageType.fireball)
        {
            other.GetComponent<EnemyAI>().ApplyBurnEffect(10,1,5);
        }
        
        if (other.gameObject.CompareTag("Enemy") && damagetype == DamageType.iceball)
        {
            other.GetComponent<EnemyAI>().ApplyFreezeEffect(10);
        }
       
        if (damagetype==DamageType.deletable)
        {
            Destroy(gameObject);
        }


    }
}
