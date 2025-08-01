using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

[System.Serializable]
public class DroppableItem
{
	public GameObject itemPrefab;
	public int amount;
}

public class MemeObject : InteractableItem
{
	[Header("Required item to exchange for gold")]
	[SerializeField] private BaseItemData requiredItem;
	[SerializeField] private string soundName;
	[SerializeField] private int goldAmount;
	[SerializeField] private Transform dropSpawnPoint;
	[SerializeField] private Animator animator;
	
	[Header("One-time drop on first interaction")]
	[SerializeField] private DroppableItem[] itemsToDropOnFirstInteraction;

	private bool hasDroppedItems;
	
	private string DropKey => $"DropFlag_{soundName}";

	private void Start()
	{
		hasDroppedItems = PlayerPrefs.GetInt(DropKey, 0) == 1;
	}
	
	public override void Interact()
	{
		AnimateObject().Forget();
		
		if (PlayerInventorySystem.Instance.TryConsumeItem(requiredItem, 1))
		{
			PlayerInventorySystem.Instance.playerData.GoldAmount += goldAmount;
			SoundManager.Instance.PlaySound("GoldChange");
		}

		if (!hasDroppedItems)
		{
			DropItems().Forget();
			SoundManager.Instance.PlaySound("CellPop");
			hasDroppedItems = true;
			PlayerPrefs.SetInt(DropKey, 1);
			PlayerPrefs.Save();
		}
	}

	private async UniTask AnimateObject()
	{
		AudioClip clip = SoundManager.Instance.PlaySoundOnceAtPositionUntilComplete(soundName, gameObject.transform.position);
		
		if (clip != null && animator != null)
		{
			animator.Play("Interact");
			await UniTask.Delay(System.TimeSpan.FromSeconds(clip.length));
			animator.Play("Idle");
		}
	}

	private async UniTaskVoid DropItems()
	{
		foreach (var drop in itemsToDropOnFirstInteraction)
		{
			for (int i = 0; i < drop.amount; i++)
			{
				GameObject itemGO = Instantiate(drop.itemPrefab, dropSpawnPoint.position, Quaternion.identity);

				Vector3 spreadDir = (dropSpawnPoint.forward + Random.insideUnitSphere * 0.8f).normalized;
				float distance = Random.Range(1.2f, 2.2f);
				float fixedY = 0f;
				Vector3 targetPos = dropSpawnPoint.position + spreadDir * distance;
				targetPos.y = dropSpawnPoint.position.y + fixedY;


				itemGO.transform.DOJump(targetPos, 1f, 1, 0.6f).SetEase(Ease.OutQuad);
				itemGO.transform.DORotate(new Vector3(0, 360f, 0), 1f, RotateMode.FastBeyond360)
					.SetEase(Ease.Linear)
					.SetLoops(1, LoopType.Restart);

				await UniTask.Delay(200); 
			}
		}
	}

}