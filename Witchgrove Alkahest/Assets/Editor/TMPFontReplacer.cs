using UnityEngine;
using UnityEditor;
using TMPro;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;

public class TMPFontReplacer : OdinEditorWindow
{
	[Title("Font Replacement Settings")]
	[LabelText("Target Prefab")]
	[Required]
	public GameObject prefab;

	[LabelText("New TMP Font Asset")]
	[Required]
	public TMP_FontAsset newFont;

	[Button("Replace Font in Prefab")]
	private void ReplaceFont()
	{
		if (prefab == null || newFont == null)
		{
			Debug.LogError("Please assign both the prefab and the font asset.");
			return;
		}

		string prefabPath = AssetDatabase.GetAssetPath(prefab);
		GameObject prefabInstance = PrefabUtility.LoadPrefabContents(prefabPath);
		int count = 0;

		foreach (var text in prefabInstance.GetComponentsInChildren<TextMeshProUGUI>(true))
		{
			Undo.RecordObject(text, "Replace TMP Font");
			text.font = newFont;
			EditorUtility.SetDirty(text);
			count++;
		}

		PrefabUtility.SaveAsPrefabAsset(prefabInstance, prefabPath);
		PrefabUtility.UnloadPrefabContents(prefabInstance);

		Debug.Log($"Replaced font in {count} TextMeshProUGUI components in prefab: {prefab.name}");
	}

	[MenuItem("Tools/Replace TMP Font in Prefab")]
	private static void OpenWindow()
	{
		GetWindow<TMPFontReplacer>("TMP Font Replacer").Show();
	}
}