using System;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class Ability : MonoBehaviour
{
    enum AbilityType
    {
   fire,ice,lightning,shockwave,tp,sheild,sonicboom
    }
    [SerializeField] LayerMask ignore;
    [SerializeField] AbilityType abilitytype;
    [SerializeField] int abilityuses;
    public Damage dmg;

    private void Awake()
    {
        dmg = GetComponent<Damage>();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButton(0)&& abilitytype==AbilityType.fire&&abilityuses>0)
        {
            mousemovements();
            abilityuses--;
        }
        else if(Input.GetButtonUp("Fire1")&& abilitytype==AbilityType.lightning && abilityuses > 0)
        {StartCoroutine(chargetime());
            Debug.DrawRay(gameObject.transform.position, gameObject.transform.forward * 6, Color.red);
            abilityuses--;

        }
    }

    private void StartCoroutine(IEnumerable enumerable)
    {
        throw new NotImplementedException();
    }

    void mousemovements()
    {
     float rotationSpeed = 5f;
    //    Vector3 mousemv=Input.mousePosition;
    //    mousemv.z=Camera.main.WorldToScreenPoint(mousemv).z;
    //    Vector3 worldpos=Camera.main.ScreenToWorldPoint(mousemv);

    //Vector3 dir=worldpos-transform.position;

    //    float angle=Mathf.Atan2(dir.y,dir.x)*Mathf.Rad2Deg;
    //    transform.rotation=Quaternion.AngleAxis(angle,Vector3.forward);

    float mouseX = Input.GetAxis("Mouse Y");
        transform.Rotate(Vector3.right, mouseX * rotationSpeed);
    }

    IEnumerator chargetime()
    {
        yield return new WaitForSeconds(.5f);
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 10f, ~ignore))
        {
            Debug.Log("Hit" + hit.collider.name);
            Idamage dmg = hit.collider.GetComponent<Idamage>();
            if (dmg != null)
            {
                dmg.TakeDamage(10);
            }
        }
    }
}
