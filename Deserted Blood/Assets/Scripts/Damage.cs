using UnityEngine;

public class Damage : MonoBehaviour
{
    enum DamageType
    {
        deletable, nondeletable,fireball
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
        if(other.gameObject.CompareTag("Player") && damagetype == DamageType.fireball)
        {
            gameManager.instance.playerScript.ApplyBurnEffect(10, 1, 5);
        }
            if (damagetype==DamageType.deletable)
        {
            Destroy(gameObject);
        }


    }
}
