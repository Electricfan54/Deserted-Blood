using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using Unity.Mathematics;
using UnityEngine.SceneManagement;
using TMPro;

[System.Serializable]
public struct AbilitySlotMain
{
    [SerializeField] public GameObject chargeFrame;
    [SerializeField] public Image abilityIcon;
    [SerializeField] public int maxCharges;
    [SerializeField] public int currCharges;

    [SerializeField] public List<GameObject> chargeList;
}

public class gameManager : MonoBehaviour
{

    public static gameManager instance;
    public GameObject uiMain;

    public GameObject player;
    public PlayerController playerScript;

    [Header("UI Specific")]
    [SerializeField] GameObject menuActive;

    [SerializeField] GameObject menuMain;
    [SerializeField] GameObject menuPause;

    public bool isPaused;
    float timeScaleOrig;

    List<GameObject> menuHierarchy = new List<GameObject>();

    [SerializeField] bool mainMenuActive;

    [SerializeField] Slider sliderSFX, sliderMus;
    [SerializeField] TMP_Text volTextSFX, volTextMus;

    public Image playerHPBar;
    public GameObject playerHUD;

    public GameObject bossBarUI;
    public Image bossBar;
    public TMP_Text bossNameText;

    int bossHPMax;

    [Header("Ability UI")]
    [SerializeField] GameObject chargeBase;

    public AbilitySlotMain abilitySlot1;
    public AbilitySlotMain abilitySlot2;
    public AbilitySlotMain abilitySlot3;

    [SerializeField] float frameHeight;
    [SerializeField] float frameSpacing;

    [SerializeField][Range(1,3)] int slotSelected;
    [SerializeField] GameObject selHighlight;

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
        timeScaleOrig = Time.timeScale;

        AttemptPlayerAssign();

        if (mainMenuActive)
        {
            menuActive = menuMain;
            menuActive.SetActive(true);
        }
        else
        {
            menuActive = null;
        }

        if (menuActive != null)
        {
            menuHierarchy.Add(menuActive);
        }

        transTimer = 0;
        alphaLerp = 0;

        transStart = 0;
        transEnd = 1;

        SceneManager.sceneLoaded += OnSceneLoaded;

    }

    private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {

        AttemptPlayerAssign();
        
    }

    private void AttemptPlayerAssign()
    {
        player = GameObject.FindWithTag("Player");

        if (player != null)
        {
            playerScript = player.GetComponent<PlayerController>();
        }
    }

    private void Start()
    {

        UpdateVolume();

        abilitySlot1.currCharges = abilitySlot1.maxCharges;
        abilitySlot2.currCharges = abilitySlot2.maxCharges;
        abilitySlot3.currCharges = abilitySlot3.maxCharges;

        AssignAbility(abilitySlot1);
        AssignAbility(abilitySlot2);
        AssignAbility(abilitySlot3);

    }

    // Update is called once per frame
    void Update()
    {
        if (transitionActive)
            Transition();

        UpdateAbilityUI();
        PauseFunction();

    }

    void PauseFunction()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            if (menuActive == null)
            {
                PauseGame();
                menuHierarchy.Add(menuPause);
                menuActive = menuPause;
                menuActive.SetActive(true);
            }
            else if (menuActive == menuPause)
            {
                UnpauseGame();
            }
        }
    }

    public void PauseGame()
    {

        isPaused = !isPaused;
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

    }

    public void UnpauseGame()
    {

        isPaused = !isPaused;
        Time.timeScale = timeScaleOrig;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        menuActive.SetActive(false);
        menuActive = null;
        menuHierarchy.Clear();

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

    public void UpdateVolume()
    {

        volTextSFX.text = (sliderSFX.value * 100).ToString("F0") + "%";
        volTextMus.text = (sliderMus.value * 100).ToString("F0") + "%";

    }

    public void AssignAbility(AbilitySlotMain selSlot)
    {

        if (selSlot.chargeList.Count > 0)
        {
            for (int i = 0; i < selSlot.maxCharges; i++)
            {
                Destroy(selSlot.chargeList[i]);
            }

            selSlot.chargeList.Clear();

        }

        for (int i = 0; i < selSlot.maxCharges; i++)
        {

            GameObject newCharge = Instantiate(chargeBase);
            newCharge.SetActive(true);

            newCharge.transform.SetParent(selSlot.chargeFrame.transform);

            newCharge.transform.localPosition = new Vector2(0, ((frameHeight / selSlot.maxCharges) * i + frameSpacing) - (frameSpacing / 2));
            newCharge.transform.localScale = new Vector2(newCharge.transform.localScale.x, frameHeight / selSlot.maxCharges - frameSpacing);

            selSlot.chargeList.Add(newCharge);

        }
    


    }

    public void UpdateAbilityUI()
    {

        UpdateCharges(abilitySlot1);
        UpdateCharges(abilitySlot2);
        UpdateCharges(abilitySlot3);

        switch (slotSelected)
        {
            case 1:
                selHighlight.transform.localPosition = new Vector2(810, -425);
                break;
            case 2:
                selHighlight.transform.localPosition = new Vector2(610, -425);
                break;
            case 3:
                selHighlight.transform.localPosition = new Vector2(410, -425);
                break;
        }

    }

    public void UpdateCharges(AbilitySlotMain selSlot)
    {

        for (int i = 0; i < (selSlot.maxCharges - selSlot.currCharges); i++)
        {

            selSlot.chargeList[(selSlot.maxCharges - 1) - i].GetComponent<Image>().color = new Color(0.0925596f, 0.1941795f, 0.4528302f);

        }

    }

    public void ShowBossBar(string bossName, int maxHealth)
    {
        bossBarUI.SetActive(true);
        bossNameText.text = bossName;
        bossHPMax = maxHealth;
        
    }

    public void HideBossBar()
    {
        bossBarUI.SetActive(false);
    }

    public void UpdateBossBar(int currHealth)
    {
        bossBar.fillAmount = (float)currHealth / (float)bossHPMax;
    }

}
