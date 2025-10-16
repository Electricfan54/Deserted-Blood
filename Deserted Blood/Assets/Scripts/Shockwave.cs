using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Shockwave : MonoBehaviour
{
    [SerializeField] int destroytime;
    [SerializeField] int shockwavedamage;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
         StartCoroutine(destroyshockwave());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            return;
        }
        Idamage dmg = other.GetComponent<Idamage>();
        if (dmg != null)
        {
            dmg.TakeDamage(shockwavedamage);
 other.GetComponent<Rigidbody>().AddExplosionForce(10f, transform.position, 2f, 0f, ForceMode.Impulse);
        }
       

    }
    IEnumerator destroyshockwave()
    {
        yield return new WaitForSeconds(destroytime);
        Destroy(gameObject);
    }
}
