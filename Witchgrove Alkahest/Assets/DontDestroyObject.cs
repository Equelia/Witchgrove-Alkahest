using UnityEngine;

public class DontDestroyObject : MonoBehaviour
{
	private static DontDestroyObject instance;


	private void Awake()
	{
		if (instance != null && instance != this)
		{
			Destroy(gameObject); // Удаляем дубликат
			return;
		}

		instance = this;
		DontDestroyOnLoad(gameObject); // Сохраняет весь иерархический объект
	}
}