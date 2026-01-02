using System.Collections;
using System.Collections.Generic;
using Unity.UOS.Common;
using UnityEditor;
using UnityEngine;
using Unity.UOS.Networking;
using System;
using System.Threading.Tasks;
using Unity.UOS.Launcher.UI;

namespace Unity.UOS.Launcher
{
    internal class AppLinker
    {
        private static string _selectedProjectId;
        private static string _selectedProjectName;
        
        /// <summary>
        /// UOS APP 已经填写
        /// </summary>
        /// <returns></returns>
        public static bool IsFulfill()
        {
            return !string.IsNullOrEmpty(Settings.AppID) && 
                   !string.IsNullOrEmpty(Settings.AppSecret) && 
                   !string.IsNullOrEmpty(Settings.AppServiceSecret);
        }

        /// <summary>
        /// 获取 token
        /// </summary>
        /// <returns></returns>
        public static string GetAccessToken()
        {
            return CloudProjectSettings.accessToken;
        }
        
        public static async Task<UserInfo> GetUserInfo()
        {
            var accessToken = GetAccessToken();
            if (string.IsNullOrEmpty(accessToken)) return null;

            var userInfo = await LinkAPI.GetUserInfo(accessToken);
            return userInfo;
        }

        /// <summary>
        /// 是否已经登录
        /// </summary>
        /// <returns></returns>
        public static async Task<bool> IsLoggedIn()
        {
            var accessToken = GetAccessToken();
            if (string.IsNullOrEmpty(accessToken)) return false;

            // 尝试获取用户信息
            try
            {
                var userInfo = await GetUserInfo();
                return userInfo != null;
            }
            catch (Exception e)
            {
                Debug.Log(e);
                return false;
            }
        }

        public static async void LinkProject(string accessToken, string projectGuid, string userId)
        {
            try
            {
                var uosAppResp = await LinkAPI.GetUosApp(new CreateUosAppReq()
                {
                    projectId = projectGuid,
                    userId = userId,
                    // accessToken = accessToken,
                    // projectName = _selectedProjectName,
                    // organizationId = selectedOrg.id,
                    // organizationName = selectedOrg.name,
                    // createUserId = userInfo.foreign_key,
                    // createUsername = userInfo.name,
                    // createUserPhone = "",
                    // createUserEmail = userInfo.email,
                    // uosAppName = Application.productName,
                }, accessToken);
                
                await LinkAppInMainUI.LinkApp(uosAppResp.AppId, uosAppResp.AppSecret, uosAppResp.AppServiceSecret);
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
        }
    }
}