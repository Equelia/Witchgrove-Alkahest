using UnityEngine;
using UnityEngine.SceneManagement;

public class RespawnManager : MonoBehaviour
{
	public static RespawnManager Instance { get; private set; }

	[HideInInspector] public string respawnSceneName = "MeadowLvl";
	[HideInInspector] public bool shouldRespawn = false;

	private void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Destroy(gameObject);
			return;
		}
		Instance = this;
		DontDestroyOnLoad(gameObject);
	}

	public void TriggerRespawn()
	{
		shouldRespawn = true;
		SceneManager.LoadScene(respawnSceneName);
	}
}