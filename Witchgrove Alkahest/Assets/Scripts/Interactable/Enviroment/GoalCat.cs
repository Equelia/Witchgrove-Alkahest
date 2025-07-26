using System;

public class GoalCat : InteractableItem
{
	public override void Interact()
	{
		GoalController.Instance.AdvanceGoal();
		gameObject.SetActive(false);
	}

	private void OnEnable()
	{
		SoundManager.Instance.PlaySound("GoalCat");
	}
}