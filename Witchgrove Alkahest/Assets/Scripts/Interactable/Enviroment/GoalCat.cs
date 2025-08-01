using System;
using Cysharp.Threading.Tasks;

public class GoalCat : InteractableItem
{
	public override void Interact()
	{
		GoalController.Instance.AdvanceGoal();
		gameObject.SetActive(false);
	}

	private async void OnEnable()
	{
		await UniTask.Delay(1000); 		
		SoundManager.Instance.PlaySoundOnceAtPositionUntilComplete("GoalCat", gameObject.transform.position);
	}
}