using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Threading.Tasks;
using Unity.UOS.Launcher.UI;

namespace Unity.UOS.Launcher.IMGUI
{
    public class LauncherUIIMGUI
    {
        private static readonly int ServiceItemHeight = 91;
        private static readonly int UpcomingItemHeight = 72;
        private static readonly int UpcomingHintHeight = 22;
        private static int _screenWidth = 0;
        private static readonly int MinWindowWidth = 503;
        private static readonly int OperateButtonWidth = 96;
        private static readonly int OperateButtonHeight = 24;
        private static readonly int DropdownButtonSize = 24;
        private static readonly Color TextColorWhite38 = new Color(1f, 1f, 1f, 0.38f);
        private static readonly Color TextColorWhite50 = new Color(1f, 1f, 1f, 0.5f);
        private static readonly Color BackgroundColorLight = new Color(60f / 255f, 60f / 255f, 60f / 255f, 1f);
        private static readonly Color BackgroundColorDark = new Color(49f / 255f, 49f / 255f, 49f / 255f, 1f);
        private static readonly Color BorderColor = new Color(25f / 255f, 25f / 255f, 25f / 255f, 1f);
        
        public delegate Task OnInstallDelegate(ServiceConfig config);

        public static OnInstallDelegate OnInstall;

        public delegate void OnDetailDelegate(ServiceConfig config);

        public static OnDetailDelegate OnDetail;

        public delegate Task OnRemoveDelegate(ServiceConfig config);

        public static OnRemoveDelegate OnRemove;
        
        public delegate Task OnEnableDelegate(ServiceConfig config);

        public static OnEnableDelegate OnEnable;
        private static Vector2 scrollPosition = Vector2.zero;
        public const string WarningText = "UOS Launcher 兼容的最低 Unity 版本为 2021.3 (LTS)。当前编辑器版本过低，请升级后使用。";
        public static void Init(List<ServiceConfig> serviceList)
        {
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Width(_screenWidth));
            var warningStyle = new GUIStyle();
            warningStyle.normal.textColor = Color.yellow;
            warningStyle.padding = new RectOffset(10, 10, 10, 10);
            warningStyle.wordWrap = true;
            
            GUILayout.Label(WarningText, warningStyle, GUILayout.ExpandWidth(true));
            for (int i = 0; i < serviceList.Count; i += 1)
            {
                var config = serviceList[i];
                new ServiceItem(config);
            }
            GUILayout.EndScrollView();
        }

        public class ServiceItem
        {
            private static bool _initialized;

            private static Texture2D _serviceBgTexture;
            private static Texture2D _linkBgTexture;

            public static void Init()
            {
                // 每次都需要重新计算屏幕大小
                _screenWidth = Mathf.Max(Utils.Normalize(Screen.width), Utils.Normalize(MinWindowWidth));

                if (_initialized) return;
                _initialized = true;

                // 生成 texture
                _linkBgTexture = Utils.CreateTexture(Color.clear, Color.clear, 1, 1);
                _serviceBgTexture =
                    Utils.CreateTexture(BackgroundColorLight, BorderColor, _screenWidth, ServiceItemHeight);
            }

            public ServiceItem( ServiceConfig config)
            {
                Init();

                // 外框样式
                var boxStyle = new GUIStyle();
                boxStyle.normal.background = _serviceBgTexture;
                boxStyle.padding = new RectOffset(10, 10, 10, 10);
                boxStyle.alignment = TextAnchor.MiddleLeft;
                // GUILayout.Box("", );
                
                GUILayout.BeginHorizontal(boxStyle);
                
                // 服务图标
                var texture = ServiceIcon.GetIcon(config.name);
                GUILayout.Box(texture, GUILayout.Height(48), GUILayout.Width(48));

                GUILayout.BeginVertical();

                // 标题
                var titleStyle = new GUIStyle(GUI.skin.label);
                int titleSize = 20;
                titleStyle.fontSize = titleSize;
                titleStyle.fontStyle = FontStyle.Bold;
                GUILayout.Label(config.displayName, titleStyle, GUILayout.ExpandWidth(false));
                
                // 描述
                var descStyle = new GUIStyle(GUI.skin.label);
                var descSize = 12;
                descStyle.fontSize = descSize;
                descStyle.normal.textColor = TextColorWhite50;
                descStyle.active.textColor = TextColorWhite50;
                descStyle.hover.textColor = TextColorWhite50;
                GUILayout.Label(config.description, descStyle);

                // 安装信息与版本信息
                if (!string.IsNullOrEmpty(config.installedVersion))
                {
                    var installStyle = new GUIStyle(GUI.skin.label);
                    var installSize = 12;
                    installStyle.fontSize = installSize;
                    installStyle.normal.textColor = TextColorWhite50;
                    installStyle.hover.textColor = TextColorWhite50;
                    installStyle.active.textColor = TextColorWhite50;
                    var installStatus = config.installed ? "Installed, " : "";
                    GUILayout.Label($"{installStatus}version {config.installedVersion}", installStyle);
                }
                
                GUILayout.EndVertical();
                
                // 按钮组
                GUILayout.BeginHorizontal(GUILayout.Width(160));

                // 安装/更新按钮
                var leftStyle = new GUIStyle(EditorStyles.miniButtonLeft);
                leftStyle.fixedHeight = OperateButtonHeight;
                leftStyle.fixedWidth = OperateButtonWidth;

                if (!string.IsNullOrEmpty(config.gitUrl))
                {
                    if (GUILayout.Button(config.installed ? "Detail" : "Install SDK", leftStyle))
                    {
                        if (!config.installed)
                        {
                            OnInstall(config);
                        }
                        else
                        {
                            OnDetail(config);
                        }
                    }
                }

                // 移除
                if (config.installed)
                {
                    var rightStyle = new GUIStyle(EditorStyles.miniButtonRight);
                    rightStyle.fixedHeight = OperateButtonHeight;
                    rightStyle.fixedWidth = DropdownButtonSize;

                    // 展开按钮
                    if (GUILayout.Button("···", rightStyle))
                    {
                        GenericMenu menu = new GenericMenu();
                        menu.AddItem(new GUIContent("Update"), false, () => { OnInstall(config); });
                        menu.AddSeparator("");
                        menu.AddItem(new GUIContent("Remove"), false, () => { OnRemove(config); });
                        var lastRect = GUILayoutUtility.GetLastRect();
                        lastRect.y += OperateButtonHeight;
                        menu.DropDown(lastRect);
                    }
                }
                
                // 外链图标
                var linkStyle = new GUIStyle();
                linkStyle.normal.background = _linkBgTexture;
                linkStyle.fixedHeight = 12;
                linkStyle.fixedWidth = 12;
                linkStyle.margin = new RectOffset(10, 0, 10, 0);

                var linkTexture = AssetDatabase.LoadAssetAtPath<Sprite>($"{PackageResourcesManager.GetPackageRelativePath()}/Editor/Sprites/Link.png").texture;
                if (!string.IsNullOrEmpty(config.homePage))
                {
                    if (GUILayout.Button(linkTexture, linkStyle, GUILayout.ExpandWidth(false), GUILayout.MaxWidth(24),
                            GUILayout.MaxHeight(26)))
                    {
                        Application.OpenURL(config.homePage);
                    }
                }

                GUILayout.EndHorizontal();
                GUILayout.EndHorizontal();
            }
        }
        
    }
}