using UnityEngine;
using UnityEngine.SceneManagement;

public class buttonManager : MonoBehaviour
{

    public void Play(string playSceneName)
    {

        DontDestroyOnLoad(gameManager.instance.uiMain);

        if (playSceneName.Length != 0)
        {
            Scene loadScene = SceneManager.GetSceneByName(playSceneName);
            SceneManager.LoadScene(playSceneName);
        }

        gameManager.instance.CloseAllMenus();

    }

    public void Quit()
    {

    #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
    #else
            Application.Quit();
    #endif

    }

    public void OpenSubMenu(GameObject submenu)
    {

        gameManager.instance.OpenSubMenu(submenu);

    }

    public void CloseSubMenu()
    {

        gameManager.instance.CloseSubMenu();

    }

    public void UpdateVolumeSlider()
    {
        gameManager.instance.UpdateVolume();
    }

}
