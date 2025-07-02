using UnityEngine;

public class SpecificIngredientTutorial : InteractableItem
{
	[Header("Tutorial")]
	[SerializeField] private TutorialUIGroup tutorialUIGroup;
	
	public override void Interact()
	{
		tutorialUIGroup?.Show();
	}
}