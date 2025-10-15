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
        {
            StartCoroutine(destroysheild());
        }
    }
    // Update is called once per frame
    void Update()
    {

    }



    IEnumerator destroysheild()
    {

        yield return new WaitForSeconds(destroy);
        gameManager.instance.playerScript.isInvinc = false;
        Destroy(gameObject);
    }


}
