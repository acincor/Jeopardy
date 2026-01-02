using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine.Events;

namespace Unity.UOS.Launcher.UI
{
    public class LauncherUI
    {
        private static VisualElement _root;
        private static VisualTreeAsset _uxmlTree;
        public static VisualTreeAsset ServiceItemTemplate;
        private static List<ServiceConfig> _serviceList;
        private static List<ServiceConfig> _upcomingList;
        private static List<ServiceConfig> _libraryList;
        private static List<ServiceUIItem> _serviceUIItemList = new List<ServiceUIItem>();
        public static Dictionary<string, ServiceConfig> ServiceMap = new Dictionary<string, ServiceConfig>();
        private const string Homepage = "https://uos.unity.cn";
        private static ServiceUIItem _funcRealtimeItem = null;
        private static List<ServiceUIItem> _internalTestingItems = new List<ServiceUIItem>();

        public delegate Task OnInstallDelegate(ServiceConfig config);

        public static OnInstallDelegate OnInstall;

        public delegate void OnDetailDelegate(ServiceConfig config);

        public static OnDetailDelegate OnDetail;

        public delegate Task OnRemoveDelegate(ServiceConfig config);

        public static OnRemoveDelegate OnRemove;
        
        public delegate Task OnEnableDelegate(ServiceConfig config);

        public static OnEnableDelegate OnEnable;
        public static UnityEvent OnFixedUpdate = new UnityEvent();
        private static VisualElement _servicesVisualElement;

        public static void FixedUpdate()
        {
            OnFixedUpdate.Invoke();
        }
        public static void Init(VisualElement root, VisualTreeAsset uxmlTree, VisualTreeAsset serviceItem,
            List<ServiceConfig> serviceList, List<ServiceConfig> upcomingList, List<ServiceConfig> libraryList)
        {
            _funcRealtimeItem = null;
            SetTheme(root);

            // 初始化变量
            _uxmlTree = uxmlTree;
            _root = root;
            ServiceItemTemplate = serviceItem;
            _serviceList = serviceList;
            _upcomingList = upcomingList;
            _libraryList = libraryList;
            ServiceMap = new Dictionary<string, ServiceConfig>();
            foreach (var service in _serviceList)
            {
                ServiceMap[service.name] = service;
            }
            foreach (var service in _libraryList)
            {
                ServiceMap[service.name] = service;
            }

            Draw();
        }

        /// <summary>
        /// 根据编辑器主题设置 Launcher 主题
        /// </summary>
        public static void SetTheme(VisualElement root)
        {
            // 判断皮肤
            var themeFile = EditorGUIUtility.isProSkin ? "DarkTheme" : "LightTheme";
            var theme = AssetDatabase.LoadAssetAtPath<StyleSheet>($"{PackageResourcesManager.GetPackageRelativePath()}/Editor/Theme/{themeFile}.tss");
            root.styleSheets.Add(theme);
        }

        /// <summary>
        /// 主界面绘制
        /// </summary>
        private static async void Draw()
        {
            // 界面绘制
            _root.Add(_uxmlTree.Instantiate());
            
            var helpBtn = _root.Q<Button>("OpenHelpWindow");
            helpBtn.clicked += LauncherWindow.OpenHelp;

            var linkHomepage = _root.Q<Button>("LinkHomepage");
            linkHomepage.clicked += () => { Application.OpenURL(Homepage); };
            var linkHomepageIcon = _root.Q<Button>("LinkHomepageIcon");
            linkHomepageIcon.clicked += () => { Application.OpenURL(Homepage); };

            _serviceUIItemList = new List<ServiceUIItem>();

            _servicesVisualElement = _root.Q<VisualElement>("Services");
            _internalTestingItems.Clear();
#if !TUANJIE_1_0_OR_NEWER
            CheckMinigame();
#endif

            for (int i = 0; i < _serviceList.Count; i += 1)
            {
                var config = _serviceList[i];
                var item = new ServiceUIItem();
                
                // 首次绘制，不绘制内测服务
                if (config.internalTesting) continue;
                
                _servicesVisualElement.Add(item.InitialDraw(config));
                _serviceUIItemList.Add(item);
            }

            var upcoming = _root.Q<VisualElement>("Upcoming");
            for (int i = 0; i < _upcomingList.Count; i += 1)
            {
                var config = _upcomingList[i];
                upcoming.Add(new ServiceUIItem().InitialDraw(config));
            }

            if (!_upcomingList.Any())
            {
                var upcomingLabel = _root.Q<Label>("UpcomingLabel");
                Utils.Hide(upcomingLabel);
            }

            await LinkAppInMainUI.Init(_root);
            await SetVersionForLauncher();

            DrawUpgradeButtons();
        }

        private static async void DrawUpgradeButtons()
        {
            // 获取远程最新版本信息
            await ServiceList.InitLatestVersion();
            foreach (var service in _serviceUIItemList)
            {
                service.SetUpgradeButton();
            }
            
            // 强制更新校验
            PackageUpgradeManager.CheckForceUpdate();
        }

        /// <summary>
        /// 校验是否开启小游戏
        /// </summary>
        private static void CheckMinigame()
        {
            var container = _root.Q<VisualElement>("Minigame");
            Utils.Hide(container, false);
            var douyinToggle = _root.Q<Toggle>("DouyinMinigameToggle");
            var douyin = new DouyinMinigameDefineSymbol();
            var isDouyinMinigame = douyin.IsMinigame();
            douyinToggle.SetValueWithoutNotify(isDouyinMinigame);
            douyinToggle.RegisterValueChangedCallback((e) =>
            {
#if UNITY_WEBGL
                douyin.SetValue(e.newValue);
                douyinToggle.SetEnabled(false);
#else
                if (e.newValue)
                {
                    var result = douyin.DisplayDialog();
                    if (!result)
                    {
                        douyinToggle.SetValueWithoutNotify(false);
                    }
                }
#endif
            });
            
            var weixinToggle = _root.Q<Toggle>("WeixinMinigameToggle");
            var weixin = new WeixinMinigameDefineSymbol();
            var isWeixinMinigame = weixin.IsMinigame();
            weixinToggle.SetValueWithoutNotify(isWeixinMinigame);
            weixinToggle.RegisterValueChangedCallback((e) =>
            {
#if UNITY_WEBGL
                weixin.SetValue(e.newValue);
                weixinToggle.SetEnabled(false);
#else
                if (e.newValue)
                {
                    var result = weixin.DisplayDialog();
                    if (!result)
                    {
                        weixinToggle.SetValueWithoutNotify(false);
                    }
                }
#endif
            });
        }

        /// <summary>
        /// 设置 Launcher 版本信息和升级按钮
        /// </summary>
        private static async Task SetVersionForLauncher()
        {
            var installedVersion = KnownService.Launcher.installedVersion;
            if (!string.IsNullOrEmpty(installedVersion))
            {
                // 显示升级按钮
                await PackageUpgradeManager.GetLatestVersion(KnownService.Launcher);
                var upgradeButtonLauncher = _root.Q<Button>("UpgradeButtonLauncher");
            
                if (PackageUpgradeManager.HasNew(KnownService.Launcher))
                {
                    upgradeButtonLauncher.clicked += async () =>
                    {
                        upgradeButtonLauncher.SetEnabled(false);
                        await PackageManager.Install(KnownService.Launcher);
                        upgradeButtonLauncher.SetEnabled(true);
                    };
                    Utils.Hide(upgradeButtonLauncher, false);
                    Tooltip.Listen(upgradeButtonLauncher, $"{KnownService.Launcher.displayName} v{KnownService.Launcher.version} is available, click to upgrade!",  new Vector2(20, 20));
                }
            
                // 显示当前版本信息
                var versionAndDetailLauncher = _root.Q<Button>("VersionAndDetailLauncher");
                Utils.Hide(versionAndDetailLauncher, false);
                versionAndDetailLauncher.clicked += () =>
                {
                    PackageManager.Detail(KnownService.Launcher);
                };
                versionAndDetailLauncher.text = $"v{installedVersion}";
                
                var versionDivider = _root.Q<Label>("VersionDivider");
                Utils.Hide(versionDivider, false);
            };
        }

        /// <summary>
        /// 更新 UI 服务列表项
        /// </summary>
        /// <param name="serviceConfigMap">从 AppInfo 中获取的服务开启信息</param>
        public static void DrawServiceList(Dictionary<string, ServiceConfig> serviceConfigMap)
        {
            // 更新信息
            foreach (var service in _serviceUIItemList)
            {
                if (serviceConfigMap.TryGetValue(service.Config.name, out var config))
                {
                    service.Draw(config);
                }
            }

            DrawInternalTestingItems(serviceConfigMap);
            DrawFuncRealtimeUIItem(serviceConfigMap);
        }

        /// <summary>
        /// 绘制内测项目：如果 UOS APP 已开启某个内测服务，则绘制；否则不绘制
        /// </summary>
        /// <param name="serviceConfigMap"></param>
        private static void DrawInternalTestingItems(Dictionary<string, ServiceConfig> serviceConfigMap)
        {
            // 清空已经绘制项
            foreach (var item in _internalTestingItems)
            {
                item.RemoveFromParent(_servicesVisualElement);
            }
            _internalTestingItems.Clear();
            
            
            for (int i = 0; i < _serviceList.Count; i += 1)
            {
                var configItem = _serviceList[i];
                if (serviceConfigMap.TryGetValue(configItem.name, out var config))
                {
                    if (configItem.internalTesting && config.status == ServiceStatus.Enabled)
                    {
                        // 补充绘制
                        var serviceUIItem = new ServiceUIItem();
                        _servicesVisualElement.Add(serviceUIItem.InitialDraw(configItem));
                        serviceUIItem.Draw(config);
                        _internalTestingItems.Add(serviceUIItem);
                    }
                }
            }
        }

        /// <summary>
        /// 设置 Func Realtime 服务项
        /// </summary>
        /// <param name="serviceConfigMap"></param>
        public static void DrawFuncRealtimeUIItem(Dictionary<string, ServiceConfig> serviceConfigMap)
        {
            // 处理 Func Realtime 服务项
            if(serviceConfigMap.TryGetValue(KnownService.FuncRealtime.name, out var funcRealtimeConfig))
            {
                if (_funcRealtimeItem == null)
                {
                    _funcRealtimeItem = new ServiceUIItem();
                    _servicesVisualElement.Add(_funcRealtimeItem.InitialDraw(funcRealtimeConfig));
                }
            }
            else
            {
                // 移除
                if (_funcRealtimeItem != null)
                {
                    _funcRealtimeItem.RemoveFromParent(_servicesVisualElement);
                    _funcRealtimeItem = null;
                }
            }
        }
        
        public static void DrawServiceList()
        {
            foreach (var service in _serviceUIItemList)
            {
                service.Draw();
            }
        }
    }
} 