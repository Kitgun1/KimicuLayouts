#if UNITY_EDITOR
using UnityEditor;
#endif
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
                if (_instance != null) return _instance;
                var settings = Resources.LoadAll<KimicuLayoutsSettings>("");
                if (settings == null)
                {
                    settings =new []{CreateInstance<KimicuLayoutsSettings>()};
                    if (!Directory.Exists(FolderPath)) Directory.CreateDirectory(FolderPath);
                    #if UNITY_EDITOR
                    AssetDatabase.CreateAsset(settings[0], Path);
                    AssetDatabase.SaveAssets();
                    #endif
                }

                _instance = settings[0];

                return _instance;
            }
        }

        public PaddingTextures PaddingTextures = new PaddingTextures()
        {
            { PaddingType.Group, null },
            { PaddingType.Left, null },
            { PaddingType.Right, null },
            { PaddingType.Top, null },
            { PaddingType.Bottom, null },
        };

        public Texture2D SpacingTexture;
        public (Texture2D width, Texture2D height) SizeTextures;
    }
}