using System;
using UnityEngine;
using UnityEngine.UI;

public class SettingsPanel : MonoBehaviour
{
	[Header("UI Elements")] 
	[SerializeField] private GameObject graphicPanel;
	[SerializeField] private GameObject soundPanel;
	
	[Header("Button Elements")]
	[SerializeField] private Button graphicsButton;
	[SerializeField] private Button soundButton;
	[SerializeField] private Button returnButton;
	
	[Space, SerializeField] private SettingsController settingsController;

	private void Awake()
	{
		graphicsButton.onClick.AddListener(OpenGraphicsPanel);
		soundButton.onClick.AddListener(OpenSoundPanel);
		returnButton.onClick.AddListener(ReturnToMainMenu);
	}

	private void OnDestroy()
	{
		graphicsButton.onClick.RemoveListener(OpenGraphicsPanel);
		soundButton.onClick.RemoveListener(OpenSoundPanel);
		returnButton.onClick.RemoveListener(ReturnToMainMenu);
	}

	private void OpenGraphicsPanel()
	{
		graphicPanel.SetActive(true);
	}

	private void OpenSoundPanel()
	{
		soundPanel.SetActive(true);
	}

	private void ReturnToMainMenu()
	{
		gameObject.SetActive(false);
	}
}
