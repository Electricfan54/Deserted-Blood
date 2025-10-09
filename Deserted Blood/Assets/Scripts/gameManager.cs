using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class gameManager : MonoBehaviour
{

    public static gameManager instance;
    public GameObject uiMain;

    public GameObject player;
    public PlayerController playerScript;

    [Header("UI Specific")]
    [SerializeField] GameObject menuActive;

    [SerializeField] GameObject menuPause;

    List<GameObject> menuHierarchy = new List<GameObject>();

    public Image playerHPBar;

    [Header("Transition Variables")]
    [SerializeField] Image transitionMain;
    float alphaLerp;

    bool transitionActive;

    [SerializeField] float transTimeScale;
    private float transStart;
    private float transEnd;
    float transTimer;

    private void Awake()
    {

        instance = this;

        player = GameObject.FindWithTag("Player");

        if (player != null)
        {
            playerScript = player.GetComponent<PlayerController>();
        }

        if (menuActive != null)
        {
            menuHierarchy.Add(menuActive);
        }

        transTimer = 0;
        alphaLerp = 0;

        transStart = 0;
        transEnd = 1;

    }

    // Update is called once per frame
    void Update()
    {
        if (transitionActive)
            Transition();
    }

    public void Transition()
    {

        transTimer += transTimeScale * Time.deltaTime;
        alphaLerp = Mathf.Lerp(transStart, transEnd, transTimer);

        transitionMain.color = new Color(transitionMain.color.r, transitionMain.color.g, transitionMain.color.b, alphaLerp);

        if (transTimer >= 1 && transEnd == 0)
        {
            transitionActive = false;
            float temp = transEnd;
            transEnd = transStart;
            transStart = temp;

            transTimer = 0;

            transitionMain.gameObject.SetActive(false);

        }

        if (transTimer >= 1)
        {
            float temp = transEnd;
            transEnd = transStart;
            transStart = temp;

            transTimer = 0;
        }

    }

    public void OpenSubMenu(GameObject submenu)
    {

        //transitionActive = true;
        //alphaLerp = 0;
        //transitionMain.gameObject.SetActive(true);

        menuActive.SetActive(false);
        menuHierarchy.Add(submenu);
        menuActive = submenu;
        menuActive.SetActive(true);

    }

    public void CloseAllMenus()
    {
        if (menuActive != null)
        {
            menuActive.SetActive(false);
            menuActive = null;
            menuHierarchy.Clear();
        }
    }

    public void CloseSubMenu() // Backs out in the menu list hierarchy
    {
        menuActive.SetActive(false);
        menuHierarchy.Remove(menuHierarchy[^1]);
        menuActive = menuHierarchy[^1];
        menuActive.SetActive(true);
    }

}
