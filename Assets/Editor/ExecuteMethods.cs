using System;
using System.Linq;
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

    public static void CreateUnityPackage()
    {
        // configure
        var root = "Plugins/UNL";
                var exportPath = Path.Combine(Application.dataPath + "/", "../../artifact_unity/UniNativeLinq.unitypackage");

        var path = Path.Combine(Application.dataPath, root);
        var assets = Directory.EnumerateFiles(path, "*", SearchOption.AllDirectories)
            .Where(x =>
            {
                var extension = Path.GetExtension(x);
                return extension == ".cs" || extension == ".asset" || extension == ".bytes";
            })
            .Select(x => "Assets" + x.Replace(Application.dataPath, "").Replace(@"\", "/"))
            .ToArray();

        UnityEngine.Debug.Log("Export below files" + Environment.NewLine + string.Join(Environment.NewLine, assets));

        AssetDatabase.ExportPackage(
            assets,
            exportPath,
            ExportPackageOptions.Default);

        UnityEngine.Debug.Log("Export complete: " + Path.GetFullPath(exportPath));
    }

    public static void CreateSettingUnityPackage()
    {
        // configure
        var root = "Plugins/UNL/Settings";
        var exportPath = Path.Combine(Application.dataPath + "/", "../../artifact_setting/UniNativeLinq-Settings.unitypackage");

        var path = Path.Combine(Application.dataPath, root);
        var assets = Directory.EnumerateFiles(path, "*", SearchOption.AllDirectories)
            .Where(x => Path.GetExtension(x) == ".asset")
            .Select(x => "Assets" + x.Replace(Application.dataPath, "").Replace(@"\", "/"))
            .ToArray();

        UnityEngine.Debug.Log("Export below files" + Environment.NewLine + string.Join(Environment.NewLine, assets));

        AssetDatabase.ExportPackage(
            assets,
            exportPath,
            ExportPackageOptions.Default);

        UnityEngine.Debug.Log("Export complete: " + Path.GetFullPath(exportPath));
    }
}