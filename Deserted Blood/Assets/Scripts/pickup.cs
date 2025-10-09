using UnityEngine;

public class pickup : MonoBehaviour
{
    [SerializeField] Ability ability;

    private void OnTriggerEnter(Collider other)
    {
        IPickup pickup = other.GetComponent<IPickup>();
        if (pickup!=null)
        {
            ability.currentcharge = ability.maxCharge;
            pickup.abilitystats(ability);

            Destroy(gameObject);
        }
    }
}   
