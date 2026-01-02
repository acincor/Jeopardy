using System;
using Unity.UOS.Common;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Unity.UOS.Launcher.UI
{
    public class LinkAppByAppIDTab
    {
        private static TextField _appIDInput;
        private static TextField _appSecretInput;
        private static TextField _appServiceSecretInput;
        
        // 用于保留当前 app 信息原始值
        private static string _appIDValue;
        private static string _appSecretValue;
        private static string _appServiceSecretValue;
        
        private static Label _errorMessageLabel;
        private static Button _goToDeveloperPortalButton;

        public static async Task Init(VisualElement root, TabbedMenuController tabbedMenuController)
        {
            // 获取 UI 元素
            _appIDInput = root.Q<TextField>("AppID");
            _appSecretInput = root.Q<TextField>("AppSecret");
            _appServiceSecretInput = root.Q<TextField>("AppServiceSecret");

            _errorMessageLabel = root.Q<Label>("ErrorMessageForAppInfo");
            _goToDeveloperPortalButton = root.Q<Button>("GoToDeveloperPortalForAppButton");

            // 事件注册

            _appIDInput.RegisterValueChangedCallback((e) =>
            {
                _appIDValue = e.newValue; 
            });
            _appSecretInput.RegisterValueChangedCallback((e) =>
            {
                _appSecretValue = e.newValue;
                _appSecretInput.SetValueWithoutNotify(ReplaceWithSymbol(_appSecretValue));
            });
            _appServiceSecretInput.RegisterValueChangedCallback((e) =>
            {
                _appServiceSecretValue = e.newValue;
                _appServiceSecretInput.SetValueWithoutNotify(ReplaceWithSymbol(_appServiceSecretValue));
            });
            _goToDeveloperPortalButton.clicked += () =>
            {
                Application.OpenURL("https://uos.unity.cn/apps");
            };

            if (AppLinker.IsFulfill())
            {
                GetSettings();
            }
        }
        
        internal static void GetSettings()
        {
            try
            {
                _appIDValue = Settings.AppID;
                _appSecretValue = Settings.AppSecret;
                _appServiceSecretValue = Settings.AppServiceSecret;
                _appIDInput.SetValueWithoutNotify(_appIDValue);
                _appSecretInput.SetValueWithoutNotify(ReplaceWithSymbol(_appSecretValue));
                _appServiceSecretInput.SetValueWithoutNotify(ReplaceWithSymbol(_appServiceSecretValue));
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
        }

        private static string ReplaceWithSymbol(string input, int start = 8, int end = 23, char symbol = '*')
        {
            if (string.IsNullOrEmpty(input) || end < start || end >= input.Length)
                return input;

            return input.Substring(0, start) + new string(symbol, end - start + 1) + input.Substring(end + 1);
        }

        /// <summary>
        /// 检查 app 信息是否存在空项
        /// </summary>
        private static bool IsEmpty()
        {
            var appID = _appIDValue;
            var appSecret = _appSecretValue;
            var appServiceSecret = _appServiceSecretValue;

            return string.IsNullOrEmpty(appID) && string.IsNullOrEmpty(appSecret) &&
                   string.IsNullOrEmpty(appServiceSecret);
        }

        internal static async void ConfirmLink()
        {
            var appID = _appIDValue; 
            var appSecret = _appSecretValue;
            var appServiceSecret = _appServiceSecretValue;

            try
            {
                await LinkAppInMainUI.LinkApp(appID, appSecret, appServiceSecret);
                ShowErrorMessage("");
                LinkAppWindow.CloseWindow();
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
                ShowErrorMessage(e.Message);
            }
        }
                
        private static void ShowErrorMessage(string message)
        {
            _errorMessageLabel.text = message;
        }
    }
}