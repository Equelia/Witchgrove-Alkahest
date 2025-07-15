using System;
using UnityEngine;
using UnityEngine.UI;

public class WaterKillZone : MonoBehaviour
{
	[Header("Tutorial")] [SerializeField] private UIWindowGroup deathScreen;

	[SerializeField] private Button closeButton;
	
	private PlayerDeathHandler playerDeathHandler;

	private void OnEnable()
	{
		closeButton.onClick.AddListener(Die);
	}

	private void OnDisable()
	{
		closeButton.onClick.RemoveListener(Die);
	}

	private void Die()
	{
		if (playerDeathHandler != null)
			playerDeathHandler.Respawn();
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			playerDeathHandler = other.GetComponent<PlayerDeathHandler>();
			SoundManager.Instance.PlaySound("WaterDeathSound");
			deathScreen?.Show();
		}
	}
}