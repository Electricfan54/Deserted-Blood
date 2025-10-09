using UnityEngine;

public class playerablities : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1") && gameManager.instance.playerScript.abilities[gameManager.instance.playerScript.listpos].type==Ability.AbilityType.tp&& gameManager.instance.playerScript.abilities[gameManager.instance.playerScript.listpos].currentcharge>0&&gameManager.instance.playerScript.isGrounded==false)
        {
            Debug.Log("Pew Pew");
            if(gameManager.instance.playerScript.isGrounded == false)
                {
gameManager.instance.playerScript.abilities[gameManager.instance.playerScript.listpos].currentcharge -= 1;
            gameManager.instance.player.transform.position=gameManager.instance.player.transform.position+gameManager.instance.player.transform.forward*10;
            }
            
        }
    }
}
