using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] CanvasGroup pauseScreen_Canvas;
    [SerializeField] CanvasGroup options_Canvas;
    [SerializeField] CanvasGroup settings_Canvas;
    [SerializeField] CanvasGroup graphics_Canvas;
    [SerializeField] CanvasGroup audio_Canvas;

    public static bool pause;

    void Update()
    {
        // Pause the Game
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!pause)
            {
                pause = true;

                pauseScreen_Canvas.alpha = 1f;
                pauseScreen_Canvas.blocksRaycasts = true;

                //Open the Pause-Screen
                options_Canvas.alpha = 1;
                options_Canvas.interactable = true;
                options_Canvas.blocksRaycasts = true;

                Time.timeScale = 0;
                AudioListener.volume = 0.5f;

                // Enable Cursor
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
        }
    }

    public void ResumeButton()
    {
        pause = false;

        pauseScreen_Canvas.alpha = 0;
        pauseScreen_Canvas.blocksRaycasts = false;

        // Close the Pause-Screen
        options_Canvas.alpha = 0;
        options_Canvas.interactable = false;
        options_Canvas.blocksRaycasts = false;

        Time.timeScale = 1;
        AudioListener.volume = 1f;

        // Disable Cursor
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void RestartButton()
    {
        pause = false;

        Time.timeScale = 1;
        AudioListener.volume = 1f;

        // Disable Cursor
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        SceneManager.LoadScene("TestScene1");
    }

    public void SettingsButton()
    {
        SwitchCanvas(options_Canvas, settings_Canvas);
    }

    public void SettingsBackButton()
    {
        SwitchCanvas(settings_Canvas, options_Canvas);
    }

    public void GraphicsButton()
    {
        SwitchCanvas(settings_Canvas, graphics_Canvas);
    }

    public void GraphicsBackButton()
    {
        SwitchCanvas(audio_Canvas, settings_Canvas);
    }

    public void AudioButton()
    {
        SwitchCanvas(settings_Canvas, audio_Canvas);
    }

    public void AudioBackButton()
    {
        SwitchCanvas(audio_Canvas, settings_Canvas);
    }

    // Goes back to Main-menu
    public void ExitButton()
    {
        pause = false;

        Time.timeScale = 1;
        AudioListener.volume = 1f;

        // Enable Cursor
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        SceneManager.LoadScene("MainMenu");
    }

    // Switching between Canvases
    void SwitchCanvas(CanvasGroup previousGroup, CanvasGroup nextGroup)
    {
        // Disable the previous canvas-group
        previousGroup.alpha = 0;
        previousGroup.interactable = false;
        previousGroup.blocksRaycasts = false;

        // Enable the next canvas-group
        nextGroup.alpha = 1;
        nextGroup.interactable = true;
        nextGroup.blocksRaycasts = true;
    }
}
