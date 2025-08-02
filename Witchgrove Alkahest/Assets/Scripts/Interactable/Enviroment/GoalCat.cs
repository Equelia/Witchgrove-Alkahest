using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class GoalCat : InteractableItem
{
	[SerializeField] private Transform visualRoot; 
	[SerializeField] private float disappearDuration = 1.5f;
	[SerializeField] private ParticleSystem blinkEffect; 
	[SerializeField] private ParticleSystem leveltationEffect;
	[SerializeField] private Collider coll;
	[SerializeField] private DialogTutorialSystem dialogSystem;
	
	private Vector3 originalScale;
	private Quaternion originalRotation;

	private void Awake()
	{
		if (visualRoot != null)
		{
			originalScale = visualRoot.localScale;
			originalRotation = visualRoot.localRotation;
		}
	}

	public override void Interact()
	{
		var goalData = GoalController.Instance.CurrentStep.data;
		
		dialogSystem.ShowDialog(goalData.dialogStrings, () =>
		{
			PlayDisappearSequence().Forget(); 
		});
	}

	private async UniTaskVoid PlayDisappearSequence()
	{
		coll.enabled = false;
		leveltationEffect.gameObject.SetActive(false);

		if (blinkEffect != null)
			blinkEffect.Play();

		var twist = visualRoot.DOLocalRotate(new Vector3(0, 720, 0), disappearDuration, RotateMode.FastBeyond360);
		var shrink = visualRoot.DOScale(Vector3.zero, disappearDuration).SetEase(Ease.InBack);

		await UniTask.WhenAll(
			twist.ToUniTask(),
			shrink.ToUniTask()
		);
		
		GoalController.Instance.AdvanceGoal();

		gameObject.SetActive(false);
	}
	
	private async void OnEnable()
	{
		if (visualRoot != null)
		{
			visualRoot.localScale = originalScale;
			visualRoot.localRotation = originalRotation;
		}

		coll.enabled = true;
		leveltationEffect?.gameObject.SetActive(true);
		
		await UniTask.Delay(1000);
		SoundManager.Instance.PlaySoundOnceAtPositionUntilComplete("GoalCat", transform.position);
	}
}