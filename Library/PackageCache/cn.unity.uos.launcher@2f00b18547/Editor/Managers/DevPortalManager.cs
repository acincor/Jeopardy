using System;
using System.Collections.Generic;
using Unity.UOS.Common;
using UnityEngine;

namespace Unity.UOS.Launcher
{
    public class DevPortalManager
    {
        private const string Domain = "https://uos.unity.cn";
        /// <summary>
        /// 跳转到 dev portal 配置页
        /// </summary>
        /// <param name="config"></param>
        public static void Jump(ServiceConfig config)
        {
            
            var path = "";

            if (!string.IsNullOrEmpty(config.devPortalPath))
            {
                path = config.devPortalPath.Replace("{Settings.AppID}", Settings.AppID);
            }

            // Matchmaking 区分子类型
            if (config.name == KnownService.ServiceName.Matchmaking || config.name == KnownService.ServiceName.MatchmakingServer)
            {
                if (config.subType == KnownService.ServiceName.Multiverse)
                {
                    path = path.Replace("{SubType}", "multiverse");
                }
                if (config.subType == KnownService.ServiceName.Sync)
                {
                    path = path.Replace("{SubType}", "sync");
                }
            }

            if (!String.IsNullOrEmpty(path))
            {
                Application.OpenURL(Domain + path);
            }
        }
    }
}