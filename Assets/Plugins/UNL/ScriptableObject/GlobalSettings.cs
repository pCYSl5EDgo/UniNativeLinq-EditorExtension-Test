﻿using UnityEditor;
using UnityEngine;

namespace UniNativeLinq.Editor
{
    public sealed class GlobalSettings : ScriptableObject
    {
        public bool EnableNullCheckOnRuntime;


        private static GlobalSettings _instance;

        public static GlobalSettings Instance
        {
            get
            {
                if(!(_instance is null))
                    return _instance;
                var assetPath = AssetDatabase.GUIDToAssetPath("Assets/Plugins/UNL/Settings/GlobalSettings.asset");
                _instance = AssetDatabase.LoadAssetAtPath<GlobalSettings>(assetPath);
                return _instance;
            }
        }
    }
}