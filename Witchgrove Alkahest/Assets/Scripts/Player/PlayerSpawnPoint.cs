using UnityEngine;

public class PlayerSpawnPoint : MonoBehaviour
{
	private void Start()
	{
		if (RespawnManager.Instance.shouldRespawn)
		{
			GameObject player = GameObject.FindGameObjectWithTag("Player");
			if (player != null)
			{
				player.transform.position = transform.position;
				RespawnManager.Instance.shouldRespawn = false;
			}
		}
	}
}