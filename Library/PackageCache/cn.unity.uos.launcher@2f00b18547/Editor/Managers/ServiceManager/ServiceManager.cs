using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System;
using System.Linq;
using Unity.UOS.Auth;
using Unity.UOS.Common;
using Unity.UOS.Launcher.UI;
using UnityEditor;
using Enum = System.Enum;

namespace Unity.UOS.Launcher
{
    /// <summary>
    /// 远程服务列表管理、服务状态管理
    /// </summary>
    public class ServiceManager
    {
        private const string OccupiedReason =
            "当前 UOS App 已启用 {0} 服务，该服务与 {1} 服务冲突。如需单独使用 {1} 服务，请创建一个新的 UOS App。";

        private const string PreconditionFailedReason = "当前服务基于 {0} 服务实现。如需使用 {1} 服务，请先开启 {0} 服务。";
        private const string DisabledReason = "当前服务免费试用额度已用完，服务已被停用。请及时联系 UOS 团队购买正式服务。";
        private const string UpcomingReason = "请在 UOS 官网[https://uos.unity.cn/] 开启服务。";
        public static Dictionary<string, ServiceConfig> ServiceConfigs = new Dictionary<string, ServiceConfig>();
        private static AppInfo _appInfo;
        
        private static List<string> _mutualExclusiveGroup = new List<string>()
        {
            KnownService.ServiceName.SyncRealtime,
            KnownService.ServiceName.SyncRelay,
            KnownService.ServiceName.Push,
            KnownService.ServiceName.FuncStateful,
            // multiverse 在最后（multiverse 可能是因为其他服务开启的）
            KnownService.ServiceName.Multiverse,
        };

        /// <summary>
        /// 获取远程服务列表JSON配置
        /// </summary>
        /// <returns></returns>
        public static async Task<(List<ServiceConfig>, List<ServiceConfig>, List<ServiceConfig>)> GetServiceList()
        {
            List<ServiceConfig> upcomingList = new List<ServiceConfig>();
            List<ServiceConfig> serviceList = new List<ServiceConfig>();
            List<ServiceConfig> libraryList = new List<ServiceConfig>();
            
            var getPackageMapTask = PackageManager.GetPackageMap();
            await Task.WhenAll(getPackageMapTask);
            var packageMap = await getPackageMapTask;
            var services = ServiceList.List;
            
            // 分类
            foreach (var service in services)
            {
                if (Enum.TryParse<ServiceType>(service.serviceType, true, out var serviceType))
                {
                    switch (serviceType)
                    {
                        case ServiceType.Library:
                            libraryList.Add(service);
                            break;
                        default:
                            if (service.launched)
                            {
                                serviceList.Add(service);
                            }
                            else
                            {
                                upcomingList.Add(service);
                            }

                            break;
                    }
                }
                else
                {
                    if (service.launched)
                    {
                        serviceList.Add(service);
                    }
                    else
                    {
                        upcomingList.Add(service);
                    }
                }

                // 检查是否已经安装
                service.installed = false;
                if (packageMap.TryGetValue(service.name, out var packageInfo))
                {
                    service.installed = true;
                    service.installedVersion = packageInfo.version;
                    // 已安装的 Package 获取 sample 信息
                    PackageSampleManager.GetSamples(service);
                }
            }
            
            // 写入 Func Realtime 版本信息
            GetServiceVersion(KnownService.FuncRealtime, packageMap);
            // 写入 Launcher 版本信息
            GetServiceVersion(KnownService.Launcher, packageMap);

            return (upcomingList, serviceList, libraryList);
        }

        private static void GetServiceVersion(ServiceConfig service, Dictionary<string, UnityEditor.PackageManager.PackageInfo> packageMap)
        {
            if (!string.IsNullOrEmpty(service.installedVersion)) return;
            // 获取版本信息
            if (packageMap.TryGetValue(service.name, out var info))
            {
                if (!string.IsNullOrEmpty(info.version))
                {
                    service.installed = true;
                    service.installedVersion = info.version;
                }
            }
        }

        /// <summary>
        /// 生成所有服务映射表
        /// </summary>
        /// <returns></returns>
        private static Dictionary<string, ServiceConfig> GenServiceConfigs()
        {
            Dictionary<string, ServiceConfig> map = new Dictionary<string, ServiceConfig>();
            // 生成所有服务的列表
            foreach (var config in ServiceList.List)
            {
                map.Add(config.name, new ServiceConfig()
                {
                    name = config.name,
                    displayName = config.displayName
                });
            }

            return map;
        }

        /// <summary>
        /// 将 AppInfo 中的服务状态写入，初始状态
        /// </summary>
        /// <param name="serviceConfigs">服务配置映射表</param>
        /// <param name="appInfo">app 信息</param>
        private static void SetServiceInitialStatus(Dictionary<string, ServiceConfig> serviceConfigs, AppInfo appInfo)
        {
            foreach (var kvp in appInfo.Services)
            {
                // 获取服务名称
                if (KnownService.ServiceMap.TryGetValue(kvp.Key, out string service))
                {
                    if (service == KnownService.ServiceName.Sync)
                    {
                        // sync 根据 Properties.type 来区分 relay 和 realtime
                        if (kvp.Value.Properties != null && kvp.Value.Properties.TryGetValue("type", out var type))
                        {
                            KnownService.ServiceMap.TryGetValue(type, out service);
                        }
                        else
                        {
                            // 默认没有 type properties 认为是 realtime
                            KnownService.ServiceMap.TryGetValue("realtime", out service);
                        }
                    }
                    
                    if (!string.IsNullOrEmpty(service) && serviceConfigs.TryGetValue(service, out var config))
                    {
                        // 找到配置项
                        if (Enum.TryParse(kvp.Value.Status, out ServiceStatus status))
                        {
                            config.status = status;
                            // 解析服务状态，并写入对应配置项
                            if (status == ServiceStatus.Disabled)
                            {
                                config.statusReason = String.Format(DisabledReason);
                            }
                            
                            // 处理 matchmaking 特殊逻辑
                            if (service == KnownService.ServiceName.Matchmaking)
                            {
                                // matchmaking 关闭时，不是因为欠费
                                if (status == ServiceStatus.Disabled)
                                {
                                    config.status = ServiceStatus.None;
                                    config.statusReason = "";
                                }
                            }
                        }
                    }
                }
                // else
                // {
                //     Debug.LogWarning("service not found: " + kvp.Key);
                // }
            }
        }

        /// <summary>
        /// 检查前置条件和限制条件是否满足，并更新服务状态
        /// </summary>
        /// <param name="serviceConfigs">服务配置映射表</param>
        /// <param name="appInfo">app 信息</param>
        private static void CheckServiceStatus(Dictionary<string, ServiceConfig> serviceConfigs, AppInfo appInfo)
        {
            // 检查前置条件和限制条件是否满足
            foreach (var kvp in serviceConfigs)
            {
                var config = kvp.Value;

                // 写入依赖先后顺序
                if (config.status == ServiceStatus.Enabled)
                {
                    switch (kvp.Key)
                    {
                        case KnownService.ServiceName.Matchmaking:
                            SetMatchmakingSubType(config);
                            break;
                    }
                }
                else if (config.status == ServiceStatus.None)
                {
                    switch (kvp.Key)
                    {
                        case KnownService.ServiceName.Matchmaking:
                            config.statusReason =
                                String.Format(PreconditionFailedReason, "Multiverse 或 Sync", "MatchMaking");
                            config.status = ServiceStatus.PreconditionFailed;
                            // 开且仅开了 multiverse
                            if (OnlyMultiverseEnabled())
                            {
                                config.statusReason = "";
                                config.status = ServiceStatus.None;
                            }

                            // 开启了 sync
                            if (SyncEnabled())
                            {
                                config.statusReason = "";
                                config.status = ServiceStatus.None;
                            }
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// 开启且仅开启了 Multiverse
        /// </summary>
        /// <returns></returns>
        private static bool OnlyMultiverseEnabled()
        {
            return ServiceConfigs[KnownService.ServiceName.Multiverse].status == ServiceStatus.Enabled &&
                   ServiceConfigs[KnownService.ServiceName.FuncStateful].status != ServiceStatus.Enabled &&
                   ServiceConfigs[KnownService.ServiceName.SyncRealtime].status != ServiceStatus.Enabled &&
                   ServiceConfigs[KnownService.ServiceName.SyncRelay].status != ServiceStatus.Enabled &&
                   ServiceConfigs[KnownService.ServiceName.Push].status != ServiceStatus.Enabled;
        }

        /// <summary>
        /// 设置互斥服务的状态
        /// </summary>
        private static void SetMutualExclusiveServiceStatus()
        {
            var enabledService = GetEnabledServiceInMutualExclusiveGroup();
            if (enabledService == null) return;

            foreach (var serviceName in _mutualExclusiveGroup)
            {
                var config = ServiceConfigs[serviceName];
                var enabled = enabledService.name == serviceName;
                if (!enabled)
                {
                    // 已开启多服务时，如果服务本身已经开启，不再修改其状态
                    if (_appInfo.SupportMultiServices && config.status == ServiceStatus.Enabled) continue;
                    config.status = ServiceStatus.Occupied;
                    config.statusReason = String.Format(OccupiedReason, enabledService.displayName, config.displayName);
                }
            }
        }

        /// <summary>
        /// 获取互斥服务组中，实际开通的服务名称
        /// </summary>
        /// <returns></returns>
        private static ServiceConfig GetEnabledServiceInMutualExclusiveGroup()
        {
            foreach (var serviceName in _mutualExclusiveGroup)
            {
                var config = ServiceConfigs[serviceName];
                if (config.status == ServiceStatus.Enabled)
                {
                    return config;
                }
            }

            return null;
        }

        /// <summary>
        /// 开启了 Sync
        /// </summary>
        /// <returns></returns>
        private static bool SyncEnabled()
        {
            return ServiceConfigs[KnownService.ServiceName.SyncRelay].status == ServiceStatus.Enabled ||
                   ServiceConfigs[KnownService.ServiceName.SyncRealtime].status == ServiceStatus.Enabled;
        }

        /// <summary>
        /// 根据 AppInfo 设置更新服务配置
        /// </summary>
        /// <param name="appInfo">查询到的 App 信息</param>
        public static Dictionary<string, ServiceConfig> GetServiceMapWithStatus(AppInfo appInfo)
        {
            _appInfo = appInfo;
            ServiceConfigs = GenServiceConfigs();;
            SetServiceInitialStatus(ServiceConfigs, appInfo);
            SetMutualExclusiveServiceStatus();
            CheckServiceStatus(ServiceConfigs, appInfo);
            SetMatchmakingServerStatus();
            SetLinkedServiceStatus();
            CheckFuncRealtimeStatus(ServiceConfigs, appInfo);
            return ServiceConfigs;
        }

        /// <summary>
        /// 检查是否添加 Func Realtime 服务
        /// </summary>
        /// <param name="serviceConfigs"></param>
        /// <param name="appInfo"></param>
        private static void CheckFuncRealtimeStatus(Dictionary<string, ServiceConfig> serviceConfigs, AppInfo appInfo)
        {
            var funcKey = "func";
            var metaverseFuncKey = "metaverseFunc";
            var enableMetaverseFunc = false;
            if (appInfo.Services.TryGetValue(funcKey, out var funcConfig))
            {
                if (funcConfig.Properties.TryGetValue(metaverseFuncKey, out var enabled))
                {
                    if (enabled == "true")
                    {
                        enableMetaverseFunc = true;
                    }
                }
            }
            if (enableMetaverseFunc)
            {
#if UNITY_2021_3_OR_NEWER
                serviceConfigs.TryAdd(KnownService.FuncRealtime.name, KnownService.FuncRealtime);
#endif
            }
            else
            {
                serviceConfigs.Remove(KnownService.FuncRealtime.name);
            }
        }

        /// <summary>
        /// 设置关联服务的服务状态，如PassportLogin和PassportFeature
        /// </summary>
        private static void SetLinkedServiceStatus()
        {
            foreach (var linked in KnownService.LinkedService)
            {
                if (ServiceConfigs.TryGetValue(linked.Key, out var config))
                {
                    foreach (var service in linked.Value)
                    {
                        if (ServiceConfigs.TryGetValue(service, out var linkedConfig))
                        {
                            linkedConfig.status = config.status;
                            linkedConfig.statusReason = config.statusReason;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 设置 matchmaking.server 与 matchmaking 状态相同
        /// </summary>
        private static void SetMatchmakingServerStatus()
        {
            if (ServiceConfigs.TryGetValue(KnownService.ServiceName.MatchmakingServer,
                    out var matchmakingServerConfig) &&
                ServiceConfigs.TryGetValue(KnownService.ServiceName.Matchmaking, out var matchmakingConfig))
            {
                matchmakingServerConfig.status = matchmakingConfig.status;
                matchmakingServerConfig.statusReason = matchmakingConfig.statusReason;
                matchmakingServerConfig.subType = matchmakingConfig.subType;
            }
        }

        /// <summary>
        /// 设置 matchmaking 开启类型：Multiverse、Sync Relay、Sync Realtime
        /// </summary>
        /// <param name="config"></param>
        private static void SetMatchmakingSubType(ServiceConfig config)
        {
            // 开且仅开了 multiverse
            if (OnlyMultiverseEnabled())
            {
                config.subType = KnownService.ServiceName.Multiverse;
            }
            // 开启了 sync
            else if (SyncEnabled())
            {
                config.subType = KnownService.ServiceName.Sync;
            }
        }

        /// <summary>
        /// 开启 Passport 服务
        /// </summary>
        private static async Task EnablePassport()
        {
            await API.EnableService(new EnablePassportRequest()
            {
                EnablePassport = true
            });
            await API.CreateIdDomain(new CreateIdDomainRequest()
            {
                idDomain = new IdDomain()
                {
                    uosAppID = Settings.AppID
                }
            });
            await API.CreatePassportConfig();
        }

        /// <summary>
        /// 在当前 app 下开启服务
        /// </summary>
        /// <param name="service">服务基本信息</param>
        /// <returns>开启结果</returns>
        public static async Task EnableService(ServiceConfig service)
        {
            if (string.IsNullOrEmpty(Settings.AppID) || string.IsNullOrEmpty(Settings.AppServiceSecret))
            {
                string title = "Fail to enable service";
                string message = "Please fill in the UOS AppID & Secret to \"Link APP\" first. \n If you haven't got a UOS APP, you can create one by clicking \"Create UOS APP\".";
                string okButton = "Ok";
                string cancelButton = "Cancel";
                string altButton = "Create UOS APP";

                var result = EditorUtility.DisplayDialogComplex(title, message, okButton, cancelButton, altButton);
                if (result == 2)
                {
                    Application.OpenURL("https://uos.unity.cn/apps");
                }
                
                return;
            }
            if (!string.IsNullOrEmpty(service.enableNotification))
            {
                string title = "Notification";
                string message = service.enableNotification;
                string okButton = "Ok";
                string cancelButton = "Cancel";

                var result = EditorUtility.DisplayDialog(title, message, okButton, cancelButton);
                if (!result)
                {
                    return;
                }
            }

            if (!string.IsNullOrEmpty(service.enableServiceUrl))
            {
                Application.OpenURL(service.enableServiceUrl.Replace("{Settings.AppID}", Settings.AppID));
            }

            switch (service.name)
            {
                case KnownService.ServiceName.Matchmaking:
                case KnownService.ServiceName.MatchmakingServer:
                    if (ServiceConfigs[KnownService.ServiceName.SyncRelay].status == ServiceStatus.Enabled ||
                        ServiceConfigs[KnownService.ServiceName.SyncRealtime].status == ServiceStatus.Enabled)
                    {
                        // 如果 sync relay 已开启，则是启用 sync relay 下的 match making
                        await API.EnableService(new EnableMatchmakingSyncRequest()
                        {
                            EnableMatchmakingSync = true
                        });
                        await API.CreateMatchApp(new CreateMatchAppRequest()
                        {
                            appType = "sync",
                            appName = _appInfo.ProjectName
                        });
                    }
                    else if (ServiceConfigs[KnownService.ServiceName.Multiverse].status == ServiceStatus.Enabled)
                    {
                        // 若 sync relay 未开启，且 multiverse 已开启，则启用 multiverse 下的 match making
                        await API.EnableService(new EnableMatchmakingMvRequest()
                        {
                            EnableMatchmakingMv = true,
                            EnableDedicatedRoom = true
                        });
                        await API.CreateMatchApp(new CreateMatchAppRequest()
                        {
                            appType = "multiverse",
                            appName = _appInfo.ProjectName
                        });
                    }
                    
                    SetMatchmakingSubType(service);
                    break;
                case KnownService.ServiceName.RemoteConfig:
                    await API.EnableService(new EnableRemoteConfigRequest()
                    {
                        EnableRemoteConfig = true
                    });
                    break;
                case KnownService.ServiceName.SyncRelay:
                    await API.EnableService(new EnableUsyncRelayRequest()
                    {
                        EnableUsyncRelay = true
                    });
                    // 刷新 Sync cache
                    try
                    {
                        RefreshAuthCache.RefreshSync();
                    }
                    catch (Exception e)
                    {
                        Debug.Log(e.Message);
                    }
                    await API.InitSync();
                    break;
                case KnownService.ServiceName.SyncRealtime:
                    await API.EnableService(new EnableUsyncRealtimeRequest()
                    {
                        EnableUsyncRealtime = true
                    });
                    // 刷新 Sync cache
                    try
                    {
                        RefreshAuthCache.RefreshSync();
                    }
                    catch (Exception e)
                    {
                        Debug.Log(e.Message);
                    }
                    await API.InitSync();
                    break;
                case KnownService.ServiceName.Cdn:
                    await API.EnableService(new EnableAssetStreamingRequest()
                    {
                        EnableAssetStreaming = true
                    });
                    // 刷新 CDN cache
                    try
                    {
                        RefreshAuthCache.RefreshCdn();
                    }
                    catch (Exception e)
                    {
                        Debug.Log(e.Message);
                    }
                    break;
                case KnownService.ServiceName.PassportLogin:
                    await EnablePassport();
                    break;
                case KnownService.ServiceName.PassportFeature:
                    await EnablePassport();
                    break;
                case KnownService.ServiceName.Save:
                    await API.EnableService(new EnableSaveRequest()
                    {
                        EnableSave = true
                    });
                    // 刷新 cache
                    try
                    {
                        RefreshAuthCache.RefreshSave();
                    }
                    catch (Exception e)
                    {
                        Debug.Log(e.Message);
                    }
                    break;
                case KnownService.ServiceName.Multiverse:
                    await API.EnableService(new EnableMultiverseRequest()
                    {
                        EnableMultiverse = true
                    });
                    // 刷新 multiverse cache
                    try
                    {
                        RefreshAuthCache.RefreshMultiverse();
                    }
                    catch (Exception e)
                    {
                        Debug.Log(e.Message);
                    }
                    await API.CreateMultiverseApp(new CreateMultiverseGameRequest()
                    {
                        osId = 1,
                        allocationTTL = "20m",
                        gameType = "OnDemandGame",
                        gameName = _appInfo.ProjectName,
                        orgId = _appInfo.OrganizationId,
                    });
                    break;
                case KnownService.ServiceName.FuncStateful:
                    var fieldName = KnownService.ServiceMap.First((kvp) => kvp.Value == KnownService.ServiceName.FuncStateful).Key;
                    var funcStatefulEnableServiceResp = await API.EnableService(new EnableFuncStatefulRequest()
                    {
                        EnableFuncStateful = true
                    });
                    var uosAppId = Settings.AppID;
                    if (funcStatefulEnableServiceResp.SupportMultiServices)
                    {
                        // 开启了多服务模式则使用 ServiceId
                        if (funcStatefulEnableServiceResp.Services.TryGetValue(fieldName, out var funcStatefulService))
                        {
                            if (string.IsNullOrEmpty(funcStatefulService.ServiceId))
                            {
                                throw new Exception("获取 Func Id 失败！");
                            }

                            uosAppId = funcStatefulService.ServiceId;
                        }
                        else
                        {
                            throw new Exception("获取 Func Id 失败！");
                        }
                    }
                    await API.CreateFuncStatefulApp(new CreateFuncStatefulAppRequest()
                    {
                        osId = 7,
                        endpointSelectMethod = "DEFAULT",
                        type = "stateful",
                        name = _appInfo.ProjectName,
                        orgId = _appInfo.OrganizationId,
                        uosAppId = uosAppId
                    });

                    break;
                case KnownService.ServiceName.FuncStateless:
                    await API.EnableService(new EnableFuncStatelessRequest()
                    {
                        EnableFuncStateless = true
                    });
                    await API.CreateFuncStatelessApp(new CreateFuncStatelessAppRequest()
                    {
                        name = _appInfo.ProjectName,
                        orgId = _appInfo.OrganizationId,
                        uosAppId = Settings.AppID,
                        runtime = "Dotnet"
                    });
                    break;
                case KnownService.ServiceName.Stacktrace:
                    await API.EnableService(new EnableStacktraceRequest()
                    {
                        EnableStacktrace = true
                    });
                    await API.CreateStacktraceApp(new CreateStacktraceAppRequest()
                    {
                        uosAppId = Settings.AppID,
                        projectId = _appInfo.ProjectId
                    });
                    break;
                case KnownService.ServiceName.Push:
                case KnownService.ServiceName.PushStorage:
                    await API.EnableService(new EnablePushRequest()
                    {
                        EnablePush = true
                    });
                    await API.CreatePushApp(new CreatePushAppRequest()
                    {
                        name = _appInfo.ProjectName,
                        orgId = _appInfo.OrganizationId,
                    });
                    break;
                case KnownService.ServiceName.Device:
                    await API.EnableService(new EnableDeviceRequest()
                    {
                        EnableDevice = true
                    });
                    break;
                case KnownService.ServiceName.Safe:
                    await API.EnableService(new EnableSafeRequest()
                    {
                        EnableSafe = true
                    });
                    await API.CreateSafeApp(new CreateSafeAppRequest() { });
                    break;
            }

            EditorUtility.DisplayDialog("Warning", "Enabling a new service will clear the logged in Passport credentials", "OK");
            AuthTokenManager.Initialize(Settings.AppID, Settings.AppSecret);
            AuthTokenManager.ClearToken();
            RefreshAuthCache.RefreshPassport();

            await LinkAppInMainUI.LinkApp();
        }
    }
}