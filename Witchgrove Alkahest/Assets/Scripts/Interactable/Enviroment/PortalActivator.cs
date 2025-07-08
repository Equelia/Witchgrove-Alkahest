using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

public class PortalActivator : InteractableItem
{
	[SerializeField] private BaseItemData itemForPortal;
	[SerializeField] private GameObject portalGameObject;
	[SerializeField] private float animationDuration = 1.0f;

	private Dictionary<Transform, Vector3> originalScales = new Dictionary<Transform, Vector3>();

	private void Awake()
	{
		if (portalGameObject != null)
		{
			// Сохраняем исходные масштабы всех потомков (включая самого портала)
			foreach (Transform t in portalGameObject.GetComponentsInChildren<Transform>(true))
			{
				originalScales[t] = t.localScale;
				t.localScale = Vector3.zero;
			}

			portalGameObject.SetActive(false); // скрываем изначально
		}
	}

	public override void Interact()
	{
		if (portalGameObject == null || itemForPortal == null)
			return;

		if (PlayerInventorySystem.Instance.TryConsumeItem(itemForPortal, 1))
		{
			portalGameObject.SetActive(true);

			// Применяем анимацию ко всем сохранённым трансформам
			foreach (var kvp in originalScales)
			{
				kvp.Key.localScale = Vector3.zero;
				kvp.Key.DOScale(kvp.Value, animationDuration)
					.SetEase(Ease.OutBack);
			}
		}
	}
}