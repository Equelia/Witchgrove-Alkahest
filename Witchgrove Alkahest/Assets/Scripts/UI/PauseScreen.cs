using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseScreen : MonoBehaviour
{
	[Header("UI Buttons")] 
	[SerializeField] private Button continueButton;
	[SerializeField] private Button settingsButton;
	[SerializeField] private Button mainMenuButton;

	[Header("UI Elements")] 
	[SerializeField] private GameObject settingsPanel;

	private void Awake()
	{
		continueButton.onClick.AddListener(Continue);
		settingsButton.onClick.AddListener(Settings);
		mainMenuButton.onClick.AddListener(MainMenu);
	}

	private void OnDestroy()
	{
		continueButton.onClick.RemoveListener(Continue);
		settingsButton.onClick.RemoveListener(Settings);
		mainMenuButton.onClick.RemoveListener(MainMenu);
	}

	private void Continue()
	{
		gameObject.SetActive(false);
	}

	private void Settings()
	{
		// TODO
		// settingsPanel.SetActive(true);
		// gameObject.SetActive(false);
	}

	private void MainMenu()
	{
		//TODO //SceneManager.LoadScene("MainMenu");
	}
}
