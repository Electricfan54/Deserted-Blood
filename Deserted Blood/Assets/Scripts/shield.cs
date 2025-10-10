using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class shield : MonoBehaviour
{
    int origdam;
    int meleeorigdam;
   [SerializeField] int destroy;
    
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
        if(other.CompareTag("Enemy"))
        { StartCoroutine(destroysheild(other));
         
           
         
        }
    }
   

    IEnumerator destroysheild(Collider collider)
    {  meleeorigdam= collider.GetComponent<EnemyAI>().meleeDamage;
              origdam= collider.GetComponent<EnemyAI>().projDamage;
        collider.GetComponent<EnemyAI>().meleeDamage = 0;
        collider.GetComponent<EnemyAI>().projDamage = 0;
        yield return new WaitForSeconds(destroy);
        collider.GetComponent<EnemyAI>().meleeDamage = meleeorigdam;
        collider.GetComponent<EnemyAI>().projDamage = origdam;
        Destroy(gameObject);
    }
  
}
