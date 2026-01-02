using System.Collections.Generic;
using System.Reflection;

namespace Unity.UOS.Launcher
{
    public class KnownService
    {
        public static readonly ServiceConfig Launcher = new ServiceConfig()
        {
            name = "cn.unity.uos.launcher",
            gitUrl = "https://cnb.cool/unity/uos/UOSLauncher.git",
            versionUrl = "https://cnb.cool/unity/uos/UOSLauncher/-/git/raw/master/package.json",
            forceUpdateVersion = "2.0.5"
        };
        
        public static readonly ServiceConfig FuncRealtime = new ServiceConfig()
        {
            name = "cn.unity.uos.func.realtime",
            gitUrl = "https://cnb.cool/unity/uos/FuncRealtimeSDK.git",
            versionUrl = "https://cnb.cool/unity/uos/FuncRealtimeSDK/-/git/raw/master/package.json",
            displayName = "Func - Realtime",
            description = "For the metaverse scenarios",
            launched = true,
            status = ServiceStatus.Enabled,
            devPortalPath = "/services/{Settings.AppID}/func/profiles",
        };
        
        public static class ServiceName
        {
            public const string RemoteConfig = "cn.unity.uos.config";
            public const string Matchmaking = "cn.unity.uos.matchmaking";
            public const string Cdn = "cn.unity.uos.cdn";
            public const string Multiverse = "cn.unity.uos.multiverse";
            public const string SyncRealtime = "cn.unity.uos.sync.realtime";
            public const string SyncRelay = "cn.unity.uos.sync.relay";
            public const string Sync = "cn.unity.uos.sync";
            public const string Passport = "cn.unity.uos.passport"; // for icon 
            public const string PassportLogin = "cn.unity.uos.passport.login";
            public const string PassportFeature = "cn.unity.uos.passport.feature";
            public const string FuncStateful = "cn.unity.uos.func.stateful";
            public const string Crud = "cn.unity.uos.crud";
            public const string Safe = "cn.unity.uos.safe";
            public const string Upr = "cn.unity.uos.upr";
            public const string Device = "cn.unity.uos.device";
            public const string FuncStateless = "cn.unity.uos.func.stateless";
            public const string Save = "cn.unity.uos.cloudsave";
            public const string MatchmakingServer = "cn.unity.uos.matchmaking.server";
            public const string Hello = "cn.unity.uos.hello";
            public const string Stacktrace = "cn.unity.uos.stacktrace";
            public const string Push = "cn.unity.uos.push";
            public const string Baking = "cn.unity.uos.baking";
            public const string Metrics = "cn.unity.uos.metrics";
            public const string Wind = "cn.unity.uos.wind";
            public const string PushStorage = "cn.unity.uos.push.message.storage";
        }

        private static List<string> _serviceList;

        public static List<string> ServiceList
        {
            get
            {
                _serviceList ??= GetServiceList();
                
                return _serviceList;
            }
        }

        private static List<string> GetServiceList()
        {
            List<string> serviceList = new List<string>();
            FieldInfo[] fields = typeof(ServiceName).GetFields(BindingFlags.Public | BindingFlags.Static);
            foreach (var field in fields)
            {
                object value = field.GetValue(null);
                serviceList.Add(value.ToString());
            }

            return serviceList;
        }

        // 服务开启状态字段映射
        public static Dictionary<string, string> ServiceMap = new Dictionary<string, string>()
        {
            {
                "remote-config", ServiceName.RemoteConfig
            },
            {
                "matchmaking", ServiceName.Matchmaking
            },
            {
                "asset-streaming", ServiceName.Cdn
            },
            {
                "multiverse", ServiceName.Multiverse
            },
            {
                "passport", ServiceName.PassportLogin
            },
            {
                "func", ServiceName.FuncStateful
            },
            {
                "func-stateless", ServiceName.FuncStateless
            },
            {
                "storage", ServiceName.Crud
            },
            {
                "usync", ServiceName.Sync
            },
            {
                "relay", ServiceName.SyncRelay
            },
            {
                "realtime", ServiceName.SyncRealtime
            },
            {
                "save", ServiceName.Save
            },
            {
                "hello", ServiceName.Hello
            },
            {
                "stacktrace", ServiceName.Stacktrace
            },
            {
                "push", ServiceName.Push
            },
            {
                "device", ServiceName.Device
            },
            {
                "safe", ServiceName.Safe
            },
            {
                "baking", ServiceName.Baking
            },
            {
                "metrics", ServiceName.Metrics
            },
            {
                "wind", ServiceName.Wind
            }
        };

        public static Dictionary<string, string[]> LinkedService = new Dictionary<string, string[]>()
        {
            {
                ServiceName.PassportLogin, new[] { ServiceName.PassportFeature }
            },
            {
                ServiceName.Matchmaking, new[] { ServiceName.MatchmakingServer }
            },
            {
                ServiceName.Push, new[] { ServiceName.PushStorage }
            }
        };
    }
}