#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;
using System;

namespace cindesebos.EditorAutoSaver
{
    [InitializeOnLoad]
    public static class WelcomeWindowOnImport
    {
        private const string WelcomeShownKey = "Editor Auto Saver";

        static WelcomeWindowOnImport()
        {
            if (!EditorPrefs.GetBool(WelcomeShownKey, false))
            {
                EditorApplication.delayCall += () =>
                {
                    WelcomeWindow.ShowWindow();

                    EditorPrefs.SetBool(WelcomeShownKey, true);
                };
            }
        }
    }

    public class WelcomeWindow : EditorWindow
    {
        public static void ShowWindow()
        {
            var window = GetWindow<WelcomeWindow>("Tutorial");
            window.minSize = new Vector2(300, 150);
        }

        private void OnGUI()
        {
            GUILayout.Space(10);

            var centeredStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 15
            };

            GUILayout.Label("🚀Welcome!🚀", centeredStyle);

            GUILayout.Label("Customize in Tools → cindesebos → Editor Auto Saver", EditorStyles.miniBoldLabel);

            if (GUILayout.Button("Love you, bro🫡")) Close();
        }
    }
}

#endif
