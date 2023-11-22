using System.Linq;
using KimicuLayouts.Runtime;

#if UNITY_EDITOR
using KimicuUtility;
using UnityEditor;
using UnityEngine;

namespace KimicuLayout.Editor
{
    [CustomEditor(typeof(KiHorizontalLayout))]
    public class KiHorizontalLayoutEditor : UnityEditor.Editor
    {
        private const float Spacing = 5;
        private const float Indent = 15;

        private static readonly Rect MouseRect = new Rect(20, 20, 140, 40);

        private KiHorizontalLayout _layout;
        private Event _event;
        private KimicuLayoutsSettings _settings;

        private GUIStyle _headerStyle;

        private bool _paddingFoldout;
        private readonly bool[] _paddingLeftPress = new bool[4];
        private bool _spacingPress;
        private readonly VectorBoolean2 _sizePress = new();

        private void OnEnable()
        {
            _event ??= Event.current;
            _paddingFoldout = EditorPrefs.GetBool(nameof(_paddingFoldout), true);

            _headerStyle ??= new GUIStyle(EditorStyles.foldout)
            {
                fontStyle = FontStyle.Bold,
                imagePosition = ImagePosition.ImageLeft
            };

            EditorApplication.update += Update;
        }

        private void OnDisable()
        {
            _event ??= Event.current;
            EditorApplication.update -= Update;
        }

        public override void OnInspectorGUI()
        {
            _event ??= Event.current;
            _settings = KimicuLayoutsSettings.Instance;
            _settings.TryInitialize();
            _layout = (KiHorizontalLayout)target;

            // Padding
            Rect controlRect = EditorGUILayout.GetControlRect();
            Rect labelRect = new(controlRect);
            Rect valueRect = new();
            GUIContent content = new("Padding", _settings.PaddingTextures[PaddingType.Group]);
            _paddingFoldout = EditorGUI.Foldout(labelRect, _paddingFoldout, content, true, _headerStyle);
            EditorPrefs.SetBool(nameof(_paddingFoldout), _paddingFoldout);
            if (_paddingFoldout)
            {
                string[] paddingNames = { "Left", "Right", "Top", "Bottom" };
                PaddingType[] paddingTypes =
                {
                    PaddingType.Left, PaddingType.Right,
                    PaddingType.Top, PaddingType.Bottom
                };

                var fields = _layout.Padding.GetType().GetProperties();
                for (int i = 0; i < 4; i++)
                {
                    if (i % 2 == 0) controlRect = EditorGUILayout.GetControlRect();

                    float x = i % 2 == 0 ? controlRect.x + Indent : valueRect.xMax + Spacing;
                    labelRect = new Rect(x, controlRect.y, i % 2 == 0 ? 60 : 80, controlRect.height);

                    float offset = i % 2 == 0 ? -10 : 10;
                    float valueWidth = controlRect.width / 2 - (labelRect.width + Indent - Spacing) + offset;
                    valueRect = new Rect(labelRect.xMax, controlRect.y, valueWidth, controlRect.height);

                    content = new GUIContent(paddingNames[i], _settings.PaddingTextures[paddingTypes[i]]);
                    EditorGUI.PrefixLabel(labelRect, content);
                    fields.First(f => f.Name == paddingNames[i].ToLower())
                        .SetValue(_layout.Padding,
                            EditorGUI.IntField(valueRect, (int)fields
                                .First(f => f.Name == paddingNames[i].ToLower())
                                .GetValue(_layout.Padding)));

                    if (_event.type == EventType.MouseDown && labelRect.Contains(_event.mousePosition))
                        _paddingLeftPress[i] = true;

                    if (labelRect.Contains(_event.mousePosition) || _paddingLeftPress[i])
                        EditorGUIUtility.AddCursorRect(MouseRect, MouseCursor.SlideArrow);

                    if (_event.type == EventType.MouseUp && _paddingLeftPress[i]) _paddingLeftPress[i] = false;
                }
            }

            // Spacing
            controlRect = EditorGUILayout.GetControlRect();
            labelRect = new Rect(controlRect.x, controlRect.y, 75, controlRect.height);
            float width = controlRect.width - labelRect.width;
            valueRect = new Rect(labelRect.xMax, controlRect.y, width, controlRect.height);
            content = new GUIContent("Spacing", _settings.SpacingTexture);
            EditorGUI.PrefixLabel(labelRect, content);
            _layout.Spacing = EditorGUI.FloatField(valueRect, _layout.Spacing);

            if (_event.type == EventType.MouseDown && labelRect.Contains(_event.mousePosition)) _spacingPress = true;

            if (labelRect.Contains(_event.mousePosition) || _spacingPress)
                EditorGUIUtility.AddCursorRect(MouseRect, MouseCursor.SlideArrow);

            if (_event.type == EventType.MouseUp && _spacingPress) _spacingPress = false;

            // Height | Width
            string[] labels = { "Width", "Height" };
            controlRect = EditorGUILayout.GetControlRect();
            Rect postValueRect = new();
            for (int i = 0; i < 2; i++)
            {
                float startPosition = i == 0 ? controlRect.x : postValueRect.xMax + Spacing * 2;
                labelRect = new Rect(startPosition, controlRect.y, 65, controlRect.height);
                width = controlRect.width / 2 - labelRect.width - Spacing * 2 - 30 - (i == 0 ? 10 : 0);
                valueRect = new Rect(labelRect.xMax + (i == 0 ? 10 : 0), controlRect.y, width, controlRect.height);
                Rect postLabelRect = new Rect(valueRect.xMax + Spacing, controlRect.y, 15, controlRect.height);
                postValueRect = new Rect(postLabelRect.xMax, controlRect.y, 15, controlRect.height);
                Texture2D targetTexture = i == 0 ? _settings.SizeTextures.Item1 : _settings.SizeTextures.Item2;
                content = new GUIContent(labels[i], targetTexture);
                EditorGUI.PrefixLabel(labelRect, content);
                if (i == 0)
                {
                    if (_layout.ByPercentage.X) EditorGUI.BeginDisabledGroup(true);
                    _layout.Width = EditorGUI.FloatField(valueRect, _layout.Width);
                    if (_layout.ByPercentage.X) EditorGUI.EndDisabledGroup();
                }
                else
                {
                    if (_layout.ByPercentage.Y || !_layout.ControlSizeHeight) EditorGUI.BeginDisabledGroup(true);
                    _layout.Height = EditorGUI.FloatField(valueRect, _layout.Height);
                    if (_layout.ByPercentage.Y || !_layout.ControlSizeHeight) EditorGUI.EndDisabledGroup();
                }

                EditorGUI.PrefixLabel(postLabelRect, new GUIContent("%"));
                if (i == 0) _layout.ByPercentage.X = EditorGUI.Toggle(postValueRect, _layout.ByPercentage.X);
                else
                {
                    if (!_layout.ControlSizeHeight) EditorGUI.BeginDisabledGroup(true);
                    _layout.ByPercentage.Y = EditorGUI.Toggle(postValueRect, _layout.ByPercentage.Y);
                    if (!_layout.ControlSizeHeight) EditorGUI.EndDisabledGroup();
                }

                if (_event.type == EventType.MouseDown && labelRect.Contains(_event.mousePosition))
                {
                    if (i == 0) _sizePress.X = true;
                    else _sizePress.Y = true;
                }

                if (labelRect.Contains(_event.mousePosition) || i == 0 ? _sizePress.X : _sizePress.Y)
                    EditorGUIUtility.AddCursorRect(MouseRect, MouseCursor.SlideArrow);

                if (_event.type == EventType.MouseUp && (i == 0 ? _sizePress.X : _sizePress.Y))
                {
                    if (i == 0) _sizePress.X = false;
                    else _sizePress.Y = false;
                }
            }

            controlRect = EditorGUILayout.GetControlRect();
            labelRect = new Rect(controlRect.width / 2 + 22.5f, controlRect.y, 60 + Spacing, controlRect.height);
            valueRect = new Rect(labelRect.xMax, controlRect.y, 15, controlRect.height);
            content = new GUIContent("Control Y");
            EditorGUI.PrefixLabel(labelRect, content);
            _layout.ControlSizeHeight = EditorGUI.Toggle(valueRect, _layout.ControlSizeHeight);
            if (!_layout.ControlSizeHeight) _layout.ByPercentage.Y = false;

            labelRect = new Rect(controlRect.xMax - 15 - 65, controlRect.y, 60 + Spacing, controlRect.height);
            valueRect = new Rect(controlRect.xMax - 15, controlRect.y, 15, controlRect.height);
            content = new GUIContent("Vertical Fit");
            EditorGUI.PrefixLabel(labelRect, content);
            _layout.VerticalFit = EditorGUI.Toggle(valueRect, _layout.VerticalFit);


            Repaint();
            _layout.CalculateLayoutInputHorizontal();
        }

        private void Update()
        {
            _event ??= Event.current;
            if (_event == null) return;
            if (_event.type == EventType.MouseUp)
            {
                for (int i = 0; i < 4; i++) _paddingLeftPress[i] = false;
                _spacingPress = false;
                _sizePress.X = false;
                _sizePress.Y = false;
            }

            for (int i = 0; i < 4; i++)
            {
                if (!_paddingLeftPress[i]) continue;
                Debug.LogWarning($"{(_event.delta.x / 10).Snap(0.1f)}");
                var fields = _layout.Padding.GetType().GetProperties();
                fields[i].SetValue(_layout.Padding,
                    (int)fields[i].GetValue(_layout.Padding) + (int)_event.delta.x);
                Repaint();
            }

            if (_spacingPress)
            {
                Debug.LogWarning($"{(_event.delta.x / 10).Snap(0.1f)}");
                _layout.Spacing += (_event.delta.x / 10).Snap(0.1f);
                Repaint();
            }

            if (_sizePress.X)
            {
                Debug.LogWarning($"{(_event.delta.x / 10).Snap(0.1f)}");
                _layout.Width += (_event.delta.x / 10).Snap(0.1f);
                Repaint();
            }

            if (_sizePress.Y)
            {
                Debug.LogWarning($"{(_event.delta.x / 10).Snap(0.1f)}");
                _layout.Height += (_event.delta.x / 10).Snap(0.1f);
                Repaint();
            }
        }
    }
}
#endif