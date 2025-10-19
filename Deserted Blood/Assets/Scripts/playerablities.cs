using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class playerablities : MonoBehaviour
{

    int damage;
    [SerializeField] LayerMask lightningtarget;
    [SerializeField] ParticleSystem lightningeffect;
    [SerializeField] AudioSource aud;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        selectability();
      if (Input.GetKeyDown(KeyCode.L))
        {
            useAbility();
        }

    }

    void selectability()
    {
        if (Input.GetKeyDown(KeyCode.Q) && gameManager.instance.playerScript.listpos < gameManager.instance.playerScript.abilities.Count - 1)
        {
            gameManager.instance.playerScript.listpos++;
            gameManager.instance.UpdateAbilityUI();
            changegun();
        }
        else if (Input.GetKeyDown(KeyCode.W) && gameManager.instance.playerScript.listpos > 0)
        {
            gameManager.instance.playerScript.listpos--;
            gameManager.instance.UpdateAbilityUI();
            changegun();
        }

    }

    void useAbility()
    {
  if (gameManager.instance.playerScript.abilities.Count == 0)
        {

        }

        //Teleport ability
        else if (gameManager.instance.playerScript.abilities[gameManager.instance.playerScript.listpos].type == Ability.AbilityType.tp && gameManager.instance.playerScript.abilities[gameManager.instance.playerScript.listpos].currentcharge > 0 && gameManager.instance.playerScript.isGrounded == false)
        {
           
            Debug.Log("Pew Pew");
            if (gameManager.instance.playerScript.isGrounded == false)
            {
                gameManager.instance.playerScript.abilities[gameManager.instance.playerScript.listpos].currentcharge -= 1;
                RaycastHit hit;
                if (Physics.Raycast(transform.position, transform.forward, out hit, 5))
                {
                    gameManager.instance.player.transform.position =  hit.point-gameManager.instance.player.transform.forward;
                    gameManager.instance.UpdateCharges();
                }
                else
                {
                    gameManager.instance.player.transform.position = gameManager.instance.player.transform.position + gameManager.instance.player.transform.forward * 5;
                    gameManager.instance.UpdateCharges();
                }
            }



        }

        //lightning ability
        else if ( gameManager.instance.playerScript.abilities[gameManager.instance.playerScript.listpos].type == Ability.AbilityType.lightning && gameManager.instance.playerScript.abilities[gameManager.instance.playerScript.listpos].currentcharge > 0)
        {

            damage = gameManager.instance.playerScript.abilities[gameManager.instance.playerScript.listpos].damage;
            Instantiate(gameManager.instance.playerScript.abilities[gameManager.instance.playerScript.listpos].abilityPrefab, gameManager.instance.player.transform.position + Vector3.up * 1, gameManager.instance.player.transform.rotation);
            StartCoroutine(chargetime());
            aud.pitch = Random.Range(0.8f, 1.2f);
            aud.PlayOneShot(gameManager.instance.playerScript.abilities[gameManager.instance.playerScript.listpos].abilitySound,gameManager.instance.sfxVolume);
            Debug.DrawRay(gameObject.transform.position, gameObject.transform.forward * 6, Color.red);
            gameManager.instance.playerScript.abilities[gameManager.instance.playerScript.listpos].currentcharge -= 1;
            gameManager.instance.UpdateCharges();
        }

        //fire ability
        else if (gameManager.instance.playerScript.abilities[gameManager.instance.playerScript.listpos].type == Ability.AbilityType.fire && gameManager.instance.playerScript.abilities[gameManager.instance.playerScript.listpos].currentcharge > 0)
        {
            Instantiate(gameManager.instance.playerScript.abilities[gameManager.instance.playerScript.listpos].abilityPrefab, gameManager.instance.player.transform.position + Vector3.up * 1 , gameManager.instance.player.transform.rotation);
            aud.pitch = Random.Range(0.8f, 1.2f);
            aud.PlayOneShot(gameManager.instance.playerScript.abilities[gameManager.instance.playerScript.listpos].abilitySound, gameManager.instance.sfxVolume);
            gameManager.instance.playerScript.abilities[gameManager.instance.playerScript.listpos].currentcharge -= 1;
            gameManager.instance.UpdateCharges();
        }
  //shockwave ability
        else if ( gameManager.instance.playerScript.abilities[gameManager.instance.playerScript.listpos].type == Ability.AbilityType.shockwave && gameManager.instance.playerScript.abilities[gameManager.instance.playerScript.listpos].currentcharge > 0)
        {
            StartCoroutine(Slamtime());
            aud.pitch = Random.Range(0.8f, 1.2f);
            aud.PlayOneShot(gameManager.instance.playerScript.abilities[gameManager.instance.playerScript.listpos].abilitySound, gameManager.instance.sfxVolume);
            gameManager.instance.playerScript.abilities[gameManager.instance.playerScript.listpos].currentcharge -= 1;
            gameManager.instance.UpdateCharges();
        }
  //ice ability
        else if ( gameManager.instance.playerScript.abilities[gameManager.instance.playerScript.listpos].type == Ability.AbilityType.ice && gameManager.instance.playerScript.abilities[gameManager.instance.playerScript.listpos].currentcharge > 0)
        {
            Instantiate(gameManager.instance.playerScript.abilities[gameManager.instance.playerScript.listpos].abilityPrefab, gameManager.instance.player.transform.position , gameManager.instance.player.transform.rotation);
            aud.pitch = Random.Range(0.8f, 1.2f);
            aud.PlayOneShot(gameManager.instance.playerScript.abilities[gameManager.instance.playerScript.listpos].abilitySound, gameManager.instance.sfxVolume);
            gameManager.instance.playerScript.abilities[gameManager.instance.playerScript.listpos].currentcharge -= 1;
            gameManager.instance.UpdateCharges();
        }
  //sonicBoom ability
        else if ( gameManager.instance.playerScript.abilities[gameManager.instance.playerScript.listpos].type == Ability.AbilityType.sonicboom && gameManager.instance.playerScript.abilities[gameManager.instance.playerScript.listpos].currentcharge > 0)
        {
            Instantiate(gameManager.instance.playerScript.abilities[gameManager.instance.playerScript.listpos].abilityPrefab, gameManager.instance.player.transform.position + Vector3.up * 1, gameManager.instance.player.transform.rotation);
            aud.pitch = Random.Range(0.8f, 1.2f);
            aud.PlayOneShot(gameManager.instance.playerScript.abilities[gameManager.instance.playerScript.listpos].abilitySound, gameManager.instance.sfxVolume);
            gameManager.instance.playerScript.abilities[gameManager.instance.playerScript.listpos].currentcharge -= 1;
            gameManager.instance.UpdateCharges();
        }
  //sheild ability
        else if ( gameManager.instance.playerScript.abilities[gameManager.instance.playerScript.listpos].type == Ability.AbilityType.sheild && gameManager.instance.playerScript.abilities[gameManager.instance.playerScript.listpos].currentcharge > 0)
        {

            Instantiate(gameManager.instance.playerScript.abilities[gameManager.instance.playerScript.listpos].abilityPrefab, gameManager.instance.player.transform);
            aud.pitch = Random.Range(0.8f, 1.2f);
            aud.PlayOneShot(gameManager.instance.playerScript.abilities[gameManager.instance.playerScript.listpos].abilitySound, gameManager.instance.sfxVolume);
            gameManager.instance.playerScript.isInvinc = true;
            gameManager.instance.playerScript.abilities[gameManager.instance.playerScript.listpos].currentcharge -= 1;
            gameManager.instance.UpdateCharges();
        }
    }

    void changegun()
    {

        damage = gameManager.instance.playerScript.abilities[gameManager.instance.playerScript.listpos].damage;
    }
    IEnumerator chargetime()
    {
        yield return new WaitForSeconds(.5f);
        RaycastHit[] hit = Physics.RaycastAll(transform.position+Vector3.up*1, transform.forward, 5, lightningtarget);
        for (int i = 0; i < hit.Length; i++)
        {
            Debug.Log("Hit" + hit[i].collider.name);
       
            Idamage dam = hit[i].collider.GetComponentInParent<Idamage>();
            if (dam != null)
            {
                dam.TakeDamage(damage);
            }
            else
            {
                Debug.Log("No Damageable component found on " + hit[i].collider.name);
            }
            Instantiate(lightningeffect, hit[i].point, Quaternion.LookRotation(hit[i].normal));
        }


    }

    IEnumerator Slamtime()
    {
        gameManager.instance.playerScript.transform.position = gameManager.instance.playerScript.transform.position + Vector3.up * 3;
        yield return new WaitForSeconds(.3f);
        Instantiate(gameManager.instance.playerScript.abilities[gameManager.instance.playerScript.listpos].abilityPrefab, gameManager.instance.player.transform.position + Vector3.up * 1, gameManager.instance.player.transform.rotation);
    }
}
