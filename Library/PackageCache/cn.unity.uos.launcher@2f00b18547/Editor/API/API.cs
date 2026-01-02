using UnityEngine.Networking;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Unity.UOS.Common;
using Unity.UOS.Networking;

namespace Unity.UOS.Launcher
{
    public class API
    {
        public static Dictionary<string, string> NoCacheHeaders = new Dictionary<string, string>()
        {
            { "x-cache-control", "no-cache" }
        };
        private static HttpClient Client(string appID, string appServiceSecret, Uri baseUrl = null)
        {
            var authenticator = new HttpBasicAuthenticator(appID, appServiceSecret);
            var client = new HttpClient(new HttpClientOptions()
            {
                SdkName = "",
                SdkVersion = "",
                Authenticator = authenticator,
                BaseUrl = baseUrl == null ? Endpoints.UosEndpoint: baseUrl,
            });
            return client;
        }
        public static async Task<AppInfo> GetUosApp(string appID, string appServiceSecret)
        {
            var client = Client(appID, appServiceSecret);
            var url = "/service/app";
            var appInfo = await client.Request<AppInfo>(url, "GET");
            // 将服务状态写入到服务列表中
            return appInfo;
        }

        public static async Task<AppInfo> UpdateAppInfo(string appID, string appServiceSecret, string data)
        {
            var client = Client(appID, appServiceSecret);
            var url = $"/service/app/{appID}";
            var appInfo = await client.Request<AppInfo>(url, "PUT", data);
            return appInfo;
        }
        
        /// <summary>
        /// 开启服务
        /// </summary>
        /// <param name="config"></param>
        /// <param name="req"></param>
        /// <returns></returns>
        public static async Task<EnableServiceResponse> EnableService(EnableServiceRequest req)
        {
            var client = Client(Settings.AppID, Settings.AppServiceSecret);

            var url = $"/service/app/{Settings.AppID}/enable-service";
            return await client.Request<EnableServiceResponse>(url, "POST", JsonConvert.SerializeObject(req));
        }
        
        /// <summary>
        /// 开启 passport 服务之创建 IdDomain
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public static async Task<EnableServiceResponse> CreateIdDomain(CreateIdDomainRequest req)
        {
            var client = Client(Settings.AppID, Settings.AppServiceSecret, Endpoints.PassportEndpoint);
            var url = $"/v1/id-domains";
            return await client.Request<EnableServiceResponse>(url, "POST", JsonConvert.SerializeObject(req), null, NoCacheHeaders);
        }

        public static async Task<CreatePassportConfigResponse> CreatePassportConfig()
        {
            var client = Client(Settings.AppID, Settings.AppServiceSecret, Endpoints.PassportEndpoint);
            var url = $"/v1/config";
            return await client.Request<CreatePassportConfigResponse>(url, "POST", "", null, NoCacheHeaders);
        }
        
        /// <summary>
        /// 开启 sync 服务
        /// </summary>
        /// <returns></returns>
        public static async Task<EnableServiceResponse> InitSync()
        {
            var client = Client(Settings.AppID, Settings.AppServiceSecret, Endpoints.SyncEndpoint);

            var url = $"/api/app/{Settings.AppID}/usync-init";
            return await client.Request<EnableServiceResponse>(url, "POST", "{}");
        }
        
        /// <summary>
        /// 开启 matchmaking
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public static async Task<CreateMatchAppResponse> CreateMatchApp(CreateMatchAppRequest req)
        {
            var client = Client(Settings.AppID, Settings.AppServiceSecret, Endpoints.MatchmakingEndpoint);
            var url = "/v1/apps";
            return await client.Request<CreateMatchAppResponse>(url, "POST", JsonConvert.SerializeObject(req), null, NoCacheHeaders);
        }
        
        /// <summary>
        /// 开启 multiverse
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public static async Task<CreateMultiverseGameResponse> CreateMultiverseApp(CreateMultiverseGameRequest req)
        {
            var client = Client(Settings.AppID, Settings.AppServiceSecret, Endpoints.MultiverseEndpoint);

            var url = "/v1/game/games";
            return await client.Request<CreateMultiverseGameResponse>(url, "POST", JsonConvert.SerializeObject(req));
        }

        /// <summary>
        /// 开启 Func Stateful
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public static async Task<CreateFuncStatefulAppResponse> CreateFuncStatefulApp(CreateFuncStatefulAppRequest req)
        {
            var client = Client(Settings.AppID, Settings.AppServiceSecret, Endpoints.FuncStatefulEndpoint);

            var url = "/v1/func/functions";
            return await client.Request<CreateFuncStatefulAppResponse>(url, "POST", JsonConvert.SerializeObject(req), null, NoCacheHeaders);
        }
        
        /// <summary>
        /// 开启 Func Stateless
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public static async Task<CreateFuncStatelessAppResponse> CreateFuncStatelessApp(CreateFuncStatelessAppRequest req)
        {
            var client = Client(Settings.AppID, Settings.AppServiceSecret, Endpoints.FuncStatefulEndpoint);

            var url = "/v1/func-stateless/functions";
            return await client.Request<CreateFuncStatelessAppResponse>(url, "POST", JsonConvert.SerializeObject(req), null, NoCacheHeaders);
        }
        
        /// <summary>
        /// 开启 Stacktrace
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public static async Task<CreateStacktraceAppResponse> CreateStacktraceApp(CreateStacktraceAppRequest req)
        {
            var client = Client(Settings.AppID, Settings.AppServiceSecret, Endpoints.StacktraceEndpoint);

            var url = "/v1/stacktrace/app-initialize";
            return await client.Request<CreateStacktraceAppResponse>(url, "POST", JsonConvert.SerializeObject(req), null, NoCacheHeaders);
        }
        
        /// <summary>
        /// 开启 Push
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public static async Task<CreatePushAppResponse> CreatePushApp(CreatePushAppRequest req)
        {
            var client = Client(Settings.AppID, Settings.AppServiceSecret, Endpoints.PushEndpoint);

            var url = "/v1/apps";
            return await client.Request<CreatePushAppResponse>(url, "POST", JsonConvert.SerializeObject(req), null, NoCacheHeaders);
        }
        
                
        /// <summary>
        /// 开启 Safe
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        public static async Task<CreateSafeAppResponse> CreateSafeApp(CreateSafeAppRequest req)
        {
            var client = Client(Settings.AppID, Settings.AppServiceSecret, Endpoints.SafeEndpoint);

            var url = $"/v1/safe/{Settings.AppID}/enable";
            return await client.Request<CreateSafeAppResponse>(url, "POST", JsonConvert.SerializeObject(req), null, NoCacheHeaders);
        }

        
        /// <summary>
        /// 获取 git 仓库 package.json 信息
        /// </summary>
        /// <param name="url">git仓库链接</param>
        /// <returns></returns>
        public static async Task<GitPackageInfo> GetPackageInfo(string url)
        {
            var result = new GitPackageInfo();
            using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
            {
                var sendOperation = webRequest.SendWebRequest();
                while (!sendOperation.isDone)
                {
                    await Task.Yield();
                }

                try
                {
                    if (webRequest.result == UnityWebRequest.Result.Success)
                    {
                        string jsonData = webRequest.downloadHandler.text;
                        if (!String.IsNullOrEmpty(jsonData))
                        {
                            result = JsonUtility.FromJson<GitPackageInfo>(jsonData);
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError(e.Message);
                }
            }

            return result;
        }

        public static async Task<GetImageTransferTokenResponse> GetImageToken()
        {
            var client = Client(Settings.AppID, Settings.AppServiceSecret, Endpoints.MultiverseEndpoint);
            var url = $"/v1/game/games/{Settings.AppID}/get-image-transfer-token";
            return await client.Request<GetImageTransferTokenResponse>(url, "GET");
        }

        public static async Task<CreateImageResponse> CreateImage(string objectUrl, string imageName, string objectName)
        {
            var client = Client(Settings.AppID, Settings.AppServiceSecret, Endpoints.MultiverseEndpoint);
            var url = "/v1/game/images";
            var request = new CreateImageRequest()
            {
                gameId = Settings.AppID,
                objectUrl = objectUrl,
                imageTagPrefix = imageName,
                objectName = objectName,
                gamePackageCompressedFormat = "zip"
            };
            return await client.Request<CreateImageResponse>(url, "POST", JsonConvert.SerializeObject(request));
        }

        public static async Task<GetImageResponse> GetImage(string imageId)
        {
            var client = Client(Settings.AppID, Settings.AppServiceSecret, Endpoints.MultiverseEndpoint);
            var url = "/v1/game/images/" + imageId;
            return await client.Request<GetImageResponse>(url, "GET");
        }
    }
}