using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.UOS.Launcher
{
    public class EnableServiceRequest
    {
    }

    public class EnableRemoteConfigRequest: EnableServiceRequest
    {
        public bool EnableRemoteConfig;
    }

    public class EnablePassportRequest : EnableServiceRequest
    {
        public bool EnablePassport;
    }

    public class EnableMatchmakingMvRequest : EnableServiceRequest
    {
        public bool EnableMatchmakingMv;
        public bool EnableDedicatedRoom;
    }

    public class EnableAssetStreamingRequest : EnableServiceRequest
    {
        public bool EnableAssetStreaming;
    }

    public class EnableUsyncRelayRequest : EnableServiceRequest
    {
        public bool EnableUsyncRelay;
    }

    public class EnableUsyncRealtimeRequest : EnableServiceRequest
    {
        public bool EnableUsyncRealtime;
    }

    public class EnableMultiverseRequest : EnableServiceRequest
    {
        public bool EnableMultiverse;
    }

    public class EnableMatchmakingSyncRequest : EnableServiceRequest
    {
        public bool EnableMatchmakingSync;
    }

    public class EnableFuncStatefulRequest : EnableServiceRequest
    {
        public bool EnableFuncStateful;
    }

    public class EnableFuncStatelessRequest : EnableServiceRequest
    {
        public bool EnableFuncStateless;
    }

    public class EnableSaveRequest : EnableServiceRequest
    {
        public bool EnableSave;
    }
    
    public class EnableStacktraceRequest : EnableServiceRequest
    {
        public bool EnableStacktrace;
    }
    
    public class EnableDeviceRequest : EnableServiceRequest
    {
        public bool EnableDevice;
    }

    public class EnableServiceResponse
    {
        public bool SupportMultiServices;
        public Dictionary<string, ServiceData> Services;
    }

    public class EnablePushRequest : EnableServiceRequest
    {
        public bool EnablePush;
    }
    
    // 开启 safe 服务
    public class EnableSafeRequest : EnableServiceRequest
    {
        public bool EnableSafe;
    }
    
    [System.Serializable]
    public class ServiceData
    {
        public string ServiceName;
        public string Provider;
        public string Status;
        public string ServiceId;
    }
}