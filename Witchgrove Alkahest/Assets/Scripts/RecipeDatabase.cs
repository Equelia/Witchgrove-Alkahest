using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Recipe
{
	[Tooltip("Assign ingredient in right order")]
	public List<IngredientRequirement> ingredients;
	public ItemType result;
	public int resultCount;
}

[Serializable]
public class IngredientRequirement
{
	public ItemType type;
	public int count;
}

[CreateAssetMenu(fileName = "RecipeDatabase", menuName = "Crafting/RecipeDatabase")]
public class RecipeDatabase : ScriptableObject
{
	public List<Recipe> recipes;
}