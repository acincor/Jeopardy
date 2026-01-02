using System.Collections.Generic;
using System;

namespace Unity.UOS.Launcher
{
    public class Config
    {
        public List<ServiceConfig> Services;
    }

    [Serializable]
    public class AppInfo
    {
        [Serializable]
        public class RemoteServiceConfig
        {
            public string ServiceName;
            public string Provider;
            public string Status;
            public Dictionary<string, string> Properties;
        }

        public string ProjectId;
        public string ProjectName;
        public string OrganizationId;
        public Dictionary<string, RemoteServiceConfig> Services;
        public Dictionary<string, string> Properties;
        public bool SupportMultiServices;
    }

    [Serializable]
    public class IdDomain
    {
        public string uosAppID;
        public string name = "";
        public bool forcePhoneBinding = false;
        public bool forceRealName = true;
        public bool allowMultiLogin = true;
        public string iconUrl = "";
    }

    [Serializable]
    public class CreateIdDomainRequest
    {
        public IdDomain idDomain;
    }

    [Serializable]
    public class CreatePassportConfigResponse
    {
        
    }

    [Serializable]
    public class CreateMatchAppRequest
    {
        public string appName;
        public string appType;
    }

    [Serializable]
    public class CreateMultiverseGameRequest
    {
        public string allocationTTL;
        public int osId;
        public string gameName;
        public string gameType;
        public string orgId;
    }

    [Serializable]
    public class CreateMatchAppResponse
    {
    }

    [Serializable]
    public class CreateMultiverseGameResponse
    {
    }

    public class CreateFuncStatefulAppRequest
    {
        public string endpointSelectMethod;
        public int osId;
        public string name;
        public string orgId;
        public string type;
        public string uosAppId;
    }
    
    public class CreateFuncStatelessAppRequest
    {
        public string name;
        public string orgId;
        public string runtime;
        public string uosAppId;
    }
    
    public class CreateStacktraceAppRequest
    {
        public string uosAppId;
        public string projectId;
    }
    
    public class CreatePushAppRequest
    {
        public string name;
        public string orgId;
    }
    
    public class CreateSafeAppRequest
    {
    }

    public class CreateSafeAppResponse
    {
    }
    
    [Serializable]
    public class CreateFuncStatefulAppResponse
    {
    }
    
    [Serializable]
    public class CreateFuncStatelessAppResponse
    {
    }
    
    [Serializable]
    public class CreateStacktraceAppResponse
    {
    }
    
    [Serializable]
    public class CreatePushAppResponse
    {
    }
    
    [Serializable]
    public class GitPackageInfoResponseDate
    {
        public GitPackageInfoResponseFile file;
    }
    
    [Serializable]
    public class GitPackageInfoResponseFile
    {
        public string data;
    }

    [Serializable]
    public class GitPackageInfoResponse
    {
        public GitPackageInfoResponseDate data;
    }

    [Serializable]
    public class GitPackageInfo
    {
        public string name;
        public string displayName;
        public string description;
        public string version;
        public string unity;
        public string licensesUrl;
    }
    
    [Serializable]
    public class GetImageTransferTokenResponse 
    {
        public string token;
        public string tmpSecretId;
        public string tmpSecretKey;
        public string objectName;
        public string region;
        public string bucket;
        public int startTime;
        public string expiredTime;
    }

    [Serializable]
    public class CreateImageRequest
    {
        public string gameId;
        public string objectUrl;
        public string imageTagPrefix;
        public string gamePackageCompressedFormat;
        public string objectName;
    }

    [Serializable]
    public class CreateImageResponse
    {
        public GameImage gameImage;
    }
    
    [Serializable]
    public class GetImageResponse
    {
        public GameImage gameImage;
    }
    
    [Serializable]
    public class GameImage
    {
        public string imageId;
        public string gameId;
        public string imageTag;
        public string imageStatus;
        public string description;
        public string createdByUser;
        public string imageUrl;
        public string dockerfile;
        public List<string> executableFileList;
        public string osName;
        public string gameImageObjectName;
    }
    
    [Serializable]
    public class UpdateAppInfo
    {
        public Dictionary<string, string> Properties;
    }

    [Serializable]
    public class RefreshTokenResponse
    {
        
    }
}