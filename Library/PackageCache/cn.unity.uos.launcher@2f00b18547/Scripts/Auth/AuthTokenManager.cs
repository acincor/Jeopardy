using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Unity.UOS.Common;
using Unity.UOS.Common.UOSLauncher.Scripts.Auth;
using Unity.UOS.Networking;
using Unity.UOS.Security;
using UnityEngine;
using ErrorCode = Unity.UOS.Common.UOSLauncher.Scripts.Auth.ErrorCode;

namespace Unity.UOS.Auth
{
    /// <summary>
    /// AuthTokenManager用于管理与AuthToken(Passport/NonPassport JWT)相关的操作
    /// </summary>
    public class AuthTokenManager : GenericSingleton<AuthTokenManager>
    {
        private static readonly TimeSpan RefreshGracePeriod = TimeSpan.FromMinutes(10);
        private const string IdDomainPublicInfoPath = "/v1/sdk/id-domains/{0}/public-info";
        private const string ExternalLoginPath = "/v1/login/external";
        private const string GenerateTokenPath = "/v1/login/token";
        private const string PassportRefreshPath = "/v1/sdk/personas/refresh-token";
        private const string NonPassportRefreshPath = "/v1/login/refresh-token";

        private const string IdDomainNotFoundError = "未找到Id Domain";

        private static readonly Mutex InitMutex = new Mutex();
        // Refresh Token action mutex object.
        private static readonly Mutex RefreshMutex = new Mutex();
        private static bool _refreshing = false;

        private const string SdkName = "Auth-Token-Manager";
        private const string SdkVersion = "2.7.5";

        public string CurrentUosAppID { get; set; }
        private readonly Dictionary<string, AppSetting> _appSettings = new Dictionary<string, AppSetting>();

        /// <summary>
        /// 初始化AuthTokenManager. 所有需要用到JWT来进行Auth的SDK都需要在初始化自己的时候调用该方法.
        /// 该方法会自动检测Passport的开启状态后进行初始化.
        /// AuthTokenManager会使用GenerateToken的方式来生成AuthToken.
        /// 该方法可以重复调用, 适用于一个程序需要集成多个UosApp的情况, 多个UosApp的信息可以在AuthTokenManager中同时存在.
        /// </summary>
        /// <param name="uosAppID">Uos App ID</param>
        /// <param name="uosAppSecret">Uos App Secret</param>
        public static void Initialize(string uosAppID, string uosAppSecret)
        {
            Initialize(new InitOptions(uosAppID, uosAppSecret));
        }
        
        /// <summary>
        /// 初始化AuthTokenManager. 所有需要用到JWT来进行Auth的SDK都需要在初始化自己的时候调用该方法.
        /// 该方法会自动检测Passport的开启状态后进行初始化.
        /// AuthTokenManager会使用GenerateToken的方式来生成AuthToken.
        /// 该方法可以重复调用, 适用于一个程序需要集成多个UosApp的情况, 多个UosApp的信息可以在AuthTokenManager中同时存在.
        /// </summary>
        /// <param name="options">初始化选项, 包括UosAppID, UosAppSecret, HttpClientTimeout</param>
        public static void Initialize(InitOptions options)
        {
            InitMutex.WaitOne();
            try
            {
                if (Instance._appSettings.TryGetValue(options.UosAppID, out var appSetting))
                {
                    appSetting.UosAppID = options.UosAppID;
                    if (appSetting.UosAppSecret != options.UosAppSecret)
                    {
                        appSetting.UosAppSecret = options.UosAppSecret;
                        appSetting.TokenInfo = null;
                        appSetting.HttpClient = new HttpClient(new HttpClientOptions
                        {
                            SdkName = SdkName,
                            SdkVersion = SdkVersion,
                            BaseUrl = Endpoints.PassportEndpoint,
                            Authenticator = new UosNonceAuthenticator(options.UosAppID, options.UosAppSecret),
                            ExceptionHandler = ExceptionUtil.ExceptionHandler,
                            Timeout = options.HttpClientTimeout,
                        });
                    }
                    appSetting.TokenPersistKey = $"passport:current_token:{options.UosAppID}";
                }
                else
                {
                    appSetting = new AppSetting
                    {
                        UosAppID = options.UosAppID,
                        UosAppSecret = options.UosAppSecret,
                        TokenPersistKey = $"passport:current_token:{options.UosAppID}",
                        HttpClient = new HttpClient(new HttpClientOptions
                        {
                            SdkName = SdkName,
                            SdkVersion = SdkVersion,
                            BaseUrl = Endpoints.PassportEndpoint,
                            Authenticator = new UosNonceAuthenticator(options.UosAppID, options.UosAppSecret),
                            ExceptionHandler = ExceptionUtil.ExceptionHandler,
                            Timeout = options.HttpClientTimeout,
                        })
                    };
                    Instance._appSettings[options.UosAppID] = appSetting;
                }
                Instance.CurrentUosAppID = options.UosAppID;
            }
            finally
            {
                InitMutex.ReleaseMutex();
            }
        }

        /// <summary>
        /// 初始化AuthTokenManager. 所有需要用到JWT来进行Auth的SDK都需要在初始化自己的时候调用该方法.
        /// 该方法会自动检测Passport的开启状态后进行初始化.
        /// AuthTokenManager会使用GenerateToken的方式来生成AuthToken.
        /// 该方法可以重复调用, 适用于一个程序需要集成多个UosApp的情况, 多个UosApp的信息可以在AuthTokenManager中同时存在.
        /// </summary>
        /// <param name="uosAppID">Uos App ID</param>
        /// <param name="uosAppSecret">Uos App Secret</param>
        [Obsolete("InitializeAsync已弃用，请使用Initialize(string uosAppID, string uosAppSecret)")]
        public static async Task InitializeAsync(string uosAppID, string uosAppSecret)
        {
            Initialize(uosAppID, uosAppSecret);
        }

        /// <summary>
        /// 初始化AuthTokenManager. 所有需要用到JWT来进行Auth的SDK都需要在初始化自己的时候调用该方法
        /// 如果某个SDK不需要使用Passport的Identity系统(如RemoteConfig和Matchmaking), 可以对userPassportIdentity传入False,
        /// AuthTokenManager会使用GenerateToken的方式来生成AuthToken.
        /// 该方法可以重复调用, 适用于一个程序需要集成多个UosApp的情况, 多个UosApp的信息可以在AuthTokenManager中同时存在.
        /// </summary>
        /// <param name="uosAppID">Uos App ID</param>
        /// <param name="uosAppSecret">Uos App Secret</param>
        /// <param name="usePassportIdentity">是否使用Passport的Identity系统</param>
        [Obsolete("Initialize已弃用，请使用Initialize(string uosAppID, string uosAppSecret)")]
        public static void Initialize(string uosAppID, string uosAppSecret, bool usePassportIdentity)
        {
            Initialize(uosAppID, uosAppSecret);
        }

        /// <summary>
        /// 在外部获得AuthToken后, 将AuthToken存入AuthTokenManager并持久化
        /// 外部获取AuthToken的方式:
        /// 1. Passport Login SDK 登录流程获取AuthToken.
        /// 2. Passport External Login用第三方ID获取AuthToken
        /// 注: 仅当一个程序需要集成多个UosApp的时候使用. 如只有单一UosApp, 请使用<see cref="SaveToken(Unity.UOS.Auth.TokenInfo)"/>
        /// </summary>
        /// <param name="uosAppID">要保存Token的UosAppID</param>
        /// <param name="token">authToken(包括AccessToken和RefreshToken)</param>
        /// <exception cref="AuthException">ErrorCode.NotInitialized: AuthTokenManager未经初始化</exception>
        public static void SaveToken(string uosAppID, TokenInfo token)
        {
            if (!Instance._appSettings.ContainsKey(uosAppID))
            {
                throw new AuthException(ErrorCode.InvalidUosAppID, "无效的UosAppID");
            }

            var currentAppSetting = Instance._appSettings[uosAppID];
            currentAppSetting.TokenInfo = token;
            var tokenInfoStr = JsonConvert.SerializeObject(currentAppSetting.TokenInfo);
            PersistManager.SetString(currentAppSetting.TokenPersistKey, tokenInfoStr);
        }

        /// <summary>
        /// 在外部获得AuthToken后, 将AuthToken存入AuthTokenManager并持久化
        /// 外部获取AuthToken的方式:
        /// 1. Passport Login SDK 登录流程获取AuthToken.
        /// 2. Passport External Login用第三方ID获取AuthToken
        /// </summary>
        /// <param name="token">authToken(包括AccessToken和RefreshToken)</param>
        /// <exception cref="AuthException">ErrorCode.NotInitialized: AuthTokenManager未经初始化</exception>
        public static void SaveToken(TokenInfo token)
        {
            if (string.IsNullOrEmpty(Instance.CurrentUosAppID))
            {
                throw new AuthException(ErrorCode.NotInitialized, "AuthTokenManager未经初始化");
            }

            SaveToken(Instance.CurrentUosAppID, token);
        }

        /// <summary>
        /// 清除AuthTokenManager中当前UosAppID的AuthToken, 一般在登出后调用.
        /// </summary>
        public static void ClearToken()
        {
            ClearToken(Instance.CurrentUosAppID);
        }

        /// <summary>
        /// 清除AuthTokenManager中指定UosAppID的AuthToken, 一般在登出后调用.
        /// </summary>
        public static void ClearToken(string uosAppID)
        {
            if (Instance._appSettings.ContainsKey(uosAppID))
            {
                var currentAppSetting = Instance._appSettings[uosAppID];
                currentAppSetting.TokenInfo = null;
                PersistManager.DeleteKey(currentAppSetting.TokenPersistKey);
            }
            else
            {
                Debug.Log($"UOS App {uosAppID} not found, ClearToken ignored.");
            }
        }

        /// <summary>
        /// 获取Manager中存储的AccessToken详细信息
        /// 该方法按照以下顺序来获取AccessToken
        /// 1. 内存中的AccessToken
        /// 2. PersistManager中的AccessToken
        /// 如获取到的AccessToken已过期, 会通过RefreshToken获取新的AccessToken
        /// 当检测到当前的AccessToken即将过期时, 会启动coroutine来通过RefreshToken获取新的AccessToken
        /// 注: 仅当一个程序需要集成多个UosApp的时候使用. 如只有单一UosApp, 请使用<see cref="GetTokenInfo()"/>
        /// </summary>
        /// <param name="uosAppID">要获取AccessToken的UosAppID</param>
        /// <returns>当前的AccessToken详细信息</returns>
        /// <exception cref="AuthException">ErrorCode.NotInitialized: AuthTokenManager未经初始化</exception>
        /// <exception cref="AuthException">ErrorCode.NeedLogin: 需要重新登录以获取新的AccessToken/RefreshToken</exception>
        /// <exception cref="AuthException">ErrorCode.RefreshFailed: 调用RefreshToken失败</exception>
        public static async Task<TokenInfo> GetTokenInfo(string uosAppID)
        {
            if (!Instance._appSettings.ContainsKey(uosAppID))
            {
                throw new AuthException(ErrorCode.InvalidUosAppID, "无效的UosAppID");
            }

            var currentAppSetting = Instance._appSettings[uosAppID];
            if (currentAppSetting.TokenInfo == null)
            {
                var tokenInfoStr = PersistManager.GetString(currentAppSetting.TokenPersistKey, "");
                if (string.IsNullOrEmpty(tokenInfoStr))
                {
                    throw new AuthException(ErrorCode.NeedLogin, "需要重新登录以获取新的AccessToken/RefreshToken");
                }

                currentAppSetting.TokenInfo = JsonConvert.DeserializeObject<TokenInfo>(tokenInfoStr);
            }

            var jwt = JsonWebToken.Decode(currentAppSetting.TokenInfo.AccessToken);
            var now = DateTime.UtcNow;
            if (now > jwt.Expiration)
            {
                RefreshMutex.WaitOne();
                try
                {
                    await AsyncRefreshAccessToken(uosAppID);
                }
                finally
                {
                    RefreshMutex.ReleaseMutex();
                }
            }

            if (now > jwt.Expiration - RefreshGracePeriod && !_refreshing)
            {
                // Only refresh the token if it gets the lock. Otherwise, return directly.
                if (RefreshMutex.WaitOne(0, true))
                {
                    try
                    {
                        if (!_refreshing)
                        {
                            Instance.StartCoroutine(RefreshAccessToken(uosAppID));
                        }
                    }
                    finally
                    {
                        RefreshMutex.ReleaseMutex();
                    }
                }
            }

            return currentAppSetting.TokenInfo;
        }

        /// <summary>
        /// 获取Manager中存储的AccessToken详细信息
        /// 该方法按照以下顺序来获取AccessToken
        /// 1. 内存中的AccessToken
        /// 2. PersistManager中的AccessToken
        /// 如获取到的AccessToken已过期, 会通过RefreshToken获取新的AccessToken
        /// 当检测到当前的AccessToken即将过期时, 会启动coroutine来通过RefreshToken获取新的AccessToken
        /// </summary>
        /// <returns>当前的AccessToken详细信息</returns>
        /// <exception cref="AuthException">ErrorCode.NotInitialized: AuthTokenManager未经初始化</exception>
        /// <exception cref="AuthException">ErrorCode.NeedLogin: 需要重新登录以获取新的AccessToken/RefreshToken</exception>
        /// <exception cref="AuthException">ErrorCode.RefreshFailed: 调用RefreshToken失败</exception>
        public static async Task<TokenInfo> GetTokenInfo()
        {
            if (string.IsNullOrEmpty(Instance.CurrentUosAppID))
            {
                throw new AuthException(ErrorCode.NotInitialized, "AuthTokenManager未经初始化");
            }

            return await GetTokenInfo(Instance.CurrentUosAppID);
        }

        /// <summary>
        /// 获取Manager中存储的AccessToken
        /// 该方法按照以下顺序来获取AccessToken
        /// 1. 内存中的AccessToken
        /// 2. PersistManager中的AccessToken
        /// 如获取到的AccessToken已过期, 会通过RefreshToken获取新的AccessToken
        /// 当检测到当前的AccessToken即将过期时, 会启动coroutine来通过RefreshToken获取新的AccessToken
        /// 注: 仅当一个程序需要集成多个UosApp的时候使用. 如只有单一UosApp, 请使用<see cref="GetAccessToken()"/>
        /// </summary>
        /// <param name="uosAppID">要获取AccessToken的UosAppID</param>
        /// <returns>当前的AccessToken</returns>
        /// <exception cref="AuthException">ErrorCode.NotInitialized: AuthTokenManager未经初始化</exception>
        /// <exception cref="AuthException">ErrorCode.NeedLogin: 需要重新登录以获取新的AccessToken/RefreshToken</exception>
        /// <exception cref="AuthException">ErrorCode.RefreshFailed: 调用RefreshToken失败</exception>
        public static async Task<string> GetAccessToken(string uosAppID)
        {
            var tokenInfo = await GetTokenInfo(uosAppID);
            return tokenInfo.AccessToken;
        }

        /// <summary>
        /// 获取Manager中存储的AccessToken
        /// 该方法按照以下顺序来获取AccessToken
        /// 1. 内存中的AccessToken
        /// 2. PersistManager中的AccessToken
        /// 如获取到的AccessToken已过期, 会通过RefreshToken获取新的AccessToken
        /// 当检测到当前的AccessToken即将过期时, 会启动coroutine来通过RefreshToken获取新的AccessToken
        /// </summary>
        /// <returns>当前的AccessToken</returns>
        /// <exception cref="AuthException">ErrorCode.NotInitialized: AuthTokenManager未经初始化</exception>
        /// <exception cref="AuthException">ErrorCode.NeedLogin: 需要重新登录以获取新的AccessToken/RefreshToken</exception>
        /// <exception cref="AuthException">ErrorCode.RefreshFailed: 调用RefreshToken失败</exception>
        public static async Task<string> GetAccessToken()
        {
            if (string.IsNullOrEmpty(Instance.CurrentUosAppID))
            {
                throw new AuthException(ErrorCode.NotInitialized, "AuthTokenManager未经初始化");
            }

            return await GetAccessToken(Instance.CurrentUosAppID);
        }

        /// <summary>
        /// 使用外部UserID获取AccessToken
        /// </summary>
        /// <param name="userID">外部UserID</param>
        /// <param name="personaID">外部PersonaID(可选)</param>
        /// <param name="displayName">外部用户昵称</param>
        /// <param name="realmID">服务器ID</param>
        /// <param name="channel">渠道</param>
        /// <exception cref="AuthException">ErrorCode.NotInitialized: AuthTokenManager未经初始化</exception>
        /// <exception cref="AuthException">ErrorCode.InvalidUosAppID: 无效的UosAppID</exception>
        /// <exception cref="AuthException">ErrorCode.InvalidUosAppID: 无效的UosAppID</exception>
        /// <exception cref="AuthException">ErrorCode.InvalidOperation: 未开启Passport服务,无法执行ExternalLogin</exception>
        /// <exception cref="AuthException">ErrorCode.NonceTimestampExpired: Nonce时间戳已过期，请检查机器时间是否同步</exception>
        public static async Task<TokenInfo> ExternalLogin(string userID, string personaID = null, string displayName = null, string  realmID = null, string channel = null)
        {
            if (string.IsNullOrEmpty(Instance.CurrentUosAppID))
            {
                throw new AuthException(ErrorCode.NotInitialized, "AuthTokenManager未经初始化");
            }
            return await ExternalLoginWithAppId(Instance.CurrentUosAppID, userID, personaID, displayName, realmID, channel);
        }

        /// <summary>
        /// 使用外部UserID获取AccessToken
        /// </summary>
        /// <param name="uosAppID">UosAppID</param>
        /// <param name="userID">外部UserID</param>
        /// <param name="personaID">外部PersonaID(可选)</param>
        /// <param name="displayName">外部用户昵称</param>
        /// <param name="realmID">服务器ID</param>
        /// <param name="channel">渠道</param>
        /// <exception cref="AuthException">ErrorCode.NotInitialized: AuthTokenManager未经初始化</exception>
        /// <exception cref="AuthException">ErrorCode.InvalidUosAppID: 无效的UosAppID</exception>
        /// <exception cref="AuthException">ErrorCode.InvalidUosAppID: 无效的UosAppID</exception>
        /// <exception cref="AuthException">ErrorCode.InvalidOperation: 未开启Passport服务,无法执行ExternalLogin</exception>
        /// <exception cref="AuthException">ErrorCode.NonceTimestampExpired: Nonce时间戳已过期，请检查机器时间是否同步</exception>
        public static async Task<TokenInfo> ExternalLoginWithAppId(string uosAppID, string userID, string personaID, string displayName = null, string realmID = null, string channel = null)
        {
            var request = new ExternalLoginRequest
            {
                uosAppID = uosAppID,
                externalUserID = userID,
                externalPersonaID = personaID,
                displayName = displayName,
                realmID = realmID,
                channel = channel
            };
            return await ExternalLogin(request);
        }

        /// <summary>
        /// 使用外部UserID获取AccessToken
        /// </summary>
        /// <param name="request">外部登录请求(可包括: 外部UserID, 外部PersonaID, 昵称, RealmID</param>
        /// <exception cref="AuthException">ErrorCode.NotInitialized: AuthTokenManager未经初始化</exception>
        /// <exception cref="AuthException">ErrorCode.InvalidUosAppID: 无效的UosAppID</exception>
        /// <exception cref="AuthException">ErrorCode.InvalidUosAppID: 无效的UosAppID</exception>
        /// <exception cref="AuthException">ErrorCode.InvalidOperation: 未开启Passport服务,无法执行ExternalLogin</exception>
        public static async Task<TokenInfo> ExternalLogin(ExternalLoginRequest request)
        {
            var uosAppID = request.uosAppID;
            if (string.IsNullOrEmpty(uosAppID))
            {
                uosAppID = Instance.CurrentUosAppID;
            }
            if (!Instance._appSettings.ContainsKey(uosAppID))
            {
                throw new AuthException(ErrorCode.InvalidUosAppID, "无效的UosAppID");
            }

            var currentAppSetting = Instance._appSettings[uosAppID];

            var reqData = JsonConvert.SerializeObject(request);
            var resp = await currentAppSetting.HttpClient.Post<ExternalLoginResponse>(ExternalLoginPath, reqData);
            currentAppSetting.TokenInfo = new TokenInfo()
            {
                AccessToken = resp.personaAccessToken,
                RefreshToken = resp.personaRefreshToken,
                UserId = resp.persona == null ? request.externalUserID : resp.persona.userID,
                UserName = request.displayName,
                AccessTokenExpiresAt = JsonWebToken.Decode(resp.personaAccessToken).Expiration,
                RefreshTokenExpiresAt = JsonWebToken.Decode(resp.personaRefreshToken).Expiration,
                IsNew = resp.isNew,
                PersonaId = resp.persona == null ?  request.externalPersonaID : resp.persona.personaID
            };

            SaveToken(uosAppID, currentAppSetting.TokenInfo);
            return currentAppSetting.TokenInfo;
        }

        /// <summary>
        /// 在不开通Passport的情况下, 获取一个用户的AccessToken
        /// 注: 仅当一个程序需要集成多个UosApp的时候使用. 如只有单一UosApp, 请使用<see cref="GenerateAccessToken(string, string)"/>
        /// </summary>
        /// <param name="uosAppID">要获取AccessToken的UosAppID</param>
        /// <param name="userID">用户ID</param>
        /// <param name="userName">可选，用户名</param>
        /// <exception cref="AuthException">ErrorCode.NotInitialized: AuthTokenManager未经初始化</exception>
        /// <exception cref="AuthException">ErrorCode.GenerateTokenFailed: 获取AccessToken失败</exception>
        /// <exception cref="AuthException">ErrorCode.NonceTimestampExpired: Nonce时间戳已过期，请检查机器时间是否同步</exception>
        public static async Task GenerateAccessTokenWithUosAppId(string uosAppID, string userID, string userName = "")
        {
            if (!Instance._appSettings.ContainsKey(uosAppID))
            {
                throw new AuthException(ErrorCode.InvalidUosAppID, "无效的UosAppID");
            }

            var currentAppSetting = Instance._appSettings[uosAppID];

            var req = new GenerateAccessTokenRequest
            {
                userID = userID
            };
            var reqData = JsonConvert.SerializeObject(req);
            var resp = await currentAppSetting.HttpClient.Post<GetAccessTokenResponse>(GenerateTokenPath,
                reqData);
            currentAppSetting.TokenInfo = new TokenInfo()
            {
                AccessToken = resp.accessToken,
                RefreshToken = resp.refreshToken,
                UserId = userID,
                UserName = userName
            };
            SaveToken(uosAppID, currentAppSetting.TokenInfo);
        }

        /// <summary>
        /// 在不开通Passport的情况下, 获取一个用户的AccessToken
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <param name="userName">可选，用户名</param>
        /// <exception cref="AuthException">ErrorCode.NotInitialized: AuthTokenManager未经初始化</exception>
        /// <exception cref="AuthException">ErrorCode.GenerateTokenFailed: 获取AccessToken失败</exception>
        /// <exception cref="AuthException">ErrorCode.NonceTimestampExpired: Nonce时间戳已过期，请检查机器时间是否同步</exception>
        public static async Task GenerateAccessToken(string userID, string userName = "")
        {
            if (string.IsNullOrEmpty(Instance.CurrentUosAppID))
            {
                throw new AuthException(ErrorCode.NotInitialized, "AuthTokenManager未经初始化");
            }

            await GenerateAccessTokenWithUosAppId(Instance.CurrentUosAppID, userID, userName);
        }

        public static async Task<TokenInfo> AsyncRefreshAccessToken()
        {
            return await AsyncRefreshAccessToken(Instance.CurrentUosAppID);
        }

        public static async Task<TokenInfo> AsyncRefreshAccessToken(string uosAppID)
        {
            try
            {
                _refreshing = true;
                var currentAppSetting = Instance._appSettings[uosAppID];
                var currentTokenInfo = currentAppSetting.TokenInfo;
                var jwt = JsonWebToken.Decode(currentTokenInfo.RefreshToken);
                if (jwt.Expiration < DateTime.UtcNow)
                {
                    throw new AuthException(ErrorCode.NeedLogin, "需要重新登录以获取新的AccessToken/RefreshToken");
                }

                var localTimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                var req = new RefreshTokenRequest
                {
                    accessToken = currentTokenInfo.AccessToken,
                    refreshToken = currentTokenInfo.RefreshToken,
                    localTimestamp = localTimestamp
                };
                var json = JsonConvert.SerializeObject(req);
                var endpoint = PassportRefreshPath;

                var result = await currentAppSetting.HttpClient.Post<GetAccessTokenResponse>(endpoint, json);

                currentAppSetting.TokenInfo = new TokenInfo()
                {
                    AccessToken = result.accessToken,
                    RefreshToken = result.refreshToken,
                    UserId = currentTokenInfo.UserId,
                    UserName = currentTokenInfo.UserName,
                    AccessTokenExpiresAt = JsonWebToken.Decode(result.accessToken).Expiration,
                    RefreshTokenExpiresAt = JsonWebToken.Decode(result.refreshToken).Expiration,
                    IsNew = false
                };
                SaveToken(currentAppSetting.TokenInfo);
                return currentAppSetting.TokenInfo;
            }
            finally
            {
                _refreshing = false;
            }
        }

        private static IEnumerator RefreshAccessToken()
        {
            return RefreshAccessToken(Instance.CurrentUosAppID);
        }

        private static IEnumerator RefreshAccessToken(string uosAppID)
        {
            var task = AsyncRefreshAccessToken(uosAppID);
            yield return new WaitUntil(() => task.IsCompleted);
        }
    }

    public class TokenInfo
    {
        public string AccessToken;
        public string RefreshToken;
        public string UserId;
        public string UserName;
        public DateTime AccessTokenExpiresAt;
        public DateTime RefreshTokenExpiresAt;
        public bool IsNew;
        public string PersonaId;
    }

    public class AppSetting
    {
        public string UosAppID;
        public string UosAppSecret;
        [Obsolete]
        public bool UsePassportIdentity;

        internal string TokenPersistKey;
        internal TokenInfo TokenInfo;
        internal HttpClient HttpClient;
    }

    [Serializable]
    internal struct RefreshTokenRequest
    {
        public string accessToken;
        public string refreshToken;
        public long localTimestamp;
    }

    [Serializable]
    internal struct GenerateAccessTokenRequest
    {
        public string userID;
        public string externalPersonaID;
    }

    [Serializable]
    internal struct GetAccessTokenResponse
    {
        public string accessToken;
        public string refreshToken;
        public int expiresAt;
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum IdProvider
    {
        Undefined,
        Wechat,
        Apple,
        Qq,
        Weibo,
        WechatWeb,
        Qqweb,
        ServiceLogin,
        Hykb,
        TapTap
    }

    [Serializable]
    public struct ExternalLoginRequest
    {
        [JsonIgnore]
        public string uosAppID;
        public string externalUserID;
        public string externalPersonaID;
        public string displayName;
        public string realmID;
        public IdProvider idProvider;
        public string channel;
    }

    [Serializable]
    internal struct ExternalLoginResponse
    {
        public string personaAccessToken;
        public string personaRefreshToken;
        public int expiresAt;
        public Persona persona;
        public bool isNew;
    }

    [Serializable]
    internal class Persona
    {
        public string userID;
        public string personaID;
    }

    [Serializable]
    internal struct PublicInfoResponse
    {
        public IdDomain idDomain;
    }

    [Serializable]
    internal struct IdDomain
    {
        public string id;
    }
}