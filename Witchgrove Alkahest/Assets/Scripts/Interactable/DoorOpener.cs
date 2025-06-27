using DG.Tweening;
using UnityEngine;

public class DoorOpener : InteractableItem
{
	public float openAngle = 90f;
	public float duration = 1f;

	private bool isOpen = false;
	private Quaternion initialRotation;
	private Quaternion openRotation;

	void Start()
	{
		initialRotation = transform.localRotation;
		openRotation = initialRotation * Quaternion.Euler(0, 0, openAngle);
	}

	public override void Interact()
	{
		ToggleDoor();
	}

	public void ToggleDoor()
	{
		if (isOpen)
			transform.DOLocalRotateQuaternion(initialRotation, duration);
		else
			transform.DOLocalRotateQuaternion(openRotation, duration);

		isOpen = !isOpen;
	}
}