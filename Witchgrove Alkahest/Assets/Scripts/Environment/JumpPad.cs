using UnityEngine;

[RequireComponent(typeof(Collider))]
public class JumpPad : MonoBehaviour
{
	[Tooltip("Force of the jump pad (trajectory and speed)")]
	public Vector3 launchForce = new Vector3(0, 15f, 10f); 

	[Tooltip("Разрешить управление в воздухе во время полета?")]
	public bool allowAirControl = false;
	
	private int trajectoryPoints = 100;
	private float timeStep = 0.1f;
	private Color trajectoryColor = Color.cyan;

	private void OnTriggerEnter(Collider other)
	{
		FirstPersonController controller = FindFirstObjectByType<FirstPersonController>();
		if (controller != null)
		{
			controller.AddExternalForce(launchForce, allowAirControl);
		}
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = trajectoryColor;

		Vector3 startPos = transform.position + Vector3.up * 0.5f;
		Vector3 velocity = launchForce;
		Vector3 gravity = Physics.gravity;

		Vector3 previousPoint = startPos;
		for (int i = 1; i <= trajectoryPoints; i++)
		{
			float t = i * timeStep;
			Vector3 point = startPos + velocity * t + 0.5f * gravity * t * t;
			Gizmos.DrawLine(previousPoint, point);
			previousPoint = point;
		}
	}
}