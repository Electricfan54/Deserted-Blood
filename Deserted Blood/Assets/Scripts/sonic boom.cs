using System;
using System.Collections;
using UnityEngine;

public class sonicboom : MonoBehaviour
{
  
   

[SerializeField] int destroytime;
  
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(destroysonicboom());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            other.GetComponent<EnemyAI>().ApplyStunEffect(5);
        }
        if (other.gameObject.CompareTag("Player"))
        {
            return;
        }
    }
    IEnumerator destroysonicboom()
    {
        yield return new WaitForSeconds(destroytime);
        Destroy(gameObject);
    }
}
