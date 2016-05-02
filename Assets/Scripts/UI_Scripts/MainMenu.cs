using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] CanvasGroup titleMenu_Canvas;
    [SerializeField] CanvasGroup levelSelection_Canvas;
    [SerializeField] CanvasGroup options_Canvas;
    [SerializeField] CanvasGroup credits_Canvas;    

    // TITLE - SCENE //
    public void Play_Button()
    {
        SwitchCanvas(titleMenu_Canvas, levelSelection_Canvas);
    }

    public void Options_Button()
    {
        SwitchCanvas(titleMenu_Canvas, options_Canvas);
    }

    public void Credits_Button()
    {
        SwitchCanvas(titleMenu_Canvas, credits_Canvas);
    }

    public void Exit_Button()
    {
        Application.Quit();
    }

    // LEVEL SELECTION - SCENE //
    public void StoryMode_Button()
    {
        SceneManager.LoadScene("TestScene1");
    }

    public void SelectALevel_Button()
    {
        // Displays the different levels - unlocked / locked
    }

    public void LevelSelection_Back_Button()
    {
        SwitchCanvas(levelSelection_Canvas, titleMenu_Canvas);
    }

    // OPTIONS - SCENE //
    public void FullScreenMode_Button()
    {
        Screen.SetResolution(1366, 720, true);
    }

    public void WindowedMode_Button()
    {
        Screen.SetResolution(1280, 720, false);
    }    

    public void Options_Back_Button()
    {
        SwitchCanvas(options_Canvas, titleMenu_Canvas);
    }

    // CREDITS - SCENE //
    public void Credits_Back_Button()
    {
        SwitchCanvas(credits_Canvas, titleMenu_Canvas);
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
