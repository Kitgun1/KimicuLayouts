#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using System.IO;
using UnityEngine;

namespace KimicuLayouts.Runtime
{
    public class KimicuLayoutsSettings : ScriptableObject
    {
        private const string FolderPath = "Assets/Resources";
        private static readonly string Path = $"{FolderPath}/Kimicu Layouts Settings.asset";

        private static KimicuLayoutsSettings _instance;

        public static KimicuLayoutsSettings Instance
        {
            get
            {
                var settings = Resources.LoadAll<KimicuLayoutsSettings>("");
                if (settings.Length == 0)
                {
                    settings = new[] { CreateInstance<KimicuLayoutsSettings>() };
                    if (!Directory.Exists(FolderPath)) Directory.CreateDirectory(FolderPath);
                    #if UNITY_EDITOR
                    AssetDatabase.CreateAsset(settings[0], Path);
                    settings[0].TryInitialize();
                    AssetDatabase.SaveAssets();
                    #endif
                }

                _instance = settings[0];

                return _instance;
            }
        }

        public void TryInitialize()
        {
            if (PaddingTextures != null && PaddingTextures.Count != 5)
            {
                PaddingTextures = new PaddingTextures
                {
                    { PaddingType.Group, Resources.Load<Texture2D>("Full-1") },
                    { PaddingType.Left, Resources.Load<Texture2D>("Expand_left_stop") },
                    { PaddingType.Right, Resources.Load<Texture2D>("Expand_right_stop") },
                    { PaddingType.Top, Resources.Load<Texture2D>("Expand_top_stop") },
                    { PaddingType.Bottom, Resources.Load<Texture2D>("Expand_down_stop") },
                };
            }

            if (SpacingTexture == null) SpacingTexture = Resources.Load<Texture2D>("Move-2");
            if (SizeTextures.height == null) SizeTextures = (SizeTextures.width, Resources.Load<Texture2D>("Fluid"));
            if (SizeTextures.width == null) SizeTextures = (Resources.Load<Texture2D>("Fluid-1"), SizeTextures.height);
        }

        public PaddingTextures PaddingTextures;
        public Texture2D SpacingTexture;
        public (Texture2D width, Texture2D height) SizeTextures;
    }
}