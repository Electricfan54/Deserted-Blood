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
    float origTimeScale;

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
            StartCoroutine(HitStop());
        }
       
        if (damagetype==DamageType.deletable)
        {
            Destroy(gameObject);
        }


    }

    IEnumerator HitStop()
    {
        origTimeScale = Time.timeScale;
        Time.timeScale = 0;
        yield return new WaitForSecondsRealtime(stopTime);
        Time.timeScale = origTimeScale;
    }
}
