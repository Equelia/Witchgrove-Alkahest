using System;
using UnityEngine;

public class EndGamePortal : MonoBehaviour
{
	[SerializeField] private GameObject endGamePanel;
	[SerializeField] private GameObject playerController;

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			endGamePanel.SetActive(true);
			playerController.SetActive(false);
			PlayerInventorySystem.Instance.gameObject.SetActive(false);
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
		}
	}
}