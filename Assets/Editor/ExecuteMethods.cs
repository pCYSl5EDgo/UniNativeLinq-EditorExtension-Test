using System;
using System.IO;
using UniNativeLinq.Editor;
using UnityEditor;
using UnityEngine;

public static class ExecuteMethods
{
    public static void CreateDll()
    {
        var settingWindow = Application.isBatchMode ? new SettingWindow() : EditorWindow.GetWindow<SettingWindow>();
        settingWindow.Initialize();
        settingWindow.Execute();
    }
}