using UnityEngine;

public class StatsDisplay : MonoBehaviour
{
	[Tooltip("Трансформ игрока, чья скорость измеряется")]
	public Transform player;
	
	private float smoothingFactor = 0.005f;

	private Vector3 _lastPosition;
	private float _smoothSpeed;
	private float _smoothFps;

	void Start()
	{
		if (player == null)
			player = transform;

		_lastPosition = player.position;
		_smoothSpeed = 0f;
		_smoothFps = 0f;
	}

	void Update()
	{
		Vector3 delta = player.position - _lastPosition;
		float rawSpeed = delta.magnitude / Time.deltaTime; // м/с
		float rawFps   = 1f / Time.deltaTime;

		_lastPosition = player.position;

		_smoothSpeed = Mathf.Lerp(_smoothSpeed, rawSpeed, smoothingFactor);
		_smoothFps   = Mathf.Lerp(_smoothFps,   rawFps,   smoothingFactor);
	}

	void OnGUI()
	{
		GUIStyle style = new GUIStyle(GUI.skin.label)
		{
			fontSize = 20,
			normal = { textColor = Color.white }
		};

		GUI.Label(new Rect(10, 10, 300, 30), $"Скорость: {_smoothSpeed:F2} м/с", style);
		GUI.Label(new Rect(10, 40, 300, 30), $"FPS: {_smoothFps:F0}", style);
	}
}