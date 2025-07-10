using UnityEngine;

public class SpecificIngredientTutorial : InteractableItem
{
	[Header("Tutorial")]
	[SerializeField] private UIWindowGroup uiWindowGroup;
	
	public override void Interact()
	{
		uiWindowGroup?.Show();
	}
}