using UnityEngine;
using UnityEngine.SceneManagement;

public static class PortalManager
{
	public static int NextPortalID = 0;
	
	public static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		SoundManager.Instance.StopMusic();
		SoundManager.Instance.PlayMusic(scene.name, true);
		Debug.Log("OnSceneLoaded" + scene.name);
	}
}