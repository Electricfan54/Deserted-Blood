using UnityEngine;

public class pickup : MonoBehaviour
{

    [SerializeField] Ability ability;
    bool canPickup;

    private void Update()
    {
        if (canPickup)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                IPickup pickup = gameManager.instance.player.GetComponent<IPickup>();
                if (pickup != null)
                {

                    ability.currentcharge = ability.maxCharge;
                    pickup.abilitystats(ability);
                    gameManager.instance.pickUpPrompt.SetActive(false);

                    Destroy(gameObject);

                }

            }

        }
    }

    private void OnTriggerEnter(Collider other)
    {

        canPickup = true;
        gameManager.instance.pickUpPrompt.SetActive(true);


    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player" && gameManager.instance.pickUpPrompt.activeSelf)
        {
            canPickup = false;
            gameManager.instance.pickUpPrompt.SetActive(false);
        }
    }

}
