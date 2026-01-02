using System;

namespace Unity.UOS.Networking
{
    public static class Endpoints
    {
        public static readonly Uri UosEndpoint = new Uri("https://uos.unity.cn");
        public static readonly Uri PassportEndpoint = new Uri("https://p.unity.cn");
        public static readonly Uri CdnEndpoint = new Uri("https://a.unity.cn");
        public static readonly Uri MultiverseEndpoint = new Uri("https://multiverse.scaling.unity.cn");
        public static readonly Uri MatchmakingEndpoint = new Uri("https://m.unity.cn");
        public static readonly Uri SyncEndpoint = new Uri("https://s.unity.cn");
        public static readonly Uri FuncStatefulEndpoint = new Uri("https://f.unity.cn");
        public static readonly Uri FuncStatelessEndpoint = new Uri("https://f.unity.cn");
        public static readonly Uri CrudEndpoint = new Uri("https://crud.unity.cn");
        public static readonly Uri CloudSaveEndpoint = new Uri("https://save.unity.cn");
        public static readonly Uri RemoteConfigEndpoint = new Uri("https://c.unity.cn");
        public static readonly Uri HelloEndpoint = new Uri("https://hello.unity.cn");
        public static readonly Uri WebsocketSecureEndpoint = new Uri("wss://wsp.unity.cn:443");
        public static readonly Uri StacktraceEndpoint = new Uri("https://st.unity.cn");
        public static readonly Uri PushEndpoint = new Uri("https://push.unity.cn");
        public static readonly Uri DeviceEndpoint = new Uri("https://device.unity.cn");
        public static readonly Uri SafeEndpoint = new Uri("https://safe.unity.cn");
        public static readonly Uri MetricsEndpoint = new Uri("https://metrics.unity.cn");
        public static readonly Uri BakingEndpoint = new Uri("https://baking.unity.cn");
    }
}
