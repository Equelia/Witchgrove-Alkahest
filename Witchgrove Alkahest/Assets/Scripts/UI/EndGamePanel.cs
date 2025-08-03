using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndGamePanel : MonoBehaviour
{
	[SerializeField] private Button returnButton;

	private void OnEnable()
	{
		returnButton.onClick.AddListener(ReturnToMainMenu);
	}
	
	private void OnDisable()
	{
		returnButton.onClick.RemoveListener(ReturnToMainMenu);
	}

	private void ReturnToMainMenu()
	{
		SceneManager.LoadScene("MainMenu");
	}
}
