using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Unity.UOS.Common;

namespace UOSLauncher.Editor.Environment
{
    [System.Serializable]
    public class EnvironmentConfig
    {
        [Header("环境名称")]
        [Tooltip("环境名称")]
        public string environmentName;
        
        [Header("UOS APP 信息")]
        [Tooltip("UOS APP ID")]
        public string appID;
        [Tooltip("UOS APP Secret")]
        public string appSecret;
        [Tooltip("UOS APP Service Secret")]
        public string appServiceSecret;
        
        [Header("Sync")]
        [Tooltip("房间配置 UUID")] public string roomProfileUUID;
        [Header("Matchmaking")]
        [Tooltip("Matchmaking 配置 ID")] public string matchmakingConfigID;

        [Header("自定义配置")]
        [Tooltip("自定义键值对")] 
        public ConfigPair[] configPairs;
    }
    
    [CreateAssetMenu(menuName = "UOS/Create Environments Config", fileName = "UOSEnvironments.asset")]
    public class EnvironmentList : ScriptableObject
    {
        [SerializeField] private List<EnvironmentConfig> environments = new List<EnvironmentConfig>();
        [HideInInspector][SerializeField] private string selectedAppID;

        // 当前激活环境的快捷访问
        public EnvironmentConfig SelectedEnvironment => environments.FirstOrDefault(env => env.appID == selectedAppID);

        // 环境数量
        public int Count => environments.Count;
        public List<EnvironmentConfig> Environments => environments;

        public void AddOrUpdate(EnvironmentConfig config)
        {
            var existingConfigIndex = environments.FindIndex((configItem) => configItem.appID == config.appID);
            selectedAppID = config.appID;

            if (existingConfigIndex < 0)
            {
                environments.Add(config);
            }
            else
            {
                environments[existingConfigIndex].appID = config.appID;
                environments[existingConfigIndex].appSecret = config.appSecret;
                environments[existingConfigIndex].appServiceSecret = config.appServiceSecret;
            }
        }
        
        internal EnvironmentConfig SwitchSelectedAppID(string environmentName)
        {
            var env = environments.FirstOrDefault(env => env.environmentName == environmentName);
            if (env == null)
            {
                Debug.LogWarning("Environment slug name not found!");
                return null;
            }
            selectedAppID = env.appID;
            return env;
        }
    }
}