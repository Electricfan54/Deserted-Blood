using UnityEngine;

public class obstaclePit : MonoBehaviour
{

    [SerializeField] int damageAmount;

    private void OnTriggerEnter(Collider other)
    {

        Idamage dmg = other.GetComponent<Idamage>();

        if (other.tag == "Player" && dmg != null)
        {
            dmg.TakeDamage(damageAmount);
            gameManager.instance.player.transform.position = gameManager.instance.playerCheckpoint.position;
        }

    }

}
