using UnityEngine.SceneManagement;

public static class PortalManager
{
	public static int NextPortalID = 0;
	
	public static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		SceneManager.sceneLoaded -= OnSceneLoaded;
	}
}