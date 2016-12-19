using System;
using UnityEngine;
using System.Collections;
using BubbleChip;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{

    public SettingsPanelController settings;

    private void Start()
    {
        Application.logMessageReceived += (condition, trace, type) =>
        {
            if(type == LogType.Exception)
                Dialog.Get().Set("Ups, sorry but we have little trouble: " + condition,
                    Application.Quit, "Quit game");
        };
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void QuitGame()
    {
        Dialog.Get().Set("Do you really want to quit game?", Application.Quit, null, "Quit");
    }

    public void ShowSettings()
    {
        settings.gameObject.SetActive(true);
    }
}
