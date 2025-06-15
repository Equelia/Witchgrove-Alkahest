using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Recipe
{
	[Tooltip("Assign ingredient in right order")]
	public List<IngredientRequirement> ingredients;
	public BaseItemData result;
	public int resultCount;
}

[Serializable]
public class IngredientRequirement
{
	public BaseItemData type;
	public int count;
}

[CreateAssetMenu(fileName = "RecipeDatabase", menuName = "Crafting/RecipeDatabase")]
public class RecipeDatabase : ScriptableObject
{
	public List<Recipe> recipes;
}