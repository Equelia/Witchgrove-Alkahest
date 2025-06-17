#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class ItemCreatorWindow : OdinEditorWindow
{
    private const string IngredientPath = "Assets/Resources/Items/Ingredients";
    private const string PotionPath = "Assets/Resources/Items/Potions";
    private const string DatabasePath = "Assets/Resources/ItemDatabase.asset";
    private const string QuestDatabasePath = "Assets/Resources/QuestDatabase.asset";

    [MenuItem("Tools/Item & Recipe Creator")]
    private static void OpenWindow() => GetWindow<ItemCreatorWindow>("Item & Recipe Creator");

    public enum ItemTypeToCreate { Ingredient, Potion }

    // === Создание предмета ===
    [TitleGroup("Создание предмета")]
    [BoxGroup("Создание предмета/Настройки", showLabel: false)]
    [LabelText("Тип предмета")]
    public ItemTypeToCreate itemType = ItemTypeToCreate.Ingredient;

    [BoxGroup("Создание предмета/Настройки")]
    [PropertySpace(5)]
    [LabelText("ID (уникальное имя)")]
    [Required]
    public string itemId = "new_item";

    [BoxGroup("Создание предмета/Настройки")]
    [PropertySpace]
    [LabelText("Отображаемое имя")]
    public string displayName = "Новый предмет";

    [BoxGroup("Создание предмета/Настройки")]
    [PropertySpace]
    [LabelText("Иконка")]
    public Sprite icon;

    [BoxGroup("Создание предмета/Настройки")]
    [PropertySpace]
    [LabelText("Макс. размер стека")]
    [MinValue(1)]
    public int maxStack = 5;

    [BoxGroup("Создание предмета/Настройки")]
    [PropertySpace(10)]
    [Button("Создать предмет", ButtonSizes.Large), GUIColor(0.3f, 1f, 0.3f)]
    private void CreateItem()
    {
        if (string.IsNullOrWhiteSpace(itemId))
        {
            Debug.LogError("ID не должен быть пустым.");
            return;
        }

        string folder = itemType == ItemTypeToCreate.Ingredient ? IngredientPath : PotionPath;
        if (!Directory.Exists(folder))
            Directory.CreateDirectory(folder);

        string assetPath = $"{folder}/{itemId}.asset";
        if (File.Exists(assetPath))
        {
            Debug.LogError("❌ Предмет с таким ID уже существует!");
            return;
        }

        BaseItemData item = itemType switch
        {
            ItemTypeToCreate.Ingredient => ScriptableObject.CreateInstance<IngredientData>(),
            ItemTypeToCreate.Potion => ScriptableObject.CreateInstance<PotionData>(),
            _ => null
        };

        if (item == null) return;

        item.id = itemId.ToLower();
        item.displayName = displayName;
        item.icon = icon;
        item.maxStack = maxStack;

        AssetDatabase.CreateAsset(item, assetPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"✅ Предмет создан: {assetPath}");

        AddItemToDatabase(item);
    }

    private void AddItemToDatabase(BaseItemData item)
    {
        var db = AssetDatabase.LoadAssetAtPath<ItemDatabase>(DatabasePath);

        if (db == null)
        {
            db = ScriptableObject.CreateInstance<ItemDatabase>();
            db.ingredients = new List<IngredientData>();
            db.potions = new List<PotionData>();
            AssetDatabase.CreateAsset(db, DatabasePath);
            Debug.LogWarning("Создан новый ItemDatabase.asset в Resources/");
        }

        if (db.ingredients == null) db.ingredients = new List<IngredientData>();
        if (db.potions == null) db.potions = new List<PotionData>();

        if (item is IngredientData ing)
        {
            if (!db.ingredients.Contains(ing))
                db.ingredients.Add(ing);
        }
        else if (item is PotionData pot)
        {
            if (!db.potions.Contains(pot))
                db.potions.Add(pot);
        }

        EditorUtility.SetDirty(db);
        AssetDatabase.SaveAssets();
        Debug.Log("✅ Предмет добавлен в ItemDatabase.");
    }

    // === Создание рецепта ===
    [PropertySpace(20)]
    [TitleGroup("Создание рецепта")]
    [BoxGroup("Создание рецепта/Настройки", showLabel: false)]
    [Required]
    [LabelText("База рецептов")]
    public RecipeDatabase recipeDatabase;

    [BoxGroup("Создание рецепта/Настройки")]
    [PropertySpace(5)]
    [LabelText("Ингредиенты")]
    [TableList(ShowIndexLabels = true)]
    public List<IngredientRequirementEditor> ingredientInputs = new();

    [BoxGroup("Создание рецепта/Настройки")]
    [PropertySpace]
    [LabelText("Результат крафта")]
    [ValueDropdown(nameof(GetAllItemsDropdown))]
    public BaseItemData resultItem;

    [BoxGroup("Создание рецепта/Настройки")]
    [PropertySpace]
    [LabelText("Кол-во результата")]
    [MinValue(1)]
    public int resultCount = 1;

    [BoxGroup("Создание рецепта/Настройки")]
    [PropertySpace(10)]
    [Button("Добавить рецепт", ButtonSizes.Large), GUIColor(0.2f, 0.7f, 1f)]
    private void AddRecipe()
    {
        if (recipeDatabase == null)
        {
            Debug.LogWarning("Укажи RecipeDatabase.");
            return;
        }

        if (resultItem == null || ingredientInputs.Count == 0)
        {
            Debug.LogWarning("Заполни результат и хотя бы один ингредиент.");
            return;
        }

        var newRecipe = new Recipe
        {
            result = resultItem,
            resultCount = resultCount,
            ingredients = ingredientInputs.Select(i => new IngredientRequirement
            {
                type = i.ingredient,
                count = i.count
            }).ToList()
        };

        recipeDatabase.recipes.Add(newRecipe);
        EditorUtility.SetDirty(recipeDatabase);
        AssetDatabase.SaveAssets();

        Debug.Log("✅ Рецепт добавлен!");

        ingredientInputs.Clear();
        resultItem = null;
        resultCount = 1;
    }

    // === Создание квеста ===
    [PropertySpace(20)]
    [TitleGroup("Создание квеста")]
    [BoxGroup("Создание квеста/Настройки", showLabel: false)]
    [Required]
    [LabelText("База квестов")]
    public QuestDatabase questDatabase;

    [BoxGroup("Создание квеста/Настройки")]
    [PropertySpace(5)]
    [LabelText("ID квеста")]
    [Required]
    public string questId = "quest_id";

    [BoxGroup("Создание квеста/Настройки")]
    [LabelText("Описание")]
    [TextArea(3, 5)]
    public string questDescription = "Описание квеста";

    [BoxGroup("Создание квеста/Настройки")]
    [LabelText("Требуемый предмет")]
    [ValueDropdown(nameof(GetAllItemsDropdown))]
    public BaseItemData requiredItem;

    [BoxGroup("Создание квеста/Настройки")]
    [LabelText("Кол-во предметов")]
    [MinValue(1)]
    public int requiredCount = 1;

    [BoxGroup("Создание квеста/Настройки")]
    [PropertySpace(10)]
    [Button("Добавить квест", ButtonSizes.Large), GUIColor(1f, 0.7f, 0.2f)]
    private void AddQuest()
    {
        if (questDatabase == null)
        {
            Debug.LogWarning("Укажи QuestDatabase.");
            return;
        }

        var newQuest = new QuestData()
        {
            questId = questId,
            description = questDescription,
            requiredItem = requiredItem,
            requiredCount = requiredCount
        };

        questDatabase.quests ??= new List<QuestData>();
        questDatabase.quests.Add(newQuest);

        EditorUtility.SetDirty(questDatabase);
        AssetDatabase.SaveAssets();

        Debug.Log("✅ Квест добавлен в QuestDatabase!");

        // Сброс
        questId = "quest_id";
        questDescription = "";
        requiredItem = null;
        requiredCount = 1;
    }

    private IEnumerable<BaseItemData> GetAllItemsDropdown()
    {
        string[] guids = AssetDatabase.FindAssets("t:BaseItemData");
        foreach (var guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            var asset = AssetDatabase.LoadAssetAtPath<BaseItemData>(path);
            if (asset != null)
                yield return asset;
        }
    }

    [System.Serializable]
    public class IngredientRequirementEditor
    {
        [ValueDropdown(nameof(GetAllItemsDropdown))]
        public IngredientData ingredient;

        [MinValue(1)]
        public int count = 1;

        private static IEnumerable<IngredientData> GetAllItemsDropdown()
        {
            string[] guids = AssetDatabase.FindAssets("t:IngredientData");
            foreach (var guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                var asset = AssetDatabase.LoadAssetAtPath<IngredientData>(path);
                if (asset != null)
                    yield return asset;
            }
        }
    }
}
#endif
