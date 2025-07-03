using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
	public static bool HasSeen(string id) => PlayerPrefs.GetInt("tutorial_" + id, 0) == 1;

	public static void MarkAsSeen(string id)
	{
		PlayerPrefs.SetInt("tutorial_" + id, 1);
	}
}