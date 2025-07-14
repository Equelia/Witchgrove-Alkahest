using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsController : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Slider mouseSensitivitySlider;
    [SerializeField] private TMP_Dropdown fpsDropdown;
    
    [Header("Components")]
    [SerializeField] private FirstPersonController firstPersonController;
    
    private const string MouseSensitivityKey = "MouseSensitivity";
    private const string FpsLimitKey = "FpsLimit";

    private void Start()
    {
        LoadSettings();

        mouseSensitivitySlider.onValueChanged.AddListener(OnMouseSensitivityChanged);
        fpsDropdown.onValueChanged.AddListener(OnFpsDropdownChanged);
        
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        mouseSensitivitySlider.onValueChanged.RemoveListener(OnMouseSensitivityChanged);
        fpsDropdown.onValueChanged.RemoveListener(OnFpsDropdownChanged);
    }

    private void LoadSettings()
    {
        float savedSensitivity = PlayerPrefs.GetFloat(MouseSensitivityKey, 0.5f);
        mouseSensitivitySlider.value = savedSensitivity;

        int savedFpsIndex = PlayerPrefs.GetInt(FpsLimitKey, 0);
        fpsDropdown.value = savedFpsIndex;
        ApplyFpsLimit(savedFpsIndex);
    }

    private void OnMouseSensitivityChanged(float value)
    {
        PlayerPrefs.SetFloat(MouseSensitivityKey, value);
        PlayerPrefs.Save();
        firstPersonController.SetMouseSensitivity(value);
    }

    private void OnFpsDropdownChanged(int index)
    {
        PlayerPrefs.SetInt(FpsLimitKey, index);
        PlayerPrefs.Save();
        ApplyFpsLimit(index);
    }

    private void ApplyFpsLimit(int index)
    {
        QualitySettings.vSyncCount = 0;
        
        switch (index)
        {
            case 0: Application.targetFrameRate = -1; break; 
            case 1: Application.targetFrameRate = 20; break;
            case 2: Application.targetFrameRate = 30; break;
            case 3: Application.targetFrameRate = 40; break;
            case 4: Application.targetFrameRate = 60; break;
            case 5: Application.targetFrameRate = 120; break;
            default: Application.targetFrameRate = -1; break;
        }
    }
}
