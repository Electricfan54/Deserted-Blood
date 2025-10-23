using System.Collections;
using TMPro;
using UnityEngine;

public class playerablities : MonoBehaviour
{

    int damage;
    [SerializeField] LayerMask lightningtarget;
    [SerializeField] ParticleSystem lightningeffect;
    [SerializeField] AudioSource aud;
    Ability currentability;
    [SerializeField] GameObject player;
   [SerializeField] PlayerController playerScript;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
 player = gameManager.instance.player;
        playerScript = player.GetComponent<PlayerController>();
    }
 

    // Update is called once per frame
    void Update()
    {
        selectability();
        if (Input.GetKeyDown(KeyCode.L))
        {
            if (gameManager.instance.playerScript.abilities.Count != 0)
            {
               currentability = playerScript.abilities[playerScript.listpos];
            }
           
            useAbility();
        }

    }

    void selectability()
    {
        if (Input.GetKeyDown(KeyCode.Q) && playerScript.listpos < playerScript.abilities.Count - 1)
        {
            playerScript.listpos++;
            gameManager.instance.UpdateAbilityUI();
            changegun();
        }
        else if (Input.GetKeyDown(KeyCode.W) && playerScript.listpos > 0)
        {
            playerScript.listpos--;
            gameManager.instance.UpdateAbilityUI();
            changegun();
        }

    }

    void useAbility()
    {

        if (gameManager.instance.playerScript.abilities.Count == 0)
        {
            return;
        }
        
        //Teleport ability
        else if (currentability.type == Ability.AbilityType.tp && currentability.currentcharge > 0)
        {

            if (playerScript.isGrounded == false)
            {
               playerScript.abilities[playerScript.listpos].currentcharge -= 1;
                RaycastHit hit;
                if (Physics.Raycast(transform.position, transform.forward, out hit, 5))
                {
                  player.transform.position = hit.point - player.transform.forward;
                    aud.pitch = Random.Range(0.8f, 1.2f);
                    aud.PlayOneShot(currentability.abilitySound, gameManager.instance.sfxVolume);
                    gameManager.instance.UpdateCharges();
                }
                else
                {
                   player.transform.position = player.transform.position +player.transform.forward * 5;
                    aud.pitch = Random.Range(0.8f, 1.2f);
                    aud.PlayOneShot(currentability.abilitySound, gameManager.instance.sfxVolume);
                    gameManager.instance.UpdateCharges();
                }
            }
            else if (!gameManager.instance.toolTipText.activeSelf)
            {
                StartCoroutine(ShowAirTeleportPopup());
            }



        }

        //lightning ability
        else if (currentability.type == Ability.AbilityType.lightning && currentability.currentcharge > 0)
        {

            damage = currentability.damage;
            Instantiate(currentability.abilityPrefab,player.transform.position + Vector3.up * 1, player.transform.rotation);
            StartCoroutine(chargetime());
            aud.pitch = Random.Range(0.8f, 1.2f);
            aud.PlayOneShot(currentability.abilitySound, gameManager.instance.sfxVolume);
            Debug.DrawRay(gameObject.transform.position, gameObject.transform.forward * 6, Color.red);
           playerScript.abilities[playerScript.listpos].currentcharge -= 1;
            gameManager.instance.UpdateCharges();
        }

        //fire ability
        else if (currentability.type == Ability.AbilityType.fire && currentability.currentcharge > 0)
        {
            Instantiate(currentability.abilityPrefab, player.transform.position + Vector3.up * 1, player.transform.rotation);
            aud.pitch = Random.Range(0.8f, 1.2f);
            aud.PlayOneShot(currentability.abilitySound, gameManager.instance.sfxVolume);
            playerScript.abilities[playerScript.listpos].currentcharge -= 1;
            gameManager.instance.UpdateCharges();
        }
        //shockwave ability
        else if (currentability.type == Ability.AbilityType.shockwave && currentability.currentcharge > 0)
        {
            StartCoroutine(Slamtime());
            aud.pitch = Random.Range(0.8f, 1.2f);
            aud.PlayOneShot(currentability.abilitySound, gameManager.instance.sfxVolume);
         playerScript.abilities[playerScript.listpos].currentcharge -= 1;
            gameManager.instance.UpdateCharges();
        }
        //ice ability
        else if (currentability.type == Ability.AbilityType.ice &&currentability.currentcharge > 0)
        {
            Instantiate(currentability.abilityPrefab, player.transform.position,player.transform.rotation);
            aud.pitch = Random.Range(0.8f, 1.2f);
            aud.PlayOneShot(currentability.abilitySound, gameManager.instance.sfxVolume);
            playerScript.abilities[playerScript.listpos].currentcharge -= 1;
            gameManager.instance.UpdateCharges();
        }
        //sonicBoom ability
        else if (currentability.type == Ability.AbilityType.sonicboom && currentability.currentcharge > 0)
        {
            Instantiate(currentability.abilityPrefab, player.transform.position + Vector3.up * 1, player.transform.rotation);
            aud.pitch = Random.Range(0.8f, 1.2f);
            aud.PlayOneShot(currentability.abilitySound, gameManager.instance.sfxVolume);
           playerScript.abilities[playerScript.listpos].currentcharge -= 1;
            gameManager.instance.UpdateCharges();
        }
        //sheild ability
        else if (currentability.type == Ability.AbilityType.sheild && currentability.currentcharge > 0)
        {

            Instantiate(currentability.abilityPrefab, player.transform);
            aud.pitch = Random.Range(0.8f, 1.2f);
            aud.PlayOneShot(currentability.abilitySound, gameManager.instance.sfxVolume);
           playerScript.isInvinc = true;
           playerScript.abilities[playerScript.listpos].currentcharge -= 1;
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
        RaycastHit[] hit = Physics.RaycastAll(transform.position + Vector3.up * 1, transform.forward, 5, lightningtarget);
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
        Instantiate(currentability.abilityPrefab, gameManager.instance.player.transform.position + Vector3.up * 1, gameManager.instance.player.transform.rotation);
    }

    IEnumerator ShowAirTeleportPopup()
    {
        gameManager.instance.toolTipText.SetActive(true);
        TMP_Text text = gameManager.instance.toolTipText.GetComponentInChildren<TMP_Text>();
        text.color = Color.red;
        text.text = "Air teleport only works in the air";
        yield return new WaitForSeconds(1);
        gameManager.instance.toolTipText.SetActive(false);
    }
}
