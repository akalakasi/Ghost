using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] CanvasGroup pauseScreen_Canvas;
    [SerializeField] CanvasGroup options_Canvas;
    [SerializeField] CanvasGroup graphics_Canvas;
    [SerializeField] CanvasGroup audio_Canvas;

    public static bool pause;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!pause)
            {
                pause = true;

                pauseScreen_Canvas.alpha = 0.9f;
                pauseScreen_Canvas.blocksRaycasts = true;

                //Open the Pause-Screen
                options_Canvas.alpha = 1;
                options_Canvas.interactable = true;
                options_Canvas.blocksRaycasts = true;

                Time.timeScale = 0;
                AudioListener.volume = 0.5f;
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
    }

    public void RestartButton()
    {
        pause = false;

        Time.timeScale = 1;
        AudioListener.volume = 1f;

        SceneManager.LoadScene("TestScene1");
    }

    public void GraphicsButton()
    {
        SwitchCanvas(options_Canvas, graphics_Canvas);
    }

    public void GraphcisBackButton()
    {
        SwitchCanvas(graphics_Canvas, options_Canvas);
    }

    public void AudioButton()
    {
        SwitchCanvas(options_Canvas, audio_Canvas);
    }

    public void AudioBackButton()
    {
        SwitchCanvas(audio_Canvas, options_Canvas);
    }

    // Goes back to Main-menu
    public void ExitButton()
    {
        pause = false;

        Time.timeScale = 1;
        AudioListener.volume = 1f;

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
