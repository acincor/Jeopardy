using System;
using UnityEngine;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEditor;
using System.Linq;

namespace Unity.UOS.Launcher
{
    public class PackageUpgradeManager
    {
        /// <summary>
        /// 查询并更新服务的最新版本信息
        /// </summary>
        /// <param name="config"></param>
        public static async Task GetLatestVersion(ServiceConfig config)
        {
            try
            {
                if (!string.IsNullOrEmpty(config.versionUrl))
                {
                    // 获取最新版本
                    var gitPackageInfo = await API.GetPackageInfo(config.versionUrl);
                    config.version = gitPackageInfo.version;
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
        }

        /// <summary>
        /// 是否有更新的版本
        /// </summary>
        /// <param name="config">服务配置</param>
        /// <returns></returns>
        public static bool HasNew(ServiceConfig config)
        {
            return CompareVersions(config.version, config.installedVersion) > 0;
        }

        /// <summary>
        /// 对比 SemVer 版本号新旧
        /// </summary>
        /// <param name="version1"></param>
        /// <param name="version2"></param>
        /// <returns>0-相等；大于0:version1新于version2; 小于0:version2新于version1</returns>
        private static int CompareVersions(string version1, string version2)
        {
            if (string.IsNullOrEmpty(version1) || string.IsNullOrEmpty(version2)) return 0;

            var v1 = new Version(version1);
            var v2 = new Version(version2);
            return v1.CompareTo(v2);
        }

        /// <summary>
        /// 校验强制升级
        /// </summary>
        public static async void CheckForceUpdate()
        {
            var needUpgradeServiceList = await GetForceUpdateServiceList();
            if (needUpgradeServiceList.Any())
            {
                string title = "SDK 升级提示";
                string okButton = "升级";
                var serviceNames = new List<string>();
                foreach (var config in needUpgradeServiceList)
                {
                    serviceNames.Add(config.displayName);
                }
                string message = $"检测到当前安装的服务 {String.Join("、", serviceNames)} 已过时，可能会导致部分功能不可用。请点击按钮升级到最新版本。";
                bool result = EditorUtility.DisplayDialog(title, message, okButton, "");
                
                if (result)
                {
                    foreach (var config in needUpgradeServiceList)
                    {
                        await PackageManager.Install(config);
                    }
                }
            }
        }

        /// <summary>
        /// 获取需要强制升级的服务列表
        /// </summary>
        /// <returns></returns>
        private static async Task<List<ServiceConfig>> GetForceUpdateServiceList()
        {
            var list = ServiceList.List;
            var needUpgradeServiceList = new List<ServiceConfig>();
            foreach (var config in list)
            {
                if (RequireForceUpdate(config))
                {
                    needUpgradeServiceList.Add(config);
                }
            }
            return needUpgradeServiceList;
        }
        
        /// <summary>
        /// 校验服务是否需要强制升级
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        private static bool RequireForceUpdate(ServiceConfig config)
        {
            if (string.IsNullOrEmpty(config.forceUpdateVersion) || string.IsNullOrEmpty(config.installedVersion)) return false;
            return CompareVersions(config.forceUpdateVersion, config.installedVersion) > 0;
        }
    }
}