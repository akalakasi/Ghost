using UnityEngine;
using System.Collections;

public class ResolutionSetting : MonoBehaviour
{
    [SerializeField] GameObject[] screenModeButtons;
    [SerializeField] GameObject[] resolutionButtons;

    [SerializeField] GameObject applySettingsWindow;

    int _currScreenModeButton;
    int _currResolutionButton;
    int _screenHeight;
    int _screenWidth;

    bool _isFullScreen;    

    void Awake()
    {
        // Get current screen-settings
        _isFullScreen = Screen.fullScreen;
        _screenWidth = Screen.currentResolution.width;
        _screenHeight = Screen.currentResolution.height;
    }

    // SCREEN-MODE //
    public void FullScreenMode()
    {        
        _isFullScreen = true;
    }

    public void WindowedMode()
    {
        _isFullScreen = false;
    }

    public void ScreenMode_DirButton(int _addValue)
    {
        // Turn off this button
        screenModeButtons[_currScreenModeButton].SetActive(false);

        // select the next button
        _currScreenModeButton += _addValue;

        // Keep within the number of buttons
        if (_currScreenModeButton >= screenModeButtons.Length)
        {
            _currScreenModeButton = 0;
        }
        else if (_currScreenModeButton < 0)
        {
            _currScreenModeButton = screenModeButtons.Length - 1;
        }        

        // Turn on the next button
        screenModeButtons[_currScreenModeButton].SetActive(true);
    }

    // ASPECT-RATIO //
    public void ScreenHeight(int _h)
    {
        _screenHeight = _h;
    }

    public void ScreenWidth(int _w)
    {
        _screenWidth = _w;
    }

    public void Resolution_DirButton(int _addValue)
    {
        // Turn off this button
        resolutionButtons[_currResolutionButton].SetActive(false);

        // select the next button
        _currResolutionButton += _addValue;

        // Keep within the number of buttons
        if (_currResolutionButton >= resolutionButtons.Length)
        {
            _currResolutionButton = 0;
        }
        else if (_currResolutionButton < 0)
        {
            _currResolutionButton = resolutionButtons.Length - 1;
        }

        // Turn on the next button
        resolutionButtons[_currResolutionButton].SetActive(true);
    }

    // APPLY-SETTINGS Window //
    public void ApplyButton()
    {
        applySettingsWindow.SetActive(true);
    }

    public void ApplyYesButton()
    {
        // Set the new screen-settings
        Screen.SetResolution(_screenWidth, _screenHeight, _isFullScreen);

        applySettingsWindow.SetActive(false);
    }

    public void ApplyNoButton()
    {
        applySettingsWindow.SetActive(false);
    }
}
