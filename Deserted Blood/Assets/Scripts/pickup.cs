using UnityEngine;

public class pickup : MonoBehaviour
{

    [SerializeField] Ability ability;

    private void OnTriggerStay(Collider other)
    {
        IPickup pickup = other.GetComponent<IPickup>();
        if (pickup!=null)
        {

           gameManager.instance.pickUpPrompt.SetActive(true);

            if (Input.GetKeyDown(KeyCode.E))
            {
                ability.currentcharge = ability.maxCharge;
                pickup.abilitystats(ability);
                gameManager.instance.pickUpPrompt.SetActive(false);

                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player" && gameManager.instance.pickUpPrompt.activeSelf)
        {
            gameManager.instance.pickUpPrompt.SetActive(false);
        }
    }

}   
