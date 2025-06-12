#if UNITY_EDITOR

using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using System;

namespace cindesebos.EditorAutoSaver
{
    [InitializeOnLoad]
    public static class EditorAutoSaver
    {
        public static double SaveInterval
        {
            get => EditorPrefs.GetFloat("EditorAutoSaver_SaveInterval", 60f);
            set => EditorPrefs.SetFloat("EditorAutoSaver_SaveInterval", (float)value);
        }

        public static bool AutoSaveEnabled
        {
            get => EditorPrefs.GetBool("EditorAutoSaver_AutoSaveEnabled", true);
            set => EditorPrefs.SetBool("EditorAutoSaver_AutoSaveEnabled", value);
        }
        
        public static bool ShowMessageInConsole
        {
            get => EditorPrefs.GetBool("EditorAutoSaver_ShowMessageInConsole", true);
            set => EditorPrefs.SetBool("EditorAutoSaver_ShowMessageInConsole", value);
        }

        public static bool ShowMessagePanel
        {
            get => EditorPrefs.GetBool("EditorAutoSaver_ShowMessagePanel", true);
            set => EditorPrefs.SetBool("EditorAutoSaver_ShowMessagePanel", value);
        }

        private static double LastSavedTime;

        static EditorAutoSaver()
        {
            LastSavedTime = EditorApplication.timeSinceStartup;

            EditorApplication.update += OnEditorUpdate;
        }

        private static void OnEditorUpdate()
        {
            if (!AutoSaveEnabled) return;
            
            if (!EditorApplication.isPlaying && EditorApplication.timeSinceStartup - LastSavedTime >= SaveInterval)
            {
                SaveProject();

                LastSavedTime = EditorApplication.timeSinceStartup;
            }
        }

        private static void SaveProject()
        {
            EditorSceneManager.SaveOpenScenes();

            AssetDatabase.SaveAssets();

            string time = DateTime.Now.ToString("HH:mm:ss");

            string message = $"Scene Was Saved At {time}";

            if (ShowMessageInConsole) Debug.Log($"<color=black>[EditorAutoSaver]</color> <color=white>Scene Was Saved At</color> <color=red>{time}</color>");
            
            if (ShowMessagePanel) EditorToastPanel.Show(message);
        }
    }
}

#endif