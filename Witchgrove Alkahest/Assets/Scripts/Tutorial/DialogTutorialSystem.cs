using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogTutorialSystem : MonoBehaviour
{
	[SerializeField] private GameObject dialogPanel;
	[SerializeField] private TMP_Text dialogText;
	[SerializeField] private Button nextButton;
	[SerializeField] private Button closeButton;

	private string[] currentDialog;
	private int dialogIndex;
	private System.Action onDialogComplete;

	private void Awake()
	{
		dialogPanel.SetActive(false);
		nextButton.onClick.AddListener(OnNextClicked);
		closeButton.onClick.AddListener(OnCloseClicked);
	}

	public void ShowDialog(string[] dialogLines, System.Action onComplete = null)
	{
		currentDialog = dialogLines;
		dialogIndex = 0;
		onDialogComplete = onComplete;

		dialogPanel.SetActive(true);
		DisplayCurrentLine();
	}

	private void DisplayCurrentLine()
	{
		if (dialogIndex < currentDialog.Length)
		{
			dialogText.text = currentDialog[dialogIndex];

			bool isLastLine = dialogIndex == currentDialog.Length - 1;
			nextButton.gameObject.SetActive(!isLastLine);
			closeButton.gameObject.SetActive(isLastLine);
		}
		else
		{
			EndDialog();
		}
	}

	private void OnNextClicked()
	{
		dialogIndex++;
		DisplayCurrentLine();
	}

	private void OnCloseClicked()
	{
		EndDialog();
	}

	private void EndDialog()
	{
		dialogPanel.SetActive(false);
		onDialogComplete?.Invoke();
	}
}