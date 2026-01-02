using UnityEditor;
using Unity.UOS.Common;
using UnityEngine;
using UnityEngine.Windows;

namespace Unity.UOS.Launcher
{
    public class FixSettings
    {
        private const string SettingsScriptPath = "Packages/cn.unity.uos.launcher/Scripts/Settings/Settings.cs";
        public static void Reimport()
        {
            AssetDatabase.ImportAsset(SettingsScriptPath, ImportAssetOptions.ForceUpdate);
            Debug.Log("UOS settings file reimported!");
        }

        public static void Delete()
        {
            File.Delete(Settings.Path);
            AssetDatabase.Refresh();
            Debug.Log("UOS settings file deleted!");
        }
    }
}