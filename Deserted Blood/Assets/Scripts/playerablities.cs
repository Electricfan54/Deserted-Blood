using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class playerablities : MonoBehaviour
{

    int damage;
    [SerializeField] LayerMask ignore;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        selectability();
        if (gameManager.instance.playerScript.abilities.Count == 0)
        {

        }
        else if (Input.GetButtonDown("Fire1") && gameManager.instance.playerScript.abilities[gameManager.instance.playerScript.listpos].type == Ability.AbilityType.tp && gameManager.instance.playerScript.abilities[gameManager.instance.playerScript.listpos].currentcharge > 0 && gameManager.instance.playerScript.isGrounded == false)
        {
            Debug.Log("Pew Pew");
            if (gameManager.instance.playerScript.isGrounded == false)
            {
                gameManager.instance.playerScript.abilities[gameManager.instance.playerScript.listpos].currentcharge -= 1;
                RaycastHit hit;
                if (Physics.Raycast(transform.position, transform.forward, out hit, 5))
                {
                    gameManager.instance.player.transform.position =  hit.point;
                    gameManager.instance.UpdateCharges();
                }
            }



        }
        else if (Input.GetButtonDown("Fire1") && gameManager.instance.playerScript.abilities[gameManager.instance.playerScript.listpos].type == Ability.AbilityType.lightning && gameManager.instance.playerScript.abilities[gameManager.instance.playerScript.listpos].currentcharge > 0)
        {

            damage = gameManager.instance.playerScript.abilities[gameManager.instance.playerScript.listpos].damage;
            StartCoroutine(chargetime());
            Debug.DrawRay(gameObject.transform.position, gameObject.transform.forward * 6, Color.red);
            gameManager.instance.playerScript.abilities[gameManager.instance.playerScript.listpos].currentcharge -= 1;
            gameManager.instance.UpdateCharges();
        }
        else if (Input.GetButtonDown("Fire1") && gameManager.instance.playerScript.abilities[gameManager.instance.playerScript.listpos].type == Ability.AbilityType.fire && gameManager.instance.playerScript.abilities[gameManager.instance.playerScript.listpos].currentcharge > 0)
        {
            Instantiate(gameManager.instance.playerScript.abilities[gameManager.instance.playerScript.listpos].abilityPrefab, gameManager.instance.player.transform.position + Vector3.up * 1 + gameManager.instance.player.transform.forward * 1, gameManager.instance.player.transform.rotation);
            gameManager.instance.playerScript.abilities[gameManager.instance.playerScript.listpos].currentcharge -= 1;
            gameManager.instance.UpdateCharges();
        }
        else if (Input.GetButtonDown("Fire1") && gameManager.instance.playerScript.abilities[gameManager.instance.playerScript.listpos].type == Ability.AbilityType.shockwave && gameManager.instance.playerScript.abilities[gameManager.instance.playerScript.listpos].currentcharge > 0)
        {
            StartCoroutine(Slamtime());
            gameManager.instance.playerScript.abilities[gameManager.instance.playerScript.listpos].currentcharge -= 1;
            gameManager.instance.UpdateCharges();
        }
        else if (Input.GetButtonDown("Fire1") && gameManager.instance.playerScript.abilities[gameManager.instance.playerScript.listpos].type == Ability.AbilityType.ice && gameManager.instance.playerScript.abilities[gameManager.instance.playerScript.listpos].currentcharge > 0)
        {
            Instantiate(gameManager.instance.playerScript.abilities[gameManager.instance.playerScript.listpos].abilityPrefab, gameManager.instance.player.transform.position + Vector3.up * 1 + gameManager.instance.player.transform.forward * 1, gameManager.instance.player.transform.rotation);
            Instantiate(gameManager.instance.playerScript.abilities[gameManager.instance.playerScript.listpos].abilityPrefab2, gameManager.instance.player.transform.position + Vector3.up * 1 + gameManager.instance.player.transform.forward * -1, gameManager.instance.player.transform.rotation);
            gameManager.instance.playerScript.abilities[gameManager.instance.playerScript.listpos].currentcharge -= 1;
            gameManager.instance.UpdateCharges();
        }

        else if (Input.GetButtonDown("Fire1") && gameManager.instance.playerScript.abilities[gameManager.instance.playerScript.listpos].type == Ability.AbilityType.sonicboom && gameManager.instance.playerScript.abilities[gameManager.instance.playerScript.listpos].currentcharge > 0)
        {
            Instantiate(gameManager.instance.playerScript.abilities[gameManager.instance.playerScript.listpos].abilityPrefab, gameManager.instance.player.transform.position + Vector3.up * 1, gameManager.instance.player.transform.rotation);
            gameManager.instance.playerScript.abilities[gameManager.instance.playerScript.listpos].currentcharge -= 1;
            gameManager.instance.UpdateCharges();
        }

        else if (Input.GetButtonDown("Fire1") && gameManager.instance.playerScript.abilities[gameManager.instance.playerScript.listpos].type == Ability.AbilityType.sheild && gameManager.instance.playerScript.abilities[gameManager.instance.playerScript.listpos].currentcharge > 0)
        {
            Instantiate(gameManager.instance.playerScript.abilities[gameManager.instance.playerScript.listpos].abilityPrefab, gameManager.instance.player.transform.position + Vector3.up * 1, gameManager.instance.player.transform.rotation);
            gameManager.instance.playerScript.abilities[gameManager.instance.playerScript.listpos].currentcharge -= 1;
            gameManager.instance.UpdateCharges();
        }

    }

    void selectability()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0 && gameManager.instance.playerScript.listpos < gameManager.instance.playerScript.abilities.Count - 1)
        {
            gameManager.instance.playerScript.listpos++;
            gameManager.instance.UpdateAbilityUI();
            changegun();
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0 && gameManager.instance.playerScript.listpos > 0)
        {
            gameManager.instance.playerScript.listpos--;
            gameManager.instance.UpdateAbilityUI();
            changegun();
        }

    }


    void changegun()
    {

        damage = gameManager.instance.playerScript.abilities[gameManager.instance.playerScript.listpos].damage;
    }
    IEnumerator chargetime()
    {
        yield return new WaitForSeconds(.5f);
        RaycastHit[] hit = Physics.RaycastAll(transform.position, transform.forward, 5);
        for (int i = 0; i < hit.Length; i++)
        {
            Debug.Log("Hit" + hit[i].collider.name);
            Idamage dam = hit[i].collider.GetComponent<Idamage>();
            if (dam != null)
            {
                dam.TakeDamage(damage);
            }
            else
            {
                Debug.Log("No Damageable component found on " + hit[i].collider.name);
            }
        }


    }

    IEnumerator Slamtime()
    {
        gameManager.instance.playerScript.transform.position = gameManager.instance.playerScript.transform.position + Vector3.up * 3;
        yield return new WaitForSeconds(.3f);
        Instantiate(gameManager.instance.playerScript.abilities[gameManager.instance.playerScript.listpos].abilityPrefab, gameManager.instance.player.transform.position + Vector3.up * 1, gameManager.instance.player.transform.rotation);
    }
}
