using UnityEngine;
using DG.Tweening;

public class FloatingPlatform : MonoBehaviour
{
	[SerializeField] private float floatDistance = 0.5f;
	[SerializeField] private float duration = 2f;

	private Rigidbody rb;
	private Vector3 startPos;
	private float timerOffset;

	private void Start()
	{
		rb = GetComponent<Rigidbody>();
		rb.isKinematic = true; // Движение платформы вручную, не под действием физики

		startPos = transform.position;
		timerOffset = Random.Range(0f, Mathf.PI * 2); // для несинхронности
	}

	private void FixedUpdate()
	{
		float newY = startPos.y + Mathf.Sin((Time.time + timerOffset) * (2 * Mathf.PI / duration)) * floatDistance;
		Vector3 newPos = new Vector3(startPos.x, newY, startPos.z);
		rb.MovePosition(newPos);
	}
	
	public void ForceSetPosition(Vector3 newPosition)
	{
		rb.MovePosition(newPosition);
		startPos = newPosition;
	}

}