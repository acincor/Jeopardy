using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Unity.UOS.Launcher
{
    public class ServiceList
    {
        public static readonly List<ServiceConfig> List = new List<ServiceConfig>()
        {
            new ServiceConfig()
            {
                name = "cn.unity.uos.cdn",
                displayName = "CDN",
                description = "Asset Management & Delivery",
                gitUrl = "https://cnb.cool/unity/uos/UosCdnSDK.git",
                homePage = "https://uos.unity.cn/product/cdn",
                updateNotificationLevel = "NORMAL",
                docUrl = "https://uos.unity.cn/doc/cdn",
                versionUrl = "https://cnb.cool/unity/uos/UosCdnSDK/-/git/raw/master/package.json",
                devPortalPath = "/services/{Settings.AppID}/asset/bucket",
                enableNotification = "您正在开通 UOS CDN 服务， 服务提供免费 100GB 试用流量， 超出后将开启自动计费， 刊例价格如下：按流量计费： 0.15元/GB.",
                launched = true,
            },
            new ServiceConfig()
            {
                name = "cn.unity.uos.multiverse",
                displayName = "Multiverse",
                description = "Game Server Hosting",
                homePage = "https://uos.unity.cn/product/multiverse",
                gitUrl = "https://cnb.cool/unity/uos/multiverse-sdk.git?path=/unity",
                updateNotificationLevel = "NORMAL",
                docUrl = "https://uos.unity.cn/doc/multiverse",
                versionUrl =
                    "https://cnb.cool/unity/uos/multiverse-sdk/-/git/raw/main/unity/package.json",
                devPortalPath = "/services/{Settings.AppID}/multiverse/settings",
                launched = true,
                enableUpload = true,
                uploadNotification = "Build Multiverse Dedicated Server Image"
            },
            new ServiceConfig()
            {
                name = "cn.unity.uos.transport.kcp2k",
                displayName = "Kcp2k protocol",
                description = "Kcp2k protocol",
                gitUrl = "https://cnb.cool/unity/uos/Transport.git?path=/kcp2k",
                serviceType = ServiceType.Library.ToString(),
                launched = true,
            },
            new ServiceConfig()
            {
                name = "com.cysharp.unitask",
                displayName = "UniTask",
                description = "UniTask",
                gitUrl = "https://cnb.cool/unity/uos/UniTask.git?path=/src/UniTask/Assets/Plugins/UniTask",
                serviceType = ServiceType.Library.ToString(),
                launched = true,
            },
            new ServiceConfig()
            {
                name = "cn.unity.uos.sync.relay",
                displayName = "Sync - Relay",
                gitUrl = "https://cnb.cool/unity/uos/SyncRelaySDK.git",
                description = "Multiplayer Networking Solutions",
                homePage = "https://uos.unity.cn/product/sync",
                docUrl = "https://uos.unity.cn/doc/sync/relay-netcode#prepare",
                versionUrl = "https://cnb.cool/unity/uos/SyncRelaySDK/-/git/raw/master/package.json",
                devPortalPath = "/services/{Settings.AppID}/sync/profiles",
                launched = true,
                dependencies = new []{"cn.unity.uos.transport.kcp2k"},
            },
            new ServiceConfig()
            {
                name = "cn.unity.uos.sync.realtime",
                displayName = "Sync - Realtime",
                description = "Multiplayer Networking Solutions",
                homePage = "https://uos.unity.cn/product/sync",
                docUrl = "https://uos.unity.cn/doc/sync/realtime#prepare",
                devPortalPath = "/services/{Settings.AppID}/sync/profiles",
                launched = true,
                gitUrl = "https://cnb.cool/unity/uos/SyncRealtimeSDK.git",
                versionUrl = "https://cnb.cool/unity/uos/SyncRealtimeSDK/-/git/raw/master/package.json",
                dependencies = new []{"cn.unity.uos.transport.kcp2k"},
            },
            new ServiceConfig()
            {
                name = "com.google.external-dependency-manager",
                displayName = "EDM4U",
                description = "EDM4U",
                gitUrl = "https://cnb.cool/unity/uos/EDM4U.git",
                serviceType = ServiceType.Library.ToString(),
                launched = true,
            },
            new ServiceConfig()
            {
                name = "com.taptap.sdk.core",
                displayName = "TapTap Core",
                description = "TapTap Core Component",
                gitUrl = "https://cnb.cool/unity/uos/TapSDKCore.git",
                serviceType = ServiceType.Library.ToString(),
                launched = true,
            },
            new ServiceConfig()
            {
                name = "com.taptap.sdk.login",
                displayName = "TapTap Login",
                description = "TapTap Login Component",
                gitUrl = "https://cnb.cool/unity/uos/TapSDKLogin.git",
                serviceType = ServiceType.Library.ToString(),
                launched = true,
            }, 
            new ServiceConfig()
            {
                name = "cn.unity.uos.passport.core",
                displayName = "Passport Core",
                description = "Passport Core Component",
                gitUrl = "https://cnb.cool/unity/uos/PassportCore.git",
                serviceType = ServiceType.Library.ToString(),
                homePage = "https://uos.unity.cn/product/passport",
                docUrl = "https://uos.unity.cn/doc/passport",
                devPortalPath = "/services/{Settings.AppID}/passport",
                launched = true,
            },
            new ServiceConfig()
            {
                name = "cn.unity.uos.passport.login",
                displayName = "Passport Login",
                description = "Passport Player ID and Login Component",
                gitUrl = "https://cnb.cool/unity/uos/PassportLoginSDK.git",
                homePage = "https://uos.unity.cn/product/passport",
                docUrl = "https://uos.unity.cn/doc/passport/login#unityRegistry",
                devPortalPath = "/services/{Settings.AppID}/passport",
                dependencies = new []{"cn.unity.uos.passport.core"},
                launched = true,
                versionUrl = "https://cnb.cool/unity/uos/PassportLoginSDK/-/git/raw/main/package.json",
                forceUpdateVersion = "2.0.5",
                sampleUrl = "https://uos.unity.cn/doc/passport/tutorial"
            },
            new ServiceConfig()
            {
                name = "cn.unity.uos.passport.feature",
                displayName = "Passport Feature",
                description = "Passport Game Features",
                gitUrl = "https://cnb.cool/unity/uos/PassportFeatureSDK.git",
                homePage = "https://uos.unity.cn/product/passport",
                docUrl = "https://uos.unity.cn/doc/passport",
                devPortalPath = "/services/{Settings.AppID}/passport",
                dependencies = new []{"cn.unity.uos.passport.core"},
                launched = true,
                versionUrl = "https://cnb.cool/unity/uos/PassportFeatureSDK/-/git/raw/main/package.json",
                sampleUrl = "https://uos.unity.cn/doc/passport/tutorial"
            },
            new ServiceConfig()
            {
                name = "cn.unity.uos.func.stateful",
                displayName = "Func - Stateful",
                description = "Logic Server Hosting",
                gitUrl = "https://cnb.cool/unity/uos/FuncStatefulSDK.git",
                homePage = "https://uos.unity.cn/product/func",
                docUrl = "https://uos.unity.cn/doc/func/stateful#profile",
                versionUrl = "https://cnb.cool/unity/uos/FuncStatefulSDK/-/git/raw/main/package.json",
                devPortalPath = "/services/{Settings.AppID}/func/profiles",
                launched = true,
            },
            new ServiceConfig()
            {
                name = "cn.unity.uos.func.stateless",
                displayName = "Func - Stateless",
                description = "Cloud Function",
                homePage = "https://uos.unity.cn/product/func",
                docUrl = "https://uos.unity.cn/doc/func/stateless#function",
                devPortalPath = "/services/{Settings.AppID}/func/stateless",
                launched = true,
                gitUrl = "https://cnb.cool/unity/uos/FuncStatelessSDK.git",
                versionUrl = "https://cnb.cool/unity/uos/FuncStatelessSDK/-/git/raw/main/package.json"
            },
            new ServiceConfig()
            {
                name = "cn.unity.uos.crud.storage",
                displayName = "CRUD - Storage",
                description = "Various types of databases",
                homePage = "https://uos.unity.cn/product/crud",
                docUrl = "https://uos.unity.cn/doc/crud/storage",
                devPortalPath = "/services/{Settings.AppID}/crud/storage/management",
                launched = true,
            },
            new ServiceConfig()
            {
                name = "cn.unity.uos.cloudsave",
                displayName = "CRUD - Save",
                gitUrl = "https://cnb.cool/unity/uos/CloudSaveSDK.git",
                description = "Game Save and Player Data Storage",
                homePage = "https://uos.unity.cn/product/crud",
                docUrl = "https://uos.unity.cn/doc/crud/save",
                versionUrl =
                    "https://cnb.cool/unity/uos/CloudSaveSDK/-/git/raw/master/package.json",
                devPortalPath = "/services/{Settings.AppID}/crud/save",
                launched = true,
                forceUpdateVersion = "2.2.2"
            },
            new ServiceConfig()
            {
                name = "cn.unity.uos.matchmaking.server",
                displayName = "Matchmaking Server",
                description = "To Make Matches Accurately and Fast",
                gitUrl = "https://cnb.cool/unity/uos/matchmaking-server-sdk.git?path=/unity",
                updateNotificationLevel = "NORMAL",
                versionUrl = "https://cnb.cool/unity/uos/matchmaking-server-sdk/-/git/raw/master/unity/package.json",
                docUrl = "https://uos.unity.cn/doc/multiverse/match-server-sdk",
                devPortalPath = "/services/{Settings.AppID}/{SubType}/match",
                launched = true,
            },
            new ServiceConfig()
            {
                name = "cn.unity.uos.matchmaking",
                displayName = "Matchmaking Client",
                description = "To Make Matches Accurately and Fast",
                gitUrl = "https://cnb.cool/unity/uos/MatchmakingSDK.git",
                updateNotificationLevel = "NORMAL",
                docUrl = "https://uos.unity.cn/doc/multiverse/match-conception",
                versionUrl = "https://cnb.cool/unity/uos/MatchmakingSDK/-/git/raw/master/package.json",
                devPortalPath = "/services/{Settings.AppID}/{SubType}/match",
                launched = true,
            },
            new ServiceConfig()
            {
                name = "cn.unity.uos.config",
                displayName = "Remote Config",
                description = "Update Live Game Content Remotely",
                gitUrl = "https://cnb.cool/unity/uos/RemoteConfigSDK.git",
                updateNotificationLevel = "NORMAL",
                docUrl = "https://uos.unity.cn/doc/remote-config#concept",
                versionUrl =
                    "https://cnb.cool/unity/uos/RemoteConfigSDK/-/git/raw/master/package.json",
                devPortalPath = "/services/{Settings.AppID}/config#config",
                launched = true,
                forceUpdateVersion = "1.1.2"
            },
            new ServiceConfig()
            {
                name = "cn.unity.uos.hello",
                displayName = "Hello",
                description = "Real-Time Communication",
                homePage = "https://uos.unity.cn/product/hello",
                launched = true,
                docUrl = "https://uos.unity.cn/doc/hello",
                gitUrl = "https://cnb.cool/unity/uos/HelloSdk.git",
                versionUrl =
                    "https://cnb.cool/unity/uos/HelloSdk/-/git/raw/master/package.json",
                devPortalPath = "/services/{Settings.AppID}/hello",
                enableServiceUrl = "https://uos.unity.cn/services/{Settings.AppID}",
            },
            new ServiceConfig()
            {
                name = "cn.unity.uos.push",
                displayName = "Push",
                description = "Real-time notifications and chat service",
                homePage = "https://uos.unity.cn/product/push",
                launched = true,
                docUrl = "https://uos.unity.cn/doc/push",
                gitUrl = "https://cnb.cool/unity/uos/PushSDK.git",
                versionUrl =
                    "https://cnb.cool/unity/uos/PushSDK/-/git/raw/master/package.json",
                devPortalPath = "/services/{Settings.AppID}/push",
                dependencies = new []{"cn.unity.uos.transport.kcp2k"},
            },
            new ServiceConfig()
            {
                name = "cn.unity.uos.push.message.storage",
                displayName = "Push Storage",
                description = "Persistent message storage service",
                homePage = "https://uos.unity.cn/product/push",
                launched = true,
                docUrl = "https://uos.unity.cn/doc/push",
                gitUrl = "https://cnb.cool/unity/uos/PushMessageStorage.git",
                versionUrl =
                    "https://cnb.cool/unity/uos/PushMessageStorage/-/git/raw/master/package.json",
                devPortalPath = "/services/{Settings.AppID}/push",
                dependencies = new []{"cn.unity.uos.push", "com.cysharp.unitask"},
            },
            new ServiceConfig()
            {
                name = "cn.unity.uos.safe",
                displayName = "Safe",
                description = "Game Package Protection",
                homePage = "https://uos.unity.cn/product/safe",
                launched = true,
                docUrl = "https://uos.unity.cn/doc/safe",
                gitUrl = "https://cnb.cool/unity/uos/SafeSDK.git",
                versionUrl =
                    "https://cnb.cool/unity/uos/SafeSDK/-/git/raw/master/package.json",
                devPortalPath = "/services/{Settings.AppID}/safe",
            },
            new ServiceConfig()
            {
                name = "cn.unity.uos.upr",
                displayName = "UPR",
                description = "Performance Optimization",
                homePage = "https://upr.unity.cn",
                launched = true,
            },
            new ServiceConfig()
            {
                name = "cn.unity.uos.device",
                displayName = "Device",
                gitUrl = "https://cnb.cool/unity/uos/DeviceSDK.git",
                description = "Real Device Testing",
                homePage = "https://device.unity.cn",
                docUrl = "https://uos.unity.cn/doc/device/client-sdk",
                devPortalPath = "/services/{Settings.AppID}/device/packages",
                versionUrl = "https://cnb.cool/unity/uos/DeviceSDK/-/git/raw/master/package.json",
                launched = true,
            },
            new ServiceConfig()
            {
                name = "cn.unity.uos.stacktrace",
                displayName = "Stacktrace",
                gitUrl = "https://cnb.cool/unity/uos/StacktraceSDK.git",
                description = "App Crash Stacktrace Retrieval",
                homePage = "https://uos.unity.cn/product/stacktrace",
                docUrl = "https://uos.unity.cn/doc/stacktrace/client-sdk",
                devPortalPath = "/services/{Settings.AppID}/stacktrace/record",
                versionUrl = "https://cnb.cool/unity/uos/StacktraceSDK/-/git/raw/master/package.json",
                launched = true,
            },
            new ServiceConfig()
            {
                name = "cn.unity.uos.baking",
                displayName = "Baking",
                gitUrl = "https://cnb.cool/unity/uos/UOSBaking.git",
                description = "Cloud Distributed Baking",
                homePage = "https://uos.unity.cn/product/baking",
                docUrl = "https://uos.unity.cn/doc/baking/",
                devPortalPath = "/services/{Settings.AppID}/baking",
                versionUrl = "https://cnb.cool/unity/uos/UOSBaking/-/git/raw/main/package.json",
                launched = true,
            },
            new ServiceConfig()
            {
                name = "cn.unity.uos.metrics",
                displayName = "Metrics",
                gitUrl = "https://cnb.cool/unity/uos/MetricsSDK.git",
                description = "UOS Metrics",
                // homePage = "https://uos.unity.cn/product/metrics",
                // docUrl = "https://uos.unity.cn/doc/metrics/",
                devPortalPath = "/services/{Settings.AppID}/metrics",
                versionUrl = "https://cnb.cool/unity/uos/MetricsSDK/-/git/raw/master/package.json",
                launched = true,
                internalTesting = true,
                enableServiceUrl = "https://uos.unity.cn/contact-us",
                statusReason = "此服务默认关闭，请点击后「联系我们」以获取体验资格"
            },
            new ServiceConfig()
            {
                name = "cn.unity.uos.wind",
                displayName = "Wind",
                gitUrl = "https://cnb.cool/unity/uos/WindSDK.git",
                description = "UOS Wind",
                devPortalPath = "/services/{Settings.AppID}/wind",
                versionUrl = "https://cnb.cool/unity/uos/WindSDK/-/git/raw/master/package.json",
                launched = true,
                enableServiceUrl = "https://uos.unity.cn",
                statusReason = "请至UOS开发者平台开启此服务"
            }
        };

        public static async Task InitLatestVersion()
        {
            foreach (var service in List)
            {
                await PackageUpgradeManager.GetLatestVersion(service);
            }
        }
    }
}