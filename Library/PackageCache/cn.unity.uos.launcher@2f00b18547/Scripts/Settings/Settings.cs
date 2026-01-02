using System;
using UnityEngine;
using UnityEditor;
using UnityEngine.Serialization;
#if UNITY_UOS_SECURITY
using Unity.UOS.Encrypt;
#endif
#if UNITY_EDITOR
using UnityEngine.Windows;
#endif

namespace Unity.UOS.Common
{
    [Serializable]
    public class ConfigPair
    {
        public string key;
        public string value;
    }
    
    [CreateAssetMenu(fileName = "UOSSettings", menuName = "UOS/Create Settings")]
    [ExcludeFromPreset]
    public class Settings : ScriptableObject
    {
        private const string FileName = "UOSSettings";
        private const string FileExt = ".asset";
        private const string Folder = "Assets/Resources/";

        public delegate Settings LoadHandler();
        public static LoadHandler LoadMethod = DefaultLoadMethod;
        
        [SerializeField] private string encryptedAppID = "";
        [SerializeField] private string encryptedAppSecret = "";
#if UNITY_EDITOR || UNITY_SERVER
        [SerializeField] private string encryptedAppServiceSecret = "";
        [SerializeField] private string encrptedProjectId = "";
        [SerializeField] private string encrptedUserName = "";
#endif

        [SerializeField] private string encryptedRoomProfileUUID;
        [SerializeField] private string encryptedMatchmakingConfigID;
        [SerializeField] private ConfigPair[] configPairs;

#if UNITY_EDITOR
        public const string SettingsFileCorruptHint = "UOS 配置文件无法加载。\n 请检查项目是否有编译错误，如有编译错误，请修复。\n 然后点击 UOS -> Launcher -> Fix settings by reimport 尝试重新加载配置文件。\n 如果仍无法正常加载，请尝试点击 UOS -> Launcher -> Fix settings by delete 按钮，删除 Assets/Resources/UOSSettings.asset 文件后重新打开 Launcher 窗口填写 App 信息";
#endif
        private static Settings _instance;
        public static string Path => Folder + FileName + FileExt;

#if UNITY_EDITOR
        public static void GenSettingsFile()
        {
            _instance = CreateInstance<Settings>();
            if (!Directory.Exists(Folder))
            {
                Directory.CreateDirectory(Folder);
            }
            AssetDatabase.CreateAsset(_instance, Path);
            AssetDatabase.SaveAssets();
        }

        public static bool IsSettingsFileCorrupt()
        {
            var instance = LoadMethod();
            if (instance != null) return false;

            var settingsFileExist = File.Exists(Path);
            return settingsFileExist;
        }
#endif

        private static Settings DefaultLoadMethod()
        {
            return Resources.Load<Settings>(FileName);
        }
        
        private static Settings Instance
        {
            get
            {
#if UNITY_EDITOR
                if (_instance == null)
                {
                    // 读取已有配置
                    _instance = LoadMethod();
                }

                if (_instance == null)
                {
                    var settingsFileExist = File.Exists(Path);
                    if (settingsFileExist)
                    {
                        // Settings.cs 未正确编译 或 .asset 文件损坏
                        throw new Exception(SettingsFileCorruptHint);
                    }

                    // 创建新配置文件
                    GenSettingsFile();
                }
#else
                if (_instance == null) {
                    _instance = LoadMethod();
                }

                if (_instance == null)
                {
                    throw new Exception("请在 UOS Launcher 中填写 AppID 和 AppSecret 后继续");
                }
#endif

                return _instance;
            }
        }

        private static void Save()
        {
#if UNITY_EDITOR
            EditorUtility.SetDirty(_instance);
            AssetDatabase.SaveAssets();
#endif
        }

        public static string AppID
        {
            get
            {
                if (string.IsNullOrEmpty(Instance.encryptedAppID))
                {
                    return "";
                }

                try
                {
#if UNITY_UOS_SECURITY
                    var appID = EncryptManager.Decrypt(Instance.encryptedAppID);
#else
                    var appID = Instance.encryptedAppID;
#endif
                    return appID;
                }
                catch (Exception e)
                {
                    Debug.LogError(e.Message);
                    return "";
                }
            }
            set
            {
#if UNITY_UOS_SECURITY
                Instance.encryptedAppID = EncryptManager.Encrypt(value);
#else
                Instance.encryptedAppID = value;
#endif
                Save();
            }
        }

        public static string AppSecret
        {
            get
            {
                if (string.IsNullOrEmpty(Instance.encryptedAppSecret))
                {
                    return "";
                }

                try
                {
#if UNITY_UOS_SECURITY
                    var appSecret = EncryptManager.Decrypt(Instance.encryptedAppSecret);
#else
                    var appSecret = Instance.encryptedAppSecret;
#endif

                    return appSecret;
                }
                catch (Exception e)
                {
                    Debug.LogError(e.Message);
                    return "";
                }
            }
            set
            {
#if UNITY_UOS_SECURITY
                Instance.encryptedAppSecret = EncryptManager.Encrypt(value);
#else
                Instance.encryptedAppSecret = value;
                LogWarning();
#endif
                Save();
            }
        }
#if UNITY_EDITOR || UNITY_SERVER
        public static string AppServiceSecret
        {
            get
            {
                if (string.IsNullOrEmpty(Instance.encryptedAppServiceSecret))
                {
                    return "";
                }

                try
                {
#if UNITY_UOS_SECURITY
                    var appServiceSecret = EncryptManager.Decrypt(Instance.encryptedAppServiceSecret);
#else
                    var appServiceSecret = Instance.encryptedAppServiceSecret;
                    LogWarning();
#endif
                    return appServiceSecret;
                }
                catch (Exception e)
                {
                    Debug.LogError(e.Message);
                    return "";
                }
            }
            set
            {
#if UNITY_UOS_SECURITY
                Instance.encryptedAppServiceSecret = EncryptManager.Encrypt(value);
#else
                Instance.encryptedAppServiceSecret = value;
                LogWarning();
#endif
                Save();
            }
        }
        
        public static string ProjectId
        {
            get
            {
                if (string.IsNullOrEmpty(Instance.encrptedProjectId))
                {
                    return "";
                }

                try
                {
#if UNITY_UOS_SECURITY
                    var projectID = EncryptManager.Decrypt(Instance.encrptedProjectId);
#else
                    var projectID = Instance.encrptedProjectId;
#endif
                    return projectID;
                }
                catch (Exception e)
                {
                    Debug.LogError(e.Message);
                    return "";
                }
            }
            set
            {
#if UNITY_UOS_SECURITY
                Instance.encrptedProjectId = EncryptManager.Encrypt(value);
#else
                Instance.encrptedProjectId = value;
#endif
                Save();
            }
        }
        
        public static string UserName
        {
            get
            {
                if (string.IsNullOrEmpty(Instance.encrptedUserName))
                {
                    return "";
                }

                try
                {
#if UNITY_UOS_SECURITY
                    var userName = EncryptManager.Decrypt(Instance.encrptedUserName);
#else
                    var userName = Instance.encrptedUserName;
#endif
                    return userName;
                }
                catch (Exception e)
                {
                    Debug.LogError(e.Message);
                    return "";
                }
            }
            set
            {
#if UNITY_UOS_SECURITY
                Instance.encrptedUserName = EncryptManager.Encrypt(value);
#else
                Instance.encrptedUserName = value;
#endif
                Save();
            }
        }
#endif
        
        private static void LogWarning()
        {
            Debug.LogWarning("Encryption method is not currently being used. Please click 【UOS -> Launcher -> Import Package Resources】 to import encryption method, which increase security.");
        }

#if UNITY_EDITOR || UNITY_SERVER
        public static void ChangeEncryptKey(string encryptKey)
        {
            var appID = AppID;
            var appSecret = AppSecret;
            var appServiceSecret = AppServiceSecret;
            
#if UNITY_UOS_SECURITY
            EncryptKey.Value = encryptKey;
#endif
            AppID = appID;
            AppSecret = appSecret;
            AppServiceSecret = appServiceSecret;
        }
#endif
        
        public static string RoomProfileUUID
        {
            get
            {
                if (string.IsNullOrEmpty(Instance.encryptedRoomProfileUUID))
                {
                    return "";
                }

                try
                {
#if UNITY_UOS_SECURITY
                    var roomProfileUUID = EncryptManager.Decrypt(Instance.encryptedRoomProfileUUID);
#else
                    var roomProfileUUID = Instance.encryptedRoomProfileUUID;
#endif
                    return roomProfileUUID;
                }
                catch (Exception e)
                {
                    Debug.LogError(e.Message);
                    return "";
                }
            }
            set
            {
#if UNITY_UOS_SECURITY
                Instance.encryptedRoomProfileUUID = string.IsNullOrEmpty(value) ? "": EncryptManager.Encrypt(value);
#else
                Instance.encryptedRoomProfileUUID = value;
#endif
                Save();
            }
        }
        
        public static string MatchmakingConfigID
        {
            get
            {
                if (string.IsNullOrEmpty(Instance.encryptedMatchmakingConfigID))
                {
                    return "";
                }

                try
                {
#if UNITY_UOS_SECURITY
                    var matchmakingConfigID = EncryptManager.Decrypt(Instance.encryptedMatchmakingConfigID);
#else
                    var matchmakingConfigID = Instance.encryptedMatchmakingConfigID;
#endif
                    return matchmakingConfigID;
                }
                catch (Exception e)
                {
                    Debug.LogError(e.Message);
                    return "";
                }
            }
            set
            {
#if UNITY_UOS_SECURITY
                Instance.encryptedMatchmakingConfigID = string.IsNullOrEmpty(value) ? "": EncryptManager.Encrypt(value);
#else
                Instance.encryptedMatchmakingConfigID = value;
#endif
                Save();
            }
        }
        
        public static ConfigPair[] ConfigPairs
        {
            get => Instance.configPairs;
            set => Instance.configPairs = value;
        }
        
        public static string GetValue(string key, string defaultValue = null)
        {
            foreach (var entry in Instance.configPairs)
            {
                if (entry.key == key)
                    return entry.value;
            }
            return defaultValue;
        }
    }
}