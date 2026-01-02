using UnityEditor;
using UnityEditor.PackageManager;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.UOS.Launcher.UI;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

namespace Unity.UOS.Launcher
{
    public class PackageManager
    {
        private static ListRequest _request;
        private static async Task ConfirmRemove(ServiceConfig config)
        {
            string title = "Removing Package";
            string message = "Are you sure you want to remove this package?";
            string okButton = "Remove";
            string cancelButton = "Cancel";

            bool result = EditorUtility.DisplayDialog(title, message, okButton, cancelButton);

            if (result)
            {
                var request = Client.Remove(config.name);
                while (!request.IsCompleted)
                {
                    // 等待请求完成
                    await Task.Yield();
                }

                if (request.Status == StatusCode.Success)
                {
                    config.installed = false;
                }
                else
                {
                    // 添加失败
                    config.installed = true;
                }
            }
        }

        /// <summary>
        /// 安装或升级
        /// </summary>
        /// <param name="config">服务配置</param>
        public static async Task Install(ServiceConfig config)
        {
            if (config.name != KnownService.Launcher.name && PackageUpgradeManager.HasNew(KnownService.Launcher))
            {
                var title = "UOS Launcher 升级提示";
                var message = "请先升级 UOS Launcher 到最新版本！";
                var okButton = "升级";
                bool result = EditorUtility.DisplayDialog(title, message, okButton, "");
                if (result)
                {
                    Install(KnownService.Launcher);
                }
                return;
            }
            if (config.dependencies?.Length > 0)
            {
                foreach (var dependency in config.dependencies)
                {
                    if (LauncherUI.ServiceMap.TryGetValue(dependency, out var service))
                    {
                        await Install(service);
                    }
                }
            }
            
            var request = Client.Add($"{config.gitUrl}");
            while (!request.IsCompleted)
            {
                // 等待请求完成
                await Task.Yield();
            }
            
            var success = request.Status == StatusCode.Success;
            config.installed = success;
            if (!success)
            {
                Debug.LogError($"安装 {config.displayName} 失败。{request.Error.message}");
            }
            // 添加失败
            LauncherUI.DrawServiceList();
        }

        public static void Detail(ServiceConfig config)
        {
            UnityEditor.PackageManager.UI.Window.Open(config.name);
        }

        public static async Task Remove(object config)
        {
            await ConfirmRemove((ServiceConfig)config);
            LauncherUI.DrawServiceList();
        }
        
        public static async Task<Dictionary<string, UnityEditor.PackageManager.PackageInfo>> GetPackageMap()
        {
            // 获取 manifest.json 信息
            var packageListRequest = Client.List(true);
            while (!packageListRequest.IsCompleted)
            {
                // 等待请求完成
                await Task.Yield();
            }
            
            Dictionary<string, UnityEditor.PackageManager.PackageInfo> packageMap = new Dictionary<string, UnityEditor.PackageManager.PackageInfo>();
            if (packageListRequest.Status == StatusCode.Success)
            {
                foreach (var package in packageListRequest.Result)
                {
                    packageMap.Add(package.name, package);
                }
            }

            return packageMap;
        }
    }
}
