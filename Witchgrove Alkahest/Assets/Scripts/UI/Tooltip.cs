using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Simple singleton tooltip for showing item names on hover.
/// </summary>
public class Tooltip : MonoBehaviour
{
	public static Tooltip Instance { get; private set; }

	[Tooltip("UI Text component for tooltip")]
	[SerializeField] private TMP_Text tooltipText;

	[Tooltip("Background RectTransform to auto-size")]
	[SerializeField] private RectTransform background;

	private void Awake()
	{
		Instance = this;
		Hide();
	}

	/// <summary>
	/// Show the tooltip with given text at the given screen position.
	/// </summary>
	public void Show(string text, Vector2 screenPosition)
	{
		gameObject.SetActive(true);
		
		string formatted = System.Text.RegularExpressions.Regex.Replace(
			text, @"(?<!^)([А-Я])", " $1"
		);
		tooltipText.text = formatted;

		// Resize background to fit text plus padding
		Vector2 size = tooltipText.GetPreferredValues(formatted);
		background.sizeDelta = size + new Vector2(12, 12);

		// Position at cursor
		(transform as RectTransform).position = screenPosition;
	}
	
	public void Hide()
	{
		gameObject.SetActive(false);
	}
}