using UnityEngine;

public class GoalCatFollower : MonoBehaviour
{
	[SerializeField] private float maxDistance = 8f;     
	[SerializeField] private float followDistance = 5f;  
	[SerializeField] private Transform playerTransform;
	[SerializeField] private FloatingPlatform floatingPlatform;

	private void Update()
	{
		if (playerTransform == null) return;

		LookAtPlayer();

		float distance = Vector3.Distance(transform.position, playerTransform.position);

		if (distance > maxDistance)
		{
			TeleportBehindPlayer();
		}
	}

	private void TeleportBehindPlayer()
	{
		Vector3 offset = -playerTransform.forward * followDistance;
		offset.y = 0f; 

		Vector3 newPosition = playerTransform.position + offset;
		floatingPlatform.ForceSetPosition(newPosition);
		transform.position = newPosition;
		SoundManager.Instance.PlaySoundOnceUntilComplete("GoalCat");
	}

	private void LookAtPlayer()
	{
		Vector3 targetPos = playerTransform.position;
		targetPos.y = transform.position.y; 

		transform.LookAt(targetPos);

		transform.Rotate(0, 90f, 0);
	}
}