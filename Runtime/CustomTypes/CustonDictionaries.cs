using System;
using KimicuUtility;
using UnityEditor;
using UnityEngine;

namespace KimicuLayouts.Runtime
{
    [Serializable]
    public class PaddingTextures : SerializableDictionary<PaddingType, Texture2D> { }
    #if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(PaddingTextures))]
    public class PaddingTexturesDrawer : DictionaryDrawer<PaddingType, Texture2D> { }
    #endif
}