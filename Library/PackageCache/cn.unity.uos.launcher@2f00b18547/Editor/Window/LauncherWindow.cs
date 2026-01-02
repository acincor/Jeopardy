using System;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using Unity.UOS.Launcher.UI;
using UnityEngine.UIElements;
using System.Threading.Tasks;
using Unity.UOS.Common;

namespace Unity.UOS.Launcher
{
    public class LauncherWindow : EditorWindow
    {
        private static VisualTreeAsset m_UXMLTree;
        private static VisualTreeAsset m_serviceItem;
        private static readonly int MinWindowWidth = 503;
        private static List<ServiceConfig> _upcomingList = new List<ServiceConfig>();
        private static List<ServiceConfig> _serviceList = new List<ServiceConfig>();
        private static List<ServiceConfig> _libraryList = new List<ServiceConfig>();
        private float _deltaTime = 0;
        private const float FixedUpdateTime = 1;
        
#if UNITY_2021_3_OR_NEWER
        [MenuItem("UOS/Open Launcher", false, -110)]
        public static void Open()
        {
            LauncherWindow window =
                (LauncherWindow)GetWindow(typeof(LauncherWindow), false, "Unity Online Services", true);
            window.minSize = new Vector2(MinWindowWidth, 600);
            window.Show();
        }
        
#endif

#if UNITY_UOS_SECURITY
        [MenuItem("UOS/Launcher/Regenerate Encrypt Key", false, -10)]
        public static void GenKey()
        {
            string title = "Regenerate encrypt key";
            string message = "Are you sure you want to regenerate encrypt key?";
            string okButton = "Regenerate";
            string cancelButton = "Cancel";
            bool result = EditorUtility.DisplayDialog(title, message, okButton, cancelButton);

            if (result)
            {
                EncryptKeyManager.GenEncryptKey();
            }
        }
#endif
        [MenuItem("UOS/Launcher/Import Package Resources", false, -11)]
        public static void CheckPackageResource()
        {
            PackageResourcesManager.Check();
        }


        [MenuItem("UOS/Launcher/Fix settings by reimport", false, -12)]
        public static void FixSettingsByReimportInMenu()
        {
            FixSettings.Reimport();
        }
        
        
        [MenuItem("UOS/Launcher/Fix settings by delete", false, -13)]
        public static void FixSettingsByDeleteInMenu()
        {
            FixSettings.Delete();
        }

        // 执行操作的方法
        [MenuItem("UOS/Launcher/Enable Log")]
        public static void ToggleLog()
        {
            var defineSymbol = "UOS_LOG_LEVEL_INFO";
#if UOS_LOG_LEVEL_INFO
            PackageResourcesManager.RemoveDefineSymbolsForAllTarget(defineSymbol);
#else
            PackageResourcesManager.AddDefineSymbolsForAllTarget(defineSymbol);
#endif
        }

        // 验证方法
        [MenuItem("UOS/Launcher/Enable Log", validate = true)]
        public static bool EnableLogValidate()
        {
            // 返回当前状态，同时会显示勾选标记
#if UOS_LOG_LEVEL_INFO
            Menu.SetChecked("UOS/Launcher/Enable Log", true);
#else
            Menu.SetChecked("UOS/Launcher/Enable Log", false);
#endif
            return true; // 始终启用菜单项
        }
        
        // [MenuItem("UOS/Remove Package Resources")]
        // public static void RemovePackageResource()
        // {
        //     PackageResourcesManager.Remove();
        // }
        
        [MenuItem("UOS/Help", false, 9999)]
        public static void OpenHelp()
        {
            var window = GetWindow<HelpWindow>();
            window.titleContent = new GUIContent("Help");
            window.minSize=new Vector2(600, 320);
            window.maxSize=new Vector2(600, 320);
            window.Show();
        }

        static async Task Init()
        {
            m_UXMLTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>($"{PackageResourcesManager.GetPackageRelativePath()}/Editor/UXML/Main.uxml");
            m_serviceItem = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>($"{PackageResourcesManager.GetPackageRelativePath()}/Editor/UXML/ServiceItem.uxml");
            
            // 事件注册
            LauncherUI.OnInstall = PackageManager.Install;
            LauncherUI.OnDetail = PackageManager.Detail;
            LauncherUI.OnRemove = PackageManager.Remove;
            LauncherUI.OnEnable = ServiceManager.EnableService;

            // 获取服务配置
            var (res1, res2, res3) = await ServiceManager.GetServiceList();

            _upcomingList = res1;
            _serviceList = res2;
            _libraryList = res3;
        }

        private async void CreateGUI()
        {
            PackageResourcesManager.Check();
            await Init();
            LauncherUI.Init(rootVisualElement, m_UXMLTree, m_serviceItem, _serviceList, _upcomingList, _libraryList);
        }

        private void Update()
        {
            _deltaTime += Time.deltaTime;
            if (_deltaTime >= FixedUpdateTime)
            {
                _deltaTime -= FixedUpdateTime;            
                LauncherUI.FixedUpdate();
            }
        }

        [InitializeOnLoadMethod]
        public static async void PreCheck()
        {
            await Init();
            // 强制更新校验
            PackageUpgradeManager.CheckForceUpdate();
        }
#if UNITY_2021_3_OR_NEWER
        [MenuItem("UOS/Multiverse/Build Image", false, 0)]
        public static void BuildMultiverseImage()
        {
            OpenMultiverse();
        }

        public static void OpenServiceWindow(string name)
        {
            switch (name)
            {
                case "Multiverse": 
                    OpenMultiverse(); break;
            }
        }
        
        private static void OpenMultiverse()
        {
            var window = GetWindow<MultiverseImage>();
            window.minSize = new Vector2(600, 300);
            window.maxSize = new Vector2(800, 300);
            window.titleContent = new GUIContent("Multiverse Image");
            window.Show();
        }
        
#endif
        
        public static bool MultiverseEnabled()
        {
            foreach (var service in _serviceList)
            {
                if (service.displayName == "Multiverse")
                {
                    if (service.status == ServiceStatus.Enabled)
                    {
                        return true;
                    }
                    break;
                }
            }

            return false;
        }
    }
}