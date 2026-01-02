using System;
using Unity.UOS.Common;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UOSLauncher.Editor.Environment;

namespace Unity.UOS.Launcher.UI
{
    public class LinkAppInMainUI
    {
        // 未关联
        private static VisualElement _appUnlinkedPanel;
        private static Button _openLinkAppWindowButton;

        // 已关联
        private static VisualElement _appLinkedPanel;
        private static Label _linkedAppNameText;
        private static TextField _linkedAppIDTextField;
        private static Button _goToDevPortalForThisAppButton;
        private static Button _editBtn;
        // 错误提示
        private static VisualElement _corruptHint;
        private const string FixSettingMessageTooltip = "UOS settings file is corrupt. Please fix compile errors first, and then click to fix the settings file.";
        
        private static Button _fixSettingsByReimportButton;
        private static Button _fixSettingsByDeleteButton;
        public static async Task Init(VisualElement root)
        {
            // 获取 UI 元素
            // 未关联
            _appUnlinkedPanel = root.Q<VisualElement>("AppUnlinked");
            _openLinkAppWindowButton = root.Q<Button>("OpenLinkAppWindow");
            
            // 已关联
            _appLinkedPanel= root.Q<VisualElement>("AppLinked");
            _linkedAppNameText = root.Q<Label>("LinkedAppNameText");
            _linkedAppIDTextField = root.Q<TextField>("LinkedAppIDTextField");
            _goToDevPortalForThisAppButton = root.Q<Button>("GoToDevPortalForThisApp");
            _fixSettingsByReimportButton = root.Q<Button>("FixSettingsByReimportButton");
            _fixSettingsByDeleteButton = root.Q<Button>("FixSettingsByDeleteButton");
            EnvironmentUI.Init(root);
            
            _editBtn = root.Q<Button>("Edit");
            // 错误提示
            _corruptHint = root.Q<VisualElement>("CorruptHint");
            _fixSettingsByReimportButton.clicked += async () =>
            {
                FixSettings.Reimport();
                if (AppLinker.IsFulfill())
                {
                    await LinkApp();
                }
            };
            _fixSettingsByDeleteButton.clicked += () =>
            {
                FixSettings.Delete();
                CheckSettingsFile();
            };
            // 事件注册
            _openLinkAppWindowButton.clicked += LinkAppWindow.Open;
            _editBtn.clicked += LinkAppWindow.Open;
            _goToDevPortalForThisAppButton.clicked += () =>
            {
                Application.OpenURL($"https://uos.unity.cn/services/{Settings.AppID}");
            };

            CheckSettingsFile();
            
            // 自动关联
            if (AppLinker.IsFulfill())
            {
                await LinkApp();
            }
        }

        /// <summary>
        /// 配置文件损坏提示
        /// </summary>
        private static void SetSettingsFileCorruptStatus(bool corrupt)
        {
            Utils.Hide(_corruptHint, !corrupt);
            SetOpenLinkAppWindowButton(!corrupt);
        }

        /// <summary>
        /// 设置打开 Link App 窗口按钮
        /// </summary>
        /// <param name="enable"></param>
        private static void SetOpenLinkAppWindowButton(bool enable = true)
        {
            _openLinkAppWindowButton.SetEnabled(enable);
            _openLinkAppWindowButton.tooltip = enable ? 
                "": FixSettingMessageTooltip;
        }

        /// <summary>
        /// 将 app 信息写入 asset
        /// </summary>
        /// <param name="appID"></param>
        /// <param name="appSecret"></param>
        /// <param name="appServiceSecret"></param>
        private static void SetAppInfo(string appID, string appSecret, string appServiceSecret)
        { 
            Settings.AppID = appID;
            Settings.AppSecret = appSecret;
            Settings.AppServiceSecret = appServiceSecret;
        }
        
        private static void SetUserName(string userName)
        {
            Settings.UserName = userName;
        }
        
        private static void SetProjectId(string projectId)
        {
            Settings.ProjectId = projectId;
        }
        
        /// <summary>
        /// 检查配置文件
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private static void CheckSettingsFile()
        {
            var isSettingsFileCorrupt = Settings.IsSettingsFileCorrupt();
            SetSettingsFileCorruptStatus(isSettingsFileCorrupt);

            if (isSettingsFileCorrupt)
            {
                throw new Exception(Settings.SettingsFileCorruptHint);
            }
        }

        internal static async Task LinkApp()
        {
            CheckSettingsFile();
            await LinkApp(Settings.AppID, Settings.AppSecret, Settings.AppServiceSecret);
        }
        
        internal static async Task LinkApp(string appID, string appSecret, string appServiceSecret)
        {
            AppInfo appInfo = null;
            try
            { 
                appInfo = await API.GetUosApp(appID, appServiceSecret);
               try
               {
                   var versionInfo = new UpdateAppInfo()
                   {
                       Properties = new Dictionary<string, string>()
                       {
                           { "editorVersion", Application.unityVersion },
                           { "launcherVersion", KnownService.Launcher.installedVersion }
                       }
                   };
                   appInfo = await API.UpdateAppInfo(appID, appServiceSecret, JsonConvert.SerializeObject(versionInfo));
               }
               catch (Exception e)
               {
                   Debug.Log("更新 UOS APP 失败：" + e.Message);
               }
               // 设置环境信息
               EnvironmentManager.AddOrUpdate(new EnvironmentConfig()
               {
                   environmentName = appInfo.ProjectName,
                   appID = appID,
                   appSecret = appSecret,
                   appServiceSecret = appServiceSecret,
               });
               EnvironmentUI.UpdateOptions();
                
               var map = ServiceManager.GetServiceMapWithStatus(appInfo);
               LauncherUI.DrawServiceList(map);
            }
            catch (Exception e)
            {
                throw new Exception("Failed to link APP, please check input. Details: " + e.Message);
            }

            CheckSettingsFile();
            var userName = GetUserName(appInfo);
    
            // 设置 project id
            SetProjectId(appInfo.ProjectId);
            SetInfoIntoSettings(appID, appSecret, appServiceSecret, userName, appInfo.ProjectId);
            EnvironmentManager.SetEnvironmentInfo();
            ShowInfo(appInfo.ProjectName, appID);

            Utils.Hide(_goToDevPortalForThisAppButton, false);
            Utils.Hide(_appLinkedPanel, false);
            Utils.Hide(_appUnlinkedPanel);
        }

        internal static string GetUserName(AppInfo appInfo)
        {
            var userNameKey = "createdUserName";
            appInfo.Properties.TryGetValue(userNameKey, out var userName);
            return userName;
        }

        /// <summary>
        /// 设置 Settings 中的信息
        /// </summary>
        /// <param name="appID"></param>
        /// <param name="appSecret"></param>
        /// <param name="appServiceSecret"></param>
        /// <param name="userName"></param>
        /// <param name="projectId"></param>
        private static void SetInfoIntoSettings(string appID, string appSecret, string appServiceSecret, string userName, string projectId)
        {
            // 关联成功，存储和显示信息
            SetAppInfo(appID, appSecret, appServiceSecret);
            // 设置 username
            if (!string.IsNullOrEmpty(userName))
            {
                SetUserName(userName);
            }
            // 设置 project id
            SetProjectId(projectId);
        }
        
        /// <summary>
        /// 在界面上显示信息
        /// </summary>
        /// <param name="projectName"></param>
        /// <param name="appID"></param>
        private static void ShowInfo(string projectName, string appID)
        {
            _linkedAppNameText.text = projectName;
            _linkedAppIDTextField.SetValueWithoutNotify(appID);
            EnvironmentUI.SetActiveEnvironment();
        }
    }
}