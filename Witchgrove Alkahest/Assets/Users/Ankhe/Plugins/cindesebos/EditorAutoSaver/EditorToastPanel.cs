#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;
using System;

namespace cindesebos.EditorAutoSaver
{
    [InitializeOnLoad]
    public static class EditorToastPanel
    {
        private const float ToastDuration = 1f;
        private const float ToastFadeOutTime = 0.5f;

        private static string _message;
        private static double _toastStartTime;

        static EditorToastPanel() => SceneView.duringSceneGui += OnSceneGUI;

        public static void Show(string message)
        {
            _message = message;

            _toastStartTime = EditorApplication.timeSinceStartup;

            EditorApplication.update -= Update;
            EditorApplication.update += Update;
        }

        private static void Update()
        {
            if (string.IsNullOrEmpty(_message))
            {
                EditorApplication.update -= Update;

                return;
            }

            double elapsed = EditorApplication.timeSinceStartup - _toastStartTime;

            if (elapsed > ToastDuration)
            {
                _message = null;

                EditorApplication.update -= Update;

                return;
            }

            SceneView.RepaintAll();
        }

        private static void OnSceneGUI(SceneView sceneView)
        {
            if (string.IsNullOrEmpty(_message)) return;

            double elapsed = EditorApplication.timeSinceStartup - _toastStartTime;

            if (elapsed > ToastDuration)
            {
                _message = null;
                return;
            }

            float alpha = 1f;
            if (elapsed > ToastDuration - ToastFadeOutTime)
                alpha = 1f - (float)((elapsed - (ToastDuration - ToastFadeOutTime)) / ToastFadeOutTime);

            Handles.BeginGUI();

            float width = sceneView.position.width;
            float height = sceneView.position.height;

            var rect = new Rect(
                width - 310,
                height - 90,
                300,
                30
            );

            var style = new GUIStyle(GUI.skin.box)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 12,
                normal = { textColor = new Color(1f, 1f, 1f, alpha) }
            };

            Color backgroundColor = new Color(0.15f, 0.15f, 0.15f, alpha);

            EditorGUI.DrawRect(rect, backgroundColor);
            
            GUI.Label(rect, _message, style);

            Handles.EndGUI();

            sceneView.Repaint();
        }

    }
}

#endif
