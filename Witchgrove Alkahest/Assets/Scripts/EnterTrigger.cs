using System;
using UnityEngine;

public class EnterTrigger : MonoBehaviour
{
	[SerializeField] private CharacterController characterController;

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			characterController.skinWidth = 0.02f;
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			characterController.skinWidth = 0.0001f;
		}
	}
}
