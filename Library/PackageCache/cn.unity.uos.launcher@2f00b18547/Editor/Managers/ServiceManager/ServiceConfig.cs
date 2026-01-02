using System;
using System.Collections.Generic;
using UnityEditor.PackageManager.UI;

namespace Unity.UOS.Launcher
{
    public enum ServiceStatus {
        None = 0,
        Enabled = 1, // 正常开启中
        Disabled = 2, // 异常暂停中
        Occupied = 3, // 已被其他服务占用
        PreconditionFailed = 4, // 前置条件不满足
        Upcoming = 5, // 待支持
    }

    [Serializable]
    public enum ServiceType
    {
        Service = 0,
        Library = 1
    }

    [Serializable]
    public class ServiceConfig
    {
        public string name;
        public string displayName;
        public string description;
        public string gitUrl;
        public string version; // 最新版本号
        public string updateNotificationLevel;
        public string homePage;
        public bool installed;
        public ServiceStatus status;
        public string statusReason;
        public string enableNotification;
        public string installedVersion;
        public string docUrl;
        public string[] dependencies;
        public string serviceType;
        public string subType; // 记录 matchmaking 实际开启类型
        public string versionUrl; // git 仓库中 package.json 链接
        public string devPortalPath; // UOS开发者门户配置链接
        public bool launched; // 是否已经发布
        public bool enableUpload;
        public string uploadNotification;
        public string forceUpdateVersion; // 强制更新版本
        public List<Sample> samples; // 示例项目列表
        public bool sampleIsImported; // 示例项目是否已经导入
        public string sampleUrl; // 示例项目站点链接
        public string enableServiceUrl; // 跳转到 Dev portal 开启应用的链接
        public bool internalTesting = false; // 内测项目
    }
}