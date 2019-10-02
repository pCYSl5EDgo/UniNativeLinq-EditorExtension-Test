using System;
using System.IO;
using UniNativeLinq.Editor;
using UnityEditor;

public static class ExecuteMethods
{
    public static void CreateDll()
    {
        Console.WriteLine("THIS IS A TEST");
        var settingWindow = EditorWindow.GetWindow<SettingWindow>();
        if(settingWindow is null){
            Console.WriteLine("NULL!!!");
            EditorApplication.Exit(12);
        }
        settingWindow.Initialize();
        settingWindow.Execute();
    }
}