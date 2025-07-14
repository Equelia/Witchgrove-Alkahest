using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class GraphicPanel : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private TMP_Dropdown qualityDropdown;
    [SerializeField] private Slider brightnessSlider;
    [SerializeField] private Button returnButton;

    [Header("Post Processing")]
    [SerializeField] private Volume postProcessingVolume;

    private ColorAdjustments colorAdjustments;

    private Resolution[] resolutions;

    private const string ResolutionKey = "ScreenResolution";
    private const string QualityKey = "QualityLevel";
    private const string BrightnessKey = "BrightnessLevel";

    void Start()
    {
        if (postProcessingVolume.profile.TryGet(out colorAdjustments) == false)
        {
            Debug.LogWarning("Color Adjustments not found in Post Processing Volume.");
        }

        SetupResolutionDropdown();
        SetupQualityDropdown();
        LoadSettings();

        resolutionDropdown.onValueChanged.AddListener(SetResolution);
        qualityDropdown.onValueChanged.AddListener(SetQuality);
        brightnessSlider.onValueChanged.AddListener(SetBrightness);
        returnButton.onClick.AddListener(ReturnToSettings);
        
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        resolutionDropdown.onValueChanged.RemoveListener(SetResolution);
        qualityDropdown.onValueChanged.RemoveListener(SetQuality);
        brightnessSlider.onValueChanged.RemoveListener(SetBrightness);
        returnButton.onClick.RemoveListener(ReturnToSettings);    
    }

    private void SetupResolutionDropdown()
    {
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        var options = new System.Collections.Generic.List<string>();
        int currentIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            string res = $"{resolutions[i].width}x{resolutions[i].height}";
            options.Add(res);

            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
            {
                currentIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = PlayerPrefs.GetInt(ResolutionKey, currentIndex);
        resolutionDropdown.RefreshShownValue();
    }

    private void SetupQualityDropdown()
    {
        string[] qualityNames = QualitySettings.names;
        qualityDropdown.ClearOptions();

        qualityDropdown.AddOptions(new System.Collections.Generic.List<string>(qualityNames));
        qualityDropdown.value = PlayerPrefs.GetInt(QualityKey, QualitySettings.GetQualityLevel());
        qualityDropdown.RefreshShownValue();
    }

    private void LoadSettings()
    {
        int resIndex = PlayerPrefs.GetInt(ResolutionKey, resolutionDropdown.value);
        SetResolution(resIndex);

        int qualityIndex = PlayerPrefs.GetInt(QualityKey, qualityDropdown.value);
        SetQuality(qualityIndex);

        float brightness = PlayerPrefs.GetFloat(BrightnessKey, 0.3f); 
        brightnessSlider.value = brightness;
        SetBrightness(brightness);
    }

    private void SetResolution(int index)
    {
        if (resolutions == null || resolutions.Length == 0)
            return;

        Resolution res = resolutions[index];
        Screen.SetResolution(res.width, res.height, Screen.fullScreen);
        PlayerPrefs.SetInt(ResolutionKey, index);
    }

    private void SetQuality(int index)
    {
        QualitySettings.SetQualityLevel(index, true);
        PlayerPrefs.SetInt(QualityKey, index);
    }

    private void SetBrightness(float value)
    {
        if (colorAdjustments != null)
        {
            colorAdjustments.postExposure.value = value;
        }

        PlayerPrefs.SetFloat(BrightnessKey, value);
    }

    private void ReturnToSettings()
    {
        gameObject.SetActive(false);
    }
}
