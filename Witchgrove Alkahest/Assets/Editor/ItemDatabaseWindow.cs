#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;

public enum ItemFilterType
{
	All,
	Ingredients,
	Potions,
	TraderItems
}

public class ItemDatabaseWindow : OdinEditorWindow
{
	[MenuItem("Tools/Item Database Window")]
	private static void OpenWindow() => GetWindow<ItemDatabaseWindow>().Show();

	private string searchQuery = "";
	private Vector2 databaseScroll;
	private ItemDatabase itemDatabase;
	private List<BaseItemData> filteredItems;

	private const int itemSize = 60;
	private const int spacing = 6;
	private const int horizontalMargin = 20;

	private string currentTooltip = "";
	private Rect currentTooltipRect = Rect.zero;

	private ItemFilterType selectedFilter = ItemFilterType.All;

	protected override void OnEnable()
	{
		base.OnEnable();
		itemDatabase = ItemDatabase.Instance;
		UpdateFilteredList();
	}

	[OnInspectorGUI]
	private void Draw()
	{
		if (!Application.isPlaying || SaveManager.Instance == null || PlayerInventorySystem.Instance == null)
		{
			SirenixEditorGUI.ErrorMessageBox("Play Mode only — SaveManager or PlayerInventorySystem not initialized.");
			return;
		}

		GUILayout.Space(10);
		SirenixEditorGUI.Title("Player Inventory", null, TextAlignment.Left, true);
		DrawInventory();

		GUILayout.Space(20);
		SirenixEditorGUI.Title("Item Database", null, TextAlignment.Left, true);
		DrawSearchField();
		UpdateFilteredList();

		GUILayout.Space(10);
		DrawItemDatabase();

		if (!string.IsNullOrEmpty(currentTooltip) && Event.current.type == EventType.Repaint)
		{
			Vector2 mouse = Event.current.mousePosition;
			Vector2 size = SirenixGUIStyles.MultiLineLabel.CalcSize(new GUIContent(currentTooltip));
			Rect rect = new Rect(mouse.x + 15, mouse.y + 15, size.x + 10, size.y + 8);
			GUI.Box(rect, currentTooltip, SirenixGUIStyles.MultiLineLabel);
		}

		if (Event.current.type == EventType.MouseMove)
		{
			if (!currentTooltipRect.Contains(Event.current.mousePosition))
			{
				currentTooltip = "";
				currentTooltipRect = Rect.zero;
				Repaint();
			}
		}
	}

	private void DrawSearchField()
	{
		GUILayout.BeginHorizontal();
		GUILayout.Space(horizontalMargin);
		GUILayout.Label("Search:", GUILayout.Width(50));
		searchQuery = GUILayout.TextField(searchQuery);
		GUILayout.Space(horizontalMargin);
		GUILayout.EndHorizontal();

		GUILayout.Space(4);

		GUILayout.BeginHorizontal();
		GUILayout.Space(horizontalMargin);
		GUILayout.Label("Filter:", GUILayout.Width(50));
		selectedFilter = (ItemFilterType)EditorGUILayout.EnumPopup(selectedFilter);
		GUILayout.Space(horizontalMargin);
		GUILayout.EndHorizontal();
	}


	private void UpdateFilteredList()
	{
		if (itemDatabase == null) return;

		IEnumerable<BaseItemData> result = Enumerable.Empty<BaseItemData>();

		switch (selectedFilter)
		{
			case ItemFilterType.All:
				result = itemDatabase.ingredients.Cast<BaseItemData>()
					.Concat(itemDatabase.potions)
					.Concat(itemDatabase.traderItems.Cast<BaseItemData>());
				break;
			case ItemFilterType.Ingredients:
				result = itemDatabase.ingredients.Cast<BaseItemData>();
				break;
			case ItemFilterType.Potions:
				result = itemDatabase.potions;
				break;
			case ItemFilterType.TraderItems:
				result = itemDatabase.traderItems.Cast<BaseItemData>();
				break;
		}

		filteredItems = result
			.Where(i => i != null && i.displayName.ToLower().Contains(searchQuery.ToLower()))
			.ToList();
	}

	private void DrawInventory()
	{
		var inventorySystem = PlayerInventorySystem.Instance;
		var playerData = SaveManager.Instance.playerData;
		int slotsToShow = playerData.InventoryLevel switch { 1 => 4, 2 => 8, 3 => 12, _ => 4 };

		var slots = inventorySystem.GetAllSlots().Take(slotsToShow).ToList();
		int columns = Mathf.Min(slotsToShow, 4);

		float contentWidth = columns * (itemSize + spacing);
		float totalWidth = EditorGUIUtility.currentViewWidth;
		float margin = Mathf.Max((totalWidth - contentWidth) / 2f, horizontalMargin);

		for (int i = 0; i < slots.Count; i += columns)
		{
			GUILayout.BeginHorizontal();
			GUILayout.Space(margin);
			for (int j = 0; j < columns; j++)
			{
				int index = i + j;
				if (index >= slots.Count) break;

				var slot = slots[index];
				GUILayout.BeginVertical(GUILayout.Width(itemSize));

				if (slot.Count != 0 && slot.ItemData != null && slot.ItemData.icon != null)
				{
					GUIContent content = new GUIContent(slot.ItemData.icon.texture, slot.ItemData.displayName);
					GUIStyle style = new GUIStyle(GUI.skin.box);
					style.padding = new RectOffset(2, 2, 2, 2);
					style.margin = new RectOffset(2, 2, 2, 2);

					Rect rect = GUILayoutUtility.GetRect(content, GUI.skin.box, GUILayout.Width(itemSize),
						GUILayout.Height(itemSize));
					GUI.Box(rect, content, style);

					if (rect.Contains(Event.current.mousePosition))
					{
						currentTooltip = content.tooltip;
						currentTooltipRect = rect;
					}

					GUILayout.Label($"x{slot.Count}", SirenixGUIStyles.CenteredGreyMiniLabel);
				}
				else
				{
					GUIStyle emptyStyle = new GUIStyle(GUI.skin.box);
					emptyStyle.normal.background = Texture2D.grayTexture;
					GUILayout.Box("", emptyStyle, GUILayout.Width(itemSize), GUILayout.Height(itemSize));
					GUILayout.Label("", GUILayout.Width(itemSize));
				}

				GUILayout.EndVertical();
			}

			GUILayout.Space(margin);
			GUILayout.EndHorizontal();
		}
	}

	private void DrawItemDatabase()
	{
		if (filteredItems == null) return;

		float availableWidth = EditorGUIUtility.currentViewWidth - 2 * horizontalMargin;
		int columns = Mathf.Max(1, Mathf.FloorToInt(availableWidth / (itemSize + spacing)));
		float contentWidth = columns * (itemSize + spacing);
		float contentHeight = Mathf.Ceil(filteredItems.Count / (float)columns) * (itemSize + spacing);

		databaseScroll = GUILayout.BeginScrollView(databaseScroll, GUILayout.Height(320));

		for (int i = 0; i < filteredItems.Count; i += columns)
		{
			GUILayout.BeginHorizontal();
			GUILayout.Space(horizontalMargin);

			for (int j = 0; j < columns; j++)
			{
				int index = i + j;
				if (index >= filteredItems.Count) break;

				var item = filteredItems[index];
				if (item == null || item.icon == null) continue;

				GUIContent content = new GUIContent(item.icon.texture, item.displayName);
				GUIStyle style = new GUIStyle(GUI.skin.button)
				{
					margin = new RectOffset(1, 1, 1, 1),
					padding = new RectOffset(2, 2, 2, 2)
				};

				Rect rect = GUILayoutUtility.GetRect(content, style, GUILayout.Width(itemSize),
					GUILayout.Height(itemSize));

				if (GUI.Button(rect, content, style))
				{
					PlayerInventorySystem.Instance.TryAddOneItem(item);
					Debug.Log($"[ItemDatabase] Added '{item.displayName}' to inventory.");
				}


				Vector2 localMouse = Event.current.mousePosition;
				float itemTop = rect.y;
				float itemBottom = rect.y + rect.height;
				float scrollTop = databaseScroll.y;
				float scrollBottom = databaseScroll.y + 320;

				if ((Event.current.type == EventType.Repaint || Event.current.type == EventType.MouseMove)
				    && itemBottom >= scrollTop && itemTop <= scrollBottom
				    && rect.Contains(localMouse))
				{
					currentTooltip = content.tooltip;
					currentTooltipRect = rect;
				}
			}

			GUILayout.Space(horizontalMargin);
			GUILayout.EndHorizontal();
		}

		GUILayout.EndScrollView();
	}
}
#endif