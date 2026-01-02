using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.PackageManager.UI;
using UnityEngine;

namespace Unity.UOS.Launcher
{
    public class WelcomeWindow: EditorWindow
    {
        // 尺寸配置
        private const float WindowWidth = 780;
        private const float WindowHeight = WindowWidth * 0.672f;
        private const float ImageWidth = 220;
        private const float ImageHeight = ImageWidth * 1.286f;
        private const int TemplateLabelFontSize = 25; // 模板标签字体大小
        private const int HeaderHeight = 145; // 背景区域大小
        private const int TitleFontSize = 18; // 标题字体大小
        private const int TextFontSize = 14;
        private const int TemplateCircleButtonSize = 28; // 模板中圆形按钮的尺寸
        
        // 文案配置
        private const string Title = "欢迎使用 Unity Online Services！";
        private const string Description =
            "UOS 通过云服务的方式，为各种类型联网游戏的制作提供便利的支持，大大减少开发周期，简化运维和运营工作。";
        private const string Description2 = "您可以一键添加玩家登录系统，支持多种第三方授权方式，实现了用户协议管理，以及玩家实名认证。";
        private const string Description3 = "您还可以随时方便的实现游戏的云存档。";
        private const string Description4 = "还有超多功能强大的服务，您可以根据自己游戏的类型，在以下模板中进行挑选";
        
        private Texture2D _templateOnlineGameImage;
        private Texture2D _templateMultiplayerOnlineGameImage;
        private Texture2D _templateMiniGameImage;
        
        private Texture2D _templateOnlineGameImageHover;
        private Texture2D _templateMultiplayerOnlineGameImageHover;
        private Texture2D _templateMiniGameImageHover;
        
        private Texture2D _jumpButtonImage;
        
        private const string AutoShowKey = "UOS_LAUNCHER_WELCOME_WINDOW_AUTO_SHOW";
                
        [MenuItem("UOS/Welcome", false, 9998)]
        public static void OpenWelcomeWindow()
        {
            WelcomeWindow window =
                (WelcomeWindow)GetWindow(typeof(WelcomeWindow), false, "Welcome", true);
            window.titleContent = new GUIContent("Welcome");

            var x = (Screen.currentResolution.width - WindowWidth) / 2;
            var y = (Screen.currentResolution.height - WindowHeight) / 2;

            window.minSize=new Vector2(WelcomeWindow.WindowWidth, WelcomeWindow.WindowHeight);
            window.maxSize=new Vector2(WelcomeWindow.WindowWidth, WelcomeWindow.WindowHeight);
            // 保持窗口居中
            window.position = new Rect(x, y, WindowWidth, WindowHeight);
            
            window.Show(true);
        }
        
        [InitializeOnLoadMethod]
        public static void AutoShow()
        {
            EditorApplication.delayCall += () =>
            {
                var originalPath = Application.dataPath;
                var regex = new Regex(@"/Library/VP/mppm.*?/Assets", RegexOptions.Compiled);
                string resultPath = regex.Replace(originalPath, "/Assets");
                var key = AutoShowKey + resultPath;
                if (EditorPrefs.HasKey(key)) return;
                EditorPrefs.SetString(key, "Shown");
                OpenWelcomeWindow();
            };
        }
        
        private void OnEnable()
        {
            _templateOnlineGameImage = LoadImage("TemplateOnlineGame.png");
            _templateMultiplayerOnlineGameImage = LoadImage("TemplateMultiplayerOnlineGame.png");
            _templateMiniGameImage = LoadImage("TemplateMiniGame.png");
            
            _templateOnlineGameImageHover = LoadImage("TemplateOnlineGameHover.png");
            _templateMultiplayerOnlineGameImageHover = LoadImage("TemplateMultiplayerOnlineGameHover.png");
            _templateMiniGameImageHover = LoadImage("TemplateMiniGameHover.png");
            _jumpButtonImage = LoadImage("JumpButtonImage.png");
        }

        /// <summary>
        /// 加载图片
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        private Texture2D LoadImage(string filename)
        {
            return AssetDatabase.LoadAssetAtPath<Texture2D>($"{PackageResourcesManager.GetPackageRelativePath()}/Editor/Sprites/{filename}");
        }

        /// <summary>
        /// 绘制指定颜色的 texture
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        private Texture2D CreateTexture(Color color)
        {
            Texture2D backgroundTexture = new Texture2D(1, 1);
            backgroundTexture.SetPixel(0, 0, color);
            backgroundTexture.Apply();
            return backgroundTexture;
        }

        /// <summary>
        /// 绘制模板
        /// </summary>
        /// <param name="background"></param>
        /// <param name="label"></param>
        /// <param name="linkUrl"></param>
        private void DrawTemplate(Texture2D background, Texture2D hoverBackground, string label, string linkUrl)
        {
            var templateOnlineStyle = new GUIStyle();
            templateOnlineStyle.normal.background = background;
            templateOnlineStyle.hover.background = hoverBackground;
            GUILayout.BeginVertical(templateOnlineStyle, GUILayout.Width(ImageWidth), GUILayout.Height(ImageHeight));
            GUILayout.FlexibleSpace();
            
            // 标签
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUIStyle labelStyle = new GUIStyle(GUI.skin.label);
            labelStyle.fontStyle = FontStyle.Bold;
            labelStyle.fontSize = TemplateLabelFontSize; // 设置字体大小
            GUILayout.Label(label, labelStyle);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            
            GUILayout.Space(8);
            
            // 按钮
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            var buttonStyle = new GUIStyle();
            buttonStyle.normal.background = null;
            if (GUILayout.Button(_jumpButtonImage,
                    buttonStyle,
                    GUILayout.Width(TemplateCircleButtonSize),
                    GUILayout.Height(TemplateCircleButtonSize)))
            {
                Application.OpenURL(linkUrl);
            }
            
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            
            GUILayout.Space(28);
            GUILayout.EndVertical();
            
            // 获取布局结束后的矩形区域
            Rect verticalRect = GUILayoutUtility.GetLastRect();
        
            // 检测鼠标点击事件
            if (Event.current.type == EventType.MouseDown && verticalRect.Contains(Event.current.mousePosition))
            {
                Application.OpenURL(linkUrl);
            }
        }

        void OnGUI()
        {
            var titleStyle = new GUIStyle(EditorStyles.label);
            titleStyle.fontSize = TitleFontSize;
            titleStyle.wordWrap = true;
            titleStyle.fontStyle = FontStyle.Bold;
            
            var labelStyle = new GUIStyle(EditorStyles.label);
            labelStyle.fontSize = TextFontSize;
            labelStyle.wordWrap = true;
            
            // 设置背景区域的位置和大小
            Rect backgroundRect = new Rect(0, HeaderHeight, position.width, position.height - HeaderHeight);

            // 绘制背景
            GUI.DrawTexture(backgroundRect, CreateTexture(new Color(0, 0, 0, 0.19f)));
            
            GUILayout.Space(20);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.BeginVertical();
            
            // 标题区域
            GUILayout.BeginHorizontal();
            GUILayout.Label(Title, titleStyle);
#if UNITY_2021_3_OR_NEWER

            if (GUILayout.Button("打开 UOS Launcher", GUILayout.Width(180), GUILayout.Height(30)))
            {
                LauncherWindow.Open();
            }
#endif
            
            GUILayout.EndHorizontal();
            
            GUILayout.Space(18);

            var lineGap = 4;
            GUILayout.Label(Description, labelStyle);
            GUILayout.Space(lineGap);

            GUILayout.Label(Description2, labelStyle);
            GUILayout.Space(lineGap);

            GUILayout.Label(Description3, labelStyle);
            
            GUILayout.Space(40);

            GUILayout.Label(Description4, labelStyle);
            GUILayout.Space(18);

            // 模板
            GUILayout.BeginHorizontal();
            var templateSpace = 32;

            DrawTemplate(_templateOnlineGameImage, 
                _templateOnlineGameImageHover,
                "联网游戏",
                "https://uos.unity.cn/product/onlinegame#hub-template");
            
            GUILayout.Space(templateSpace);
            
            DrawTemplate(_templateMultiplayerOnlineGameImage, 
                _templateMultiplayerOnlineGameImageHover,
                "多人联机游戏",
                "https://uos.unity.cn/product/multiplayer#hub-template");
            
            GUILayout.Space(templateSpace);
            
            DrawTemplate(_templateMiniGameImage, 
                _templateMiniGameImageHover,
                "小游戏",
                "https://uos.unity.cn/product/minigame#hub-template");

            GUILayout.EndHorizontal();
            
            GUILayout.EndVertical();
            GUILayout.FlexibleSpace();

            GUILayout.EndHorizontal();
        }
    }
}
