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
    public class LinkAppWindow : EditorWindow
    {
        private static VisualTreeAsset m_UXMLTree;
        private static readonly int WindowWidth = 360;
        private static readonly int WindowHeight = 310;
        private static LinkAppWindow _window;
        private static TabbedMenuController _tabbedMenuController;

        private enum TabName
        {
            ProjectSelectorTab,
            UOSAPPInfoTab
        }

        public static void Open()
        {
            _window =
                (LinkAppWindow)GetWindow(typeof(LinkAppWindow), false, "Link App", true);
            _window.minSize = new Vector2(WindowWidth, WindowHeight);
            _window.maxSize = new Vector2(WindowWidth, WindowHeight);
            _window.Show();
        }

        public static void CloseWindow()
        {
            _window =
                (LinkAppWindow)GetWindow(typeof(LinkAppWindow), false, "Link App", true);
            _window.Close();
        }
#if UNITY_2021_3_OR_NEWER
        private void OnFocus()
        {
            LinkAppByUnityProjectTab.CheckLoginStatus();
        }
#endif
        
        static void Init()
        {
            m_UXMLTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>($"{PackageResourcesManager.GetPackageRelativePath()}/Editor/UXML/LinkAppWindow.uxml");
        }

        private async void CreateGUI()
        {
            Init();
            rootVisualElement.Add(m_UXMLTree.Instantiate());
#if UNITY_2021_3_OR_NEWER
            LinkAppByUnityProjectTab.Init(rootVisualElement);
#endif
            await LinkAppByAppIDTab.Init(rootVisualElement, _tabbedMenuController);
            _tabbedMenuController = new TabbedMenuController(rootVisualElement);
            _tabbedMenuController.RegisterTabCallbacks();
            LauncherUI.SetTheme(rootVisualElement);
            
            // 判断当前选中的tab 调用对应的link app方法
            var linkAppButton = rootVisualElement.Q<Button>("LinkAppButton");
            linkAppButton.clicked += HandleClickLinkAppButton;
        }

        private void HandleClickLinkAppButton()
        {
            var selectedTab = _tabbedMenuController.GetSelectedTab();
            if (selectedTab == TabName.ProjectSelectorTab.ToString())
            {
#if UNITY_2021_3_OR_NEWER
                LinkAppByUnityProjectTab.ConfirmLink();
#endif
            }
            else
            {
                LinkAppByAppIDTab.ConfirmLink();
            }
        }
    }
}