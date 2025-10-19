using UnityEngine;
using UnityEngine.SceneManagement;

public class buttonManager : MonoBehaviour
{

    [SerializeField] float sfxPreviewCD;
    [SerializeField] float musPreviewCD;

    float sfxPreviewtimer;
    float musPreviewtimer;



    private void Update()
    {

        sfxPreviewtimer += Time.unscaledDeltaTime;
        musPreviewtimer += Time.unscaledDeltaTime;

    }

    public void Play(string playSceneName)
    {

        DontDestroyOnLoad(gameManager.instance.uiMain);

        if (playSceneName.Length != 0)
        {
            Scene loadScene = SceneManager.GetSceneByName(playSceneName);
            SceneManager.LoadScene(playSceneName);
        }

        gameManager.instance.soundEffects.PlayOneShot(gameManager.instance.menuPop);
        gameManager.instance.CloseAllMenus();

    }

    public void QuitToMenu()
    {

        Scene loadScene = SceneManager.GetSceneByName("MainMenu");
        SceneManager.LoadScene("MainMenu");

    }

    public void Resume()
    {

        gameManager.instance.UnpauseGame();
        gameManager.instance.soundEffects.PlayOneShot(gameManager.instance.menuPop);

    }

    public void Retry()
    {

        gameManager.instance.UnpauseGame();
        gameManager.instance.RespawnPlayer();
        gameManager.instance.soundEffects.PlayOneShot(gameManager.instance.menuPop);

    }

    public void Quit()
    {

        gameManager.instance.soundEffects.PlayOneShot(gameManager.instance.menuPop);

    #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
    #else
            Application.Quit();
    #endif

    }

    public void OpenSubMenu(GameObject submenu)
    {

        gameManager.instance.OpenSubMenu(submenu);
        gameManager.instance.soundEffects.PlayOneShot(gameManager.instance.menuPop);

    }

    public void CloseSubMenu()
    {

        gameManager.instance.CloseSubMenu();
        gameManager.instance.soundEffects.PlayOneShot(gameManager.instance.menuPop);

    }

    public void UpdateVolumeSlider()
    {
        gameManager.instance.UpdateVolume();
    }

    public void PreviewSFXVolume()
    {

        if (sfxPreviewtimer >= sfxPreviewCD)
        {
            gameManager.instance.soundEffects.PlayOneShot(gameManager.instance.menuPop);
            sfxPreviewtimer = 0;
        }

    }

    public void PreviewMusVolume()
    {

        if (musPreviewtimer >= musPreviewCD)
        {
            gameManager.instance.backgroundMusic.PlayOneShot(gameManager.instance.menuPop);
            musPreviewtimer = 0;
        }

    }

}
