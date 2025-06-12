#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;

namespace cindesebos.EditorAutoSaver
{
    public class EditorAutoSaverWindow : EditorWindow
    {
        private const float MaxMultiplierIntervalTime = 10;
        private const float MinTime = 5f;

        private float _saveInterval;
        private float _previousSaveInterval;
        private bool _showInSeconds = false;
        private bool _autoSaveEnabled = true;
        private bool _showMessageInConsole = true;
        private bool _showMessagePanel = true;

        [MenuItem("Tools/cindesebos/Editor Auto Saver")]
        public static void ShowWindow()
        {
            var window = GetWindow<EditorAutoSaverWindow>("Editor Auto Saver");

            window.minSize = new Vector2(550, 250);
            window.maxSize = new Vector2(600, 300);

            window.Show();
        }

        private void OnEnable()
        {
            _saveInterval = (float)EditorAutoSaver.SaveInterval;

            _autoSaveEnabled = EditorAutoSaver.AutoSaveEnabled;
        }

        private void OnGUI()
        {
            GUILayout.Label("Editor AutoSaver Settings", EditorStyles.boldLabel);

            GUILayout.Space(10);

            ShowEnableToggleButton();

            GUILayout.Space(10);

            ShowCurrentTimeInterval();

            GUILayout.Space(15);

            ChooseTypeButtons();

            GUILayout.Space(25);

            ShowIntervalType();

            CalucateIntervalTime();

            GUILayout.Space(30);

            SaveButton();
        }

        private void ShowEnableToggleButton()
        {
            EditorGUILayout.BeginHorizontal();

            _autoSaveEnabled = GUILayout.Toggle(_autoSaveEnabled, _autoSaveEnabled ? "Auto Save Is Enable" : "Auto Save Is Disable", EditorStyles.toolbarButton);

            EditorAutoSaver.AutoSaveEnabled = _autoSaveEnabled;

            GUILayout.Space(10);

            _showMessageInConsole = GUILayout.Toggle(_showMessageInConsole, _showMessageInConsole
            ? "Show Message In Console Is Enable" : "Show Message In Console Is Disable", EditorStyles.toolbarButton);

            EditorAutoSaver.ShowMessageInConsole = _showMessageInConsole;

            GUILayout.Space(10);

            _showMessagePanel = GUILayout.Toggle(_showMessagePanel, _showMessagePanel
            ? "Show Message Panel Is Enable" : "Show Message Panel Is Disable", EditorStyles.toolbarButton);

            EditorAutoSaver.ShowMessagePanel = _showMessagePanel;

            EditorGUILayout.EndHorizontal();
        }

        private void ShowCurrentTimeInterval()
        {
            GUIStyle richTextLabel = new GUIStyle(GUI.skin.label) { richText = true };

            string displayText = $"Current Time Invertval Is <color=red>{_saveInterval:F0}</color> seconds ~ <color=red>{_saveInterval / 60f:F1}</color> minutes";

            GUILayout.Label(displayText, richTextLabel);
        }

        private void ChooseTypeButtons()
        {
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Toggle(_showInSeconds, "Seconds", EditorStyles.miniButtonLeft)) _showInSeconds = true;

            if (GUILayout.Toggle(!_showInSeconds, "Minutes", EditorStyles.miniButtonRight)) _showInSeconds = false;

            EditorGUILayout.EndHorizontal();
        }

        private void ShowIntervalType() => GUILayout.Label(_showInSeconds ? "Time Interval Between Saves (in Seconds):" : "Time Interval Between Saves (in Minutes):");

        private void CalucateIntervalTime()
        {
            float maxIntervalTime = 60f * MaxMultiplierIntervalTime;

            float displayValue = _showInSeconds ? _saveInterval : _saveInterval / 60f;

            float newValue = EditorGUILayout.Slider(displayValue, MinTime / (_showInSeconds ? 1f : 60f), maxIntervalTime / (_showInSeconds ? 1f : 60f));

            if (_showInSeconds) _saveInterval = newValue;
            else _saveInterval = newValue * 60f;
        }

        private void SaveButton()
        {
            bool changed = _saveInterval != _previousSaveInterval;

            GUI.enabled = changed;

            if (GUILayout.Button("Save"))
            {
                string currentSaveInterval = _showInSeconds ? $"{_saveInterval:F0} seconds" : $"{_saveInterval / 60f:F1} minutes";

                EditorAutoSaver.SaveInterval = _saveInterval;
                _previousSaveInterval = _saveInterval;

                Debug.Log($"[EditorAutoSaver] Current Save Interval Is {currentSaveInterval}");
            }

            GUI.enabled = true;
        }
    }
}

#endif