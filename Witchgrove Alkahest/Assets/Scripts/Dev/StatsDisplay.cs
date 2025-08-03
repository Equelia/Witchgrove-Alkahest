using TMPro;
using UnityEngine;

public class StatsDisplay : MonoBehaviour
{
	[SerializeField] private TextMeshProUGUI fpsText;

	private float deltaTime;

	private void Update()
	{
		deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f; 
		float fps = 1f / deltaTime;
		fpsText.text = $"FPS: {Mathf.CeilToInt(fps)}";
	}
}