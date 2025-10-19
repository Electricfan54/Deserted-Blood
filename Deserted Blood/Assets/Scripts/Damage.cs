using System.Collections;
using UnityEngine;

public class Damage : MonoBehaviour
{
    enum DamageType
    {
        deletable, nondeletable,
    }
    public int damageammount;
    [SerializeField] DamageType damagetype;
    public bool hitStop;
    public float stopTime;

    public bool applyScreenShake = false;
    public float shakeDuration = .5f;
    public float shakeStrength = .5f;


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
        if (hitStop)
        {
            gameManager.instance.HitStop(stopTime);
        }
       
        if (damagetype==DamageType.deletable)
        {
            Destroy(gameObject);
        }

        if (applyScreenShake)
            ScreenShake.instance.ShakeScreen(shakeDuration, shakeStrength);

    }
}
