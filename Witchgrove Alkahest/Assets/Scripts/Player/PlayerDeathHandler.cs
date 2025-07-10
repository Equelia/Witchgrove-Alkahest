using UnityEngine;

public class PlayerDeathHandler : MonoBehaviour
{
	public void Respawn()
	{
		PlayerInventorySystem.Instance.ClearAllSlots();
		SaveManager.Instance.SaveGame();
		RespawnManager.Instance.TriggerRespawn();
	}
}