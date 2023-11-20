#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using KimicuLayouts.Runtime;
using UnityEngine;
using UnityEditor;

namespace KimicuLayout.Editor.Settings
{
    public class KimicuLayoutsWindow : EditorWindow
    {
        private const float Spacing = 5;
        private const float Indent = 15;

        private static KimicuLayoutsSettings _settings;
        private static GUIStyle _headerStyle = null;

        private bool _paddingFoldout;
        private bool _sizeFoldout;

        [MenuItem("Kimicu/Layouts Settings")]
        private static void ShowWindow()
        {
            KimicuLayoutsWindow window = GetWindow<KimicuLayoutsWindow>();
            window.titleContent = new GUIContent("Layouts Settings");
            window.Show();
        }

        private void CreateGUI()
        {
            _paddingFoldout = EditorPrefs.GetBool(nameof(_paddingFoldout));
            _sizeFoldout = EditorPrefs.GetBool(nameof(_sizeFoldout));
            StylesLoad();
        }

        [Obsolete("Obsolete")]
        private void OnGUI()
        {
            StylesLoad();
            _settings = KimicuLayoutsSettings.Instance;
            PaddingType[] paddingTypes =
            {
                PaddingType.Left, PaddingType.Right,
                PaddingType.Top, PaddingType.Bottom,
            };

            // Padding
            Rect controlRect = EditorGUILayout.GetControlRect();
            Rect labelRect = new(controlRect.x, controlRect.y, 150, controlRect.height);
            Rect valueRect = new(labelRect.xMax, controlRect.y, controlRect.width - labelRect.width,
                controlRect.height);
            GUIContent content = new("Padding Textures");
            _paddingFoldout = EditorGUI.Foldout(labelRect, _paddingFoldout, content, true, _headerStyle);
            _settings.PaddingTextures[PaddingType.Group] = (Texture2D)EditorGUI
                .ObjectField(valueRect, _settings.PaddingTextures[PaddingType.Group], typeof(Texture2D));
            EditorPrefs.SetBool(nameof(_paddingFoldout), _paddingFoldout);
            if (_paddingFoldout)
            {
                string[] paddingNames = { "Left", "Right", "Top", "Bottom" };

                for (int i = 0; i < 4; i++)
                {
                    if (i % 2 == 0) controlRect = EditorGUILayout.GetControlRect();

                    float x = i % 2 == 0 ? controlRect.x + Indent : valueRect.xMax + Spacing;
                    labelRect = new Rect(x + (i >= 0 ? Spacing : 0), controlRect.y, i % 2 == 0 ? 30 : 50,
                        controlRect.height);

                    float offset = i % 2 == 0 ? -15 : 5;
                    float valueWidth = controlRect.width / 2 - (labelRect.width + Indent - Spacing) + offset;
                    valueRect = new Rect(labelRect.xMax, controlRect.y, valueWidth, controlRect.height);

                    content = new GUIContent(paddingNames[i]);
                    EditorGUI.PrefixLabel(labelRect, content);
                    _settings.PaddingTextures[paddingTypes[i]] = (Texture2D)EditorGUI
                        .ObjectField(valueRect, _settings.PaddingTextures[paddingTypes[i]], typeof(Texture2D));
                }
            }

            // Spacing
            EditorGUILayout.Separator();
            controlRect = EditorGUILayout.GetControlRect();
            labelRect = new(controlRect.x, controlRect.y, 150, controlRect.height);
            valueRect = new(labelRect.xMax, controlRect.y, controlRect.width - labelRect.width,
                controlRect.height);
            EditorGUI.LabelField(labelRect, $"Spacing Texture");
            _settings.SpacingTexture =
                (Texture2D)EditorGUI.ObjectField(valueRect, _settings.SpacingTexture, typeof(Texture2D));

            // Size
            EditorGUILayout.Separator();
            controlRect = EditorGUILayout.GetControlRect();
            labelRect = new Rect(controlRect.x, controlRect.y, 150, controlRect.height);
            valueRect = new Rect(labelRect.xMax, controlRect.y, controlRect.width - labelRect.width,
                controlRect.height);
            content = new GUIContent("Size Textures");
            _sizeFoldout = EditorGUI.Foldout(labelRect, _sizeFoldout, content, true, _headerStyle);
            EditorPrefs.SetBool(nameof(_sizeFoldout), _sizeFoldout);
            if (_sizeFoldout)
            {
                string[] sizeNames = { "Width", "Height" };
                controlRect = EditorGUILayout.GetControlRect();
                for (int i = 0; i < 2; i++)
                {
                    float x = i % 2 == 0 ? controlRect.x + Indent : valueRect.xMax + Spacing;
                    labelRect = new Rect(x + (i >= 0 ? Spacing : 0), controlRect.y, i % 2 == 0 ? 40 : 50,
                        controlRect.height);

                    float offset = i % 2 == 0 ? -15 : 5;
                    float valueWidth = controlRect.width / 2 - (labelRect.width + Indent - Spacing) + offset;
                    valueRect = new Rect(labelRect.xMax, controlRect.y, valueWidth, controlRect.height);

                    content = new GUIContent(sizeNames[i]);
                    EditorGUI.PrefixLabel(labelRect, content);
                    if (i == 0)
                        _settings.SizeTextures.width = (Texture2D)EditorGUI.ObjectField(valueRect,
                            _settings.SizeTextures.width, typeof(Texture2D));
                    else
                        _settings.SizeTextures.height = (Texture2D)EditorGUI.ObjectField(valueRect,
                            _settings.SizeTextures.height, typeof(Texture2D));
                }
            }
        }

        private static void StylesLoad()
        {
            _headerStyle ??= new GUIStyle(EditorStyles.foldout)
            {
                fontStyle = FontStyle.Bold,
                imagePosition = ImagePosition.ImageLeft
            };
        }
    }
}
#endif