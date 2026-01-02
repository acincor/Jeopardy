using System;
using UnityEditor;
using UnityEngine;
using System.Threading.Tasks;
using Unity.UOS.Common;
using System.Collections.Generic;
using NUnit.Framework.Constraints;
using NUnit.Framework.Interfaces;

namespace Unity.UOS.Launcher.IMGUI
{
    public class LauncherWindowIMGUI : EditorWindow
    {
        private static readonly int MinWindowWidth = 503;
        private static string _appID;
        private static string _appSecret;
        private static string _appServiceSecret;
        private static List<ServiceConfig> _upcomingList = new List<ServiceConfig>();
        private static List<ServiceConfig> _serviceList = new List<ServiceConfig>();
        private static List<ServiceConfig> _libraryList = new List<ServiceConfig>();
        private static string _projectName = "";


        [MenuItem("UOS/Open Launcher", false, -110)]
        public static void  Open()
        {
            LauncherWindowIMGUI window =
                (LauncherWindowIMGUI)GetWindow(typeof(LauncherWindowIMGUI), false, "Unity Online Services", true);
            window.minSize = new Vector2(MinWindowWidth, 600);
            window.Show();
        }

        private async void OnEnable()
        {
            LauncherUIIMGUI.OnInstall = PackageManager.Install;
            LauncherUIIMGUI.OnDetail = PackageManager.Detail;
            LauncherUIIMGUI.OnRemove = PackageManager.Remove;
            LauncherUIIMGUI.OnEnable = ServiceManager.EnableService;
            
            // 获取服务配置
            var (res1, res2, res3) = await ServiceManager.GetServiceList();
            _upcomingList = res1;
            _serviceList = res2;
            _libraryList = res3;
            
            PackageResourcesManager.Check();
            _appID = Settings.AppID;
            _appSecret = Settings.AppSecret;
            _appServiceSecret = Settings.AppServiceSecret;
            Link();
            
             Debug.LogWarning(LauncherUIIMGUI.WarningText);
        }

        private void OnGUI()
        {
            // App ID
            GUILayout.BeginHorizontal();
            GUILayout.BeginHorizontal(GUILayout.Width(150));
            GUILayout.Label("App ID");
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            _appID = GUILayout.TextField(_appID);
            GUILayout.EndHorizontal();
            
            // App Secret
            GUILayout.BeginHorizontal();
            GUILayout.BeginHorizontal(GUILayout.Width(150));
            GUILayout.Label("App Secret");
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            _appSecret = GUILayout.TextField(_appSecret);
            GUILayout.EndHorizontal();

            // App Service Secret
            GUILayout.BeginHorizontal();
            GUILayout.BeginHorizontal(GUILayout.Width(150));
            GUILayout.Label("App Service Secret");
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            _appServiceSecret = GUILayout.TextField(_appServiceSecret);
            GUILayout.EndHorizontal();
            
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Link App", GUILayout.Width(120), GUILayout.Height(28)))
            {
                Link();
            }
            GUILayout.Label(string.IsNullOrEmpty(_projectName) ? "Failed to link App" : $"Successfully linked to App: {_projectName}");
            GUILayout.EndHorizontal();
            LauncherUIIMGUI.Init(_serviceList);
        }
        
        private static async Task Link()
        {
            var appID = _appID;
            var appSecret =  _appSecret;
            var appServiceSecret = _appServiceSecret;

            AppInfo appInfo = null;
            try
            {
                appInfo = await API.GetUosApp(appID, appServiceSecret);
                _projectName = appInfo.ProjectName;
                Debug.Log($"Successfully linked to App: {appInfo.ProjectName}");
            }
            catch (Exception e)
            {
                Debug.LogError("Failed to link App, please check input.");
                // Debug.LogError("Failed to link APP, please check input. Details: " + e.Message);
                _projectName = "";
                return;
            }
            
            try
            {
                SetSettings(appID, appSecret, appServiceSecret);
            }
            catch (Exception e)
            {
                return;
            }
        }
        
        private static void SetSettings(string appID, string appSecret, string appServiceSecret)
        {
            try {
                Settings.AppID = appID;
                Settings.AppSecret = appSecret;
                Settings.AppServiceSecret = appServiceSecret;
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
        }
    }
}