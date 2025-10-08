using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class gameManager : MonoBehaviour
{

    public static gameManager instance;

    public GameObject player;
    public PlayerController playerScript;

    [Header("UI Specific")]
    [SerializeField] GameObject menuActive;

    [SerializeField] GameObject menuPause;

    List<GameObject> menuHierarchy = new List<GameObject>();

    [Header("Transition Variables")]
    [SerializeField] Image transitionMain;

    bool transitionActive;
    [SerializeField] float transDuration;
    float transTimer;

    private void Awake()
    {

        instance = this;

        player = GameObject.FindWithTag("Player");
        playerScript = player.GetComponent<PlayerController>();
        if (menuActive != null)
        {
            menuHierarchy.Add(menuActive);
        }

        transTimer = 0;

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Transition()
    {

        float alphaLerp = Mathf.Lerp(0, transDuration, Time.deltaTime * transTimer);
        

        if (transTimer >= transDuration)
        {
            transitionActive = false;
            //transitionMain.gameObject.SetActive(false);
        }

    }

    public void OpenSubMenu(GameObject submenu)
    {

        transitionActive = true;
        transitionMain.gameObject.SetActive(true);

        while (transitionActive)
        {
            Transition();
        }

        menuActive.SetActive(false);
        menuHierarchy.Add(submenu);
        menuActive = submenu;
        menuActive.SetActive(true);

    }

    public void CloseSubMenu() // Backs out in the menu list hierarchy
    {
        menuActive.SetActive(false);
        menuHierarchy.Remove(menuHierarchy[^1]);
        menuActive = menuHierarchy[^1];
        menuActive.SetActive(true);
    }

}
