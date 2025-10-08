using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Device;

public class gameManager : MonoBehaviour
{

    public static gameManager instance;

    public GameObject player;

    [Header("UI Specific")]
    [SerializeField] GameObject menuActive;

    [SerializeField] GameObject menuPause;

    List<GameObject> menuHierarchy = new List<GameObject>();

    private void Awake()
    {

        instance = this;

        player = GameObject.FindWithTag("Player");
        
        if (menuActive != null)
        {
            menuHierarchy.Add(menuActive);
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OpenSubMenu(GameObject submenu)
    {

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
