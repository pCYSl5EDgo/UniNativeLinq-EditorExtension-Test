using System;
using System.IO;
using UniNativeLinq.Editor;
using UnityEditor;

public static class ExecuteMethods
{
    public static void CreateDll()
    {
        var settingWindow = EditorWindow.GetWindow<SettingWindow>();
        if(settingWindow is null)
            EditorApplication.Exit(12);
        settingWindow.Initialize();
        settingWindow.Execute();
    }
}