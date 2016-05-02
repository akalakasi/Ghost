using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class SoundSetting : MonoBehaviour
{
    [SerializeField] Slider bgmSlider;
    [SerializeField] Slider sfxSlider; 
    [SerializeField] AudioMixer bgmMixer;
    [SerializeField] AudioMixer sfxMixer;

    float bgmVol;
    float sfxVol;

    // Use this for initialization
	void Awake ()
    {
        // Set the UI-settings to the current settings
        bgmMixer.GetFloat("BGM_Vol", out bgmVol);
        sfxMixer.GetFloat("SFX_Vol", out sfxVol);
        bgmSlider.value = bgmVol;
        sfxSlider.value = sfxVol;
	}

    public void BGM_Slider(float bgmVol)
    {
        // Sets the volume of the Background Music
        bgmMixer.SetFloat("BGM_Vol", bgmVol);
    }

    public void SFX_Slider(float sfxVol)
    {
        // Sets the volume of the SFX 
        sfxMixer.SetFloat("SFX_Vol", sfxVol);
    }
}
