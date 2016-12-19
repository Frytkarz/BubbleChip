using UnityEngine;
using System.Collections;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsPanelController : MonoBehaviour
{
    private const string KEY_SOUND_EFFECTS_VOLUME = "SoundEffectsVolume";
    private const string KEY_MUSIC_VOLUME = "MusicVolume";
    private const short MIN_VOLUME = -80;

    public Slider sldSoundEffects, sldMusic;
    public AudioMixer audioMixer;

    private void OnEnable()
    {
        float soundEffectsVolume, musicVolume;
        audioMixer.GetFloat(KEY_SOUND_EFFECTS_VOLUME, out soundEffectsVolume);
        sldSoundEffects.value = soundEffectsVolume - MIN_VOLUME;

        audioMixer.GetFloat(KEY_MUSIC_VOLUME, out musicVolume);
        sldMusic.value = musicVolume - MIN_VOLUME;
    }

    public void OnSldSoundEffectsValueChanged()
    {
        audioMixer.SetFloat(KEY_SOUND_EFFECTS_VOLUME, sldSoundEffects.value + MIN_VOLUME);
    }

    public void OnSldMusicValueChanged()
    {
        audioMixer.SetFloat(KEY_MUSIC_VOLUME, sldMusic.value + MIN_VOLUME);
    }

    public void OnCloseClick()
    {
        gameObject.SetActive(false);
    }
}
