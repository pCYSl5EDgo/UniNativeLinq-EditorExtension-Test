using System;
using System.IO;
using UniNativeLinq.Editor;
using UnityEditor;

public static class ExecuteMethods
{
    public static void CreateDll()
    {
        Console.WriteLine("THIS IS A TEST");
        SettingWindow settingWindow = default;
        try
        {
            settingWindow = EditorWindow.GetWindow<SettingWindow>();            
        }
        catch (System.Exception e)
        {
            Console.WriteLine(e.ToString());
            throw;
        }
        if(settingWindow is null){
            Console.WriteLine("NULL!!!");
            EditorApplication.Exit(12);
        }
        try
        {
            settingWindow.Initialize();
            settingWindow.Execute();
        }
        catch(Exception e)
        {
            Console.WriteLine(e.ToString());
            throw;
        }
    }
}