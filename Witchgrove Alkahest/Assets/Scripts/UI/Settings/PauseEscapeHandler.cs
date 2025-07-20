using System;
using UnityEngine;

public class PauseEscapeHandler : MonoBehaviour
{
	[SerializeField] private GameObject settingsPanel;
	[SerializeField] private GameObject graphicsPanel;
	[SerializeField] private GameObject soundPanel;

	private InventoryWindowManager windowManager;

	private void Start()
	{
		windowManager = FindObjectOfType<InventoryWindowManager>();
	}

	private void Update()
	{
		if (!windowManager.IsMenuOpen) return;

		if (Input.GetKeyDown(KeyCode.Escape))
		{
			if (graphicsPanel.activeSelf)
			{
				graphicsPanel.SetActive(false);
				return;
			}
			if (soundPanel.activeSelf)
			{
				soundPanel.SetActive(false);
				return;
			}

			// Затем закрыть настройки
			if (settingsPanel.activeSelf)
			{
				settingsPanel.SetActive(false);
				return;
			}

			// Затем закрыть главное меню
			if (windowManager.IsMenuOpen)
			{
				windowManager.CloseMainMenu();
			}
		}
	}
}