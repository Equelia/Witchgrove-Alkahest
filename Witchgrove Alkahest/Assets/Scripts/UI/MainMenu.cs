using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [Header("UI Buttons")] 
    [SerializeField] private Button startButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button authorsButton;
    [SerializeField] private Button exitButton;

    [Header("UI Elements")] 
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject authorsPanel;

    private void Awake()
    {
        startButton.onClick.AddListener(StartGame);
        settingsButton.onClick.AddListener(Settings);
        authorsButton.onClick.AddListener(OpenAuthors);
        exitButton.onClick.AddListener(Exit);
    }

    private void OnDestroy()
    {
        startButton.onClick.RemoveListener(StartGame);
        settingsButton.onClick.RemoveListener(Settings);
        authorsButton.onClick.RemoveListener(OpenAuthors);
        exitButton.onClick.RemoveListener(Exit);
    }

    private void StartGame()
    {
        SceneManager.LoadScene(1);
    }

    private void Settings()
    { 
        settingsPanel.SetActive(true); 
    }

    private void OpenAuthors()
    {
        //authorsPanel.SetActive(true);
    }

    private void Exit()
    {
        Application.Quit();
    }
}
