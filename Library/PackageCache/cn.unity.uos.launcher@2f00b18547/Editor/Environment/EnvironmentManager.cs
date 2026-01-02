using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.UOS.Common;
using UnityEditor;
using UnityEngine;
using Unity.UOS.Launcher;
using Unity.UOS.Launcher.UI;

namespace UOSLauncher.Editor.Environment
{
    public static class EnvironmentManager
    {
        public const string DEFAULT_ASSETS_PATH = "Assets/Editor/UOSEnvironments.asset";
        private static EnvironmentList _environmentList;
        
        public static List<string> Options => EnvironmentListInstance.Environments.Select(env => env.environmentName).ToList();
        
        public static EnvironmentConfig SelectedEnvironment => EnvironmentListInstance.SelectedEnvironment;
        
        public static EnvironmentList EnvironmentListInstance
        {
            get
            {
                if (_environmentList == null)
                {
                    // 读取已有配置
                    _environmentList = AssetDatabase.LoadAssetAtPath<EnvironmentList>(DEFAULT_ASSETS_PATH);;
                }

                if (_environmentList == null)
                {
                    CreateNewResource();
                    SaveResource();
                }

                return _environmentList;
            }
        }

        public static void CorrectSelectedAppID()
        {
            if (_environmentList.SelectedEnvironment == null && !string.IsNullOrEmpty(Settings.AppID))
            {
                AddOrUpdate(new EnvironmentConfig()
                {
                    appID = Settings.AppID,
                    appSecret = Settings.AppSecret,
                    appServiceSecret = Settings.AppServiceSecret
                });
            }
        }

        // 保存资源
        private static void SaveResource()
        {
            if (_environmentList != null)
            {
                EditorUtility.SetDirty(_environmentList);
                AssetDatabase.SaveAssets();
            }
        }

        // 创建新资源
        private static void CreateNewResource()
        {
            _environmentList = ScriptableObject.CreateInstance<EnvironmentList>();
            string directoryPath = Path.GetDirectoryName(DEFAULT_ASSETS_PATH);
            if (!string.IsNullOrEmpty(directoryPath) && !Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
                AssetDatabase.Refresh();
            }
            AssetDatabase.CreateAsset(_environmentList, DEFAULT_ASSETS_PATH);
        }
        
        internal static EnvironmentConfig SwitchSelectedAppID(string environmentName)
        {          
            return EnvironmentListInstance.SwitchSelectedAppID(environmentName);
        }

        public static async void SwitchEnvironment(string environmentName)
        {
            var environmentConfig = EnvironmentListInstance.SwitchSelectedAppID(environmentName);
            if (environmentConfig == null)
            {
                Debug.LogError($"环境 {environmentName} 不存在！请检查环境名称");
                return;
            }
            var appInfo = await API.GetUosApp(environmentConfig.appID, environmentConfig.appServiceSecret);
            Settings.AppID = environmentConfig.appID;
            Settings.AppSecret = environmentConfig.appSecret;
            Settings.AppServiceSecret = environmentConfig.appServiceSecret;
            Settings.ProjectId = appInfo.ProjectId;
            var userName = LinkAppInMainUI.GetUserName(appInfo);
            if (!string.IsNullOrEmpty(userName))
            {
                Settings.UserName = userName;

            }
            SetEnvironmentInfo();
        }
        
        // 添加环境
        public static void AddOrUpdate(EnvironmentConfig newConfig)
        {
            EnvironmentListInstance.AddOrUpdate(newConfig);
            SaveResource();
        }

        public static void Configure()
        {
            EditorApplication.ExecuteMenuItem("Window/General/Inspector");
            // 选中资源并聚焦Project窗口
            Selection.activeObject = EnvironmentListInstance;
            EditorUtility.FocusProjectWindow();
        }
        
        public static void SetEnvironmentInfo()
        {
            if (SelectedEnvironment == null) return;
            Settings.ConfigPairs = SelectedEnvironment.configPairs;
            Settings.RoomProfileUUID = SelectedEnvironment.roomProfileUUID;
            Settings.MatchmakingConfigID = SelectedEnvironment.matchmakingConfigID;
        }
    }
}