using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
	[SerializeField] private GameObject[] tutorialPanels;
	
	public static bool HasSeen(string id) => PlayerPrefs.GetInt("tutorial_" + id, 0) == 1;

	public static void MarkAsSeen(string id)
	{
		PlayerPrefs.SetInt("tutorial_" + id, 1);
	}

	public bool IsTutorialActive()
	{
		foreach (var tutorial in tutorialPanels)
		{
			if (tutorial.activeSelf)
				return true;
		}
		return false;
	}
}