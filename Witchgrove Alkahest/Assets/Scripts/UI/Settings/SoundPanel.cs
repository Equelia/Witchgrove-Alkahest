using UnityEngine;
using UnityEngine.UI;

public class SoundPanel : MonoBehaviour
{
    [Header("UI Sliders")]
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;

    [Header("UI Elements")] 
    [SerializeField] private Button returnButton;

    private const string MasterVolumeKey = "MasterVolume";
    private const string MusicVolumeKey = "MusicVolume";
    private const string SfxVolumeKey = "SfxVolume";

    private void Start()
    {
        LoadVolumes();

        masterVolumeSlider.onValueChanged.AddListener(SetMasterVolume);
        musicVolumeSlider.onValueChanged.AddListener(SetMusicVolume);
        sfxVolumeSlider.onValueChanged.AddListener(SetSfxVolume);
        returnButton.onClick.AddListener(ReturnToSettings);
        
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        masterVolumeSlider.onValueChanged.RemoveListener(SetMasterVolume);
        musicVolumeSlider.onValueChanged.RemoveListener(SetMusicVolume);
        sfxVolumeSlider.onValueChanged.RemoveListener(SetSfxVolume);
        returnButton.onClick.RemoveListener(ReturnToSettings);
    }

    private void LoadVolumes()
    {
        float master = PlayerPrefs.GetFloat(MasterVolumeKey, 1f);
        float music = PlayerPrefs.GetFloat(MusicVolumeKey, 1f);
        float sfx = PlayerPrefs.GetFloat(SfxVolumeKey, 1f);

        masterVolumeSlider.value = master;
        musicVolumeSlider.value = music;
        sfxVolumeSlider.value = sfx;

        ApplyVolumes(master, music, sfx);
    }

    private void SetMasterVolume(float value)
    {
        PlayerPrefs.SetFloat(MasterVolumeKey, value);
        PlayerPrefs.Save();
        ApplyVolumes(value, musicVolumeSlider.value, sfxVolumeSlider.value);
    }

    private void SetMusicVolume(float value)
    {
        PlayerPrefs.SetFloat(MusicVolumeKey, value);
        PlayerPrefs.Save();
        ApplyVolumes(masterVolumeSlider.value, value, sfxVolumeSlider.value);
    }

    private void SetSfxVolume(float value)
    {
        PlayerPrefs.SetFloat(SfxVolumeKey, value);
        PlayerPrefs.Save();
        ApplyVolumes(masterVolumeSlider.value, musicVolumeSlider.value, value);
    }

    private void ApplyVolumes(float master, float music, float sfx)
    {
        if (SoundManager.Instance == null)
            return;

        if (SoundManager.Instance.musicSource != null)
            SoundManager.Instance.musicSource.volume = master * music / 10;

        foreach (var src in SoundManager.Instance.GetAllSfxSources())
        {
            if (src != null)
                src.volume = master * sfx;
        }
    }
    
    private void ReturnToSettings()
    {
        gameObject.SetActive(false);
    }
}
