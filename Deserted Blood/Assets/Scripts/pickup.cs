using UnityEngine;

public class pickup : MonoBehaviour
{

    [SerializeField] Ability ability;

    private void OnTriggerStay(Collider other)
    {
        IPickup pickup = other.GetComponent<IPickup>();
        if (pickup!=null)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                ability.currentcharge = ability.maxCharge;
                pickup.abilitystats(ability);

                Destroy(gameObject);
            }
        }
    }
}   
