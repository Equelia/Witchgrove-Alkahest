using UnityEngine;

public class CellSoundHandler : MonoBehaviour
{
	private const string DefaultAddSound = "CellPop";
	private string addSoundName;
	private Cell _cell;
	
	public void Initialize(Cell cell, string soundName = null)
	{
		if (_cell != null)
			_cell.OnItemAddedExternally -= PlaySound;

		_cell = cell;
		addSoundName = soundName;
		_cell.OnItemAddedExternally += PlaySound;
	}


	private void PlaySound(Cell cell, int amount)
	{
		if (amount <= 0) return;

		string sound = string.IsNullOrEmpty(addSoundName) ? DefaultAddSound : addSoundName;
		SoundManager.Instance.PlaySound(sound);
		
		Debug.Log(sound);
	}

	private void OnDestroy()
	{
		if (_cell != null)
			_cell.OnItemAddedExternally -= PlaySound;
	}
}