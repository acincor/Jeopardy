using UnityEngine;
using UnityEngine.UIElements;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace Unity.UOS.Launcher.UI
{
        public class ServiceUIItem
        {
            private VisualElement _selfRoot;
            public ServiceConfig Config;

            /// <summary>
            /// 绘制服务项
            /// </summary>
            /// <param name="config">服务项配置</param>
            /// <returns></returns>
            public VisualElement InitialDraw(ServiceConfig config)
            {
                var item = LauncherUI.ServiceItemTemplate.Instantiate();
               
                _selfRoot = item;
                Config = config;
                
                // 服务名称
                item.Q<Label>("Title").text = config.displayName;

                // 主页链接
                var link = item.Q<Button>("HomePage");
                Tooltip.Listen(link, "Home Page", new Vector2(20, 20));
                if (String.IsNullOrEmpty(config.homePage))
                {
                    Utils.Hide(link);
                }
                else
                {
                    link.clicked += () => { Application.OpenURL(config.homePage); };
                }

                // 图标
                var texture = ServiceIcon.GetIcon(config.name);
                if (texture != null)
                {
                    var serviceIcon = item.Q<VisualElement>("ServiceIcon");
                    serviceIcon.style.backgroundImage = new StyleBackground((Texture2D)texture);
                }
                
                // 描述信息
                var description = item.Q<Label>("Description");
                description.text = config.description;

                // 文档
                var docs = item.Q<Button>("Docs");
                Tooltip.Listen(docs, "Documents", new Vector2(20, 20));
                if (string.IsNullOrEmpty(config.docUrl))
                {
                    Utils.Hide(docs);
                }
                else
                {
                    docs.clicked += () =>
                    {
                        Application.OpenURL(config.docUrl);
                    };
                }

                // 展示安装信息
                var installInfo = item.Q<Label>("InstallInfo");
                if (config.installed)
                {
                    var installStatus = config.installed ? "Installed," : "";
                    var installStatusLabel = item.Q<Label>("InstallStatus");
                    installStatusLabel.text = $"{installStatus}";
                    var versionAndDetail = item.Q<Button>("VersionAndDetail");
                    versionAndDetail.text = $"v{config.installedVersion}";
                    versionAndDetail.clicked += () =>
                    {
                        LauncherUI.OnDetail(config);
                    };
                }
                else
                {
                    Utils.Hide(installInfo);
                    item.style.height = 72;
                }
                
                // 操作按钮
                var manageButtons = _selfRoot.Q<VisualElement>("ManageButtons");
                var enableButtons = _selfRoot.Q<VisualElement>("EnableButtons");
                
                if (String.IsNullOrEmpty(config.gitUrl) || !config.launched)
                {
                    // upcomping 类型，和非SDK类型（没有gitUrl）不展示操作按钮
                    Utils.Hide(manageButtons);
                    Utils.Hide(enableButtons);
                    item.style.height = 72;
                }
                else
                {
                    // Service 类型
                    var installButton = item.Q<Button>("Install");
                    var dropdown = item.Q<Button>("Dropdown");
                    var configWeb = item.Q<Button>("ConfigWeb");
                    var uploadBtn = item.Q<Button>("Upload");
                    var enable = item.Q<Button>("Enable");
                    var sampleButton = item.Q<Button>("ServiceSample");
                    var sampleImportedButton = item.Q<Button>("ServiceSampleImported");
                    var sampleUrlButton = item.Q<Button>("ServiceSampleUrl");
                    
                    if (!config.enableUpload)
                    {
                        Utils.Hide(uploadBtn);
                    }
#if UNITY_2023_2_OR_NEWER
                    
                    dropdown.clicked += () =>
                    {
                        GenericMenu menu = new GenericMenu();
                        menu.AddItem(new GUIContent("Install"), false, () => { LauncherUI.OnInstall(config); });
                        menu.ShowAsContext();
                    };

#elif UNITY_2021_2_OR_NEWER
                    GenericDropdownMenu menu = new GenericDropdownMenu();
                    menu.AddItem("Install", false, () => { LauncherUI.OnInstall(config); });

                    dropdown.clicked += () =>
                    {
                        var pos = dropdown.worldBound.position;
                        menu.DropDown(new Rect(new Vector2(pos.x, pos.y - 76), new Vector2(80, 100)), dropdown,
                            true);
                    };
#endif
                    // dev portal 配置
                    configWeb.clicked += () =>
                    {
                        DevPortalManager.Jump(config);
                    };
                    Tooltip.Listen(configWeb, "Go to UOS Developer Portal", new Vector2(20, 20));

                    // install 按钮响应
                    var installServiceButton = new ServiceButton(installButton);
                    
                    installButton.clicked += async () =>
                    {
                        installServiceButton.Loading(true);
                        await LauncherUI.OnInstall(config);
                        installServiceButton.Loading(false);

                        DrawButtonStatus(Config);
                    };
                    Tooltip.Listen(installButton, "Install SDK", new Vector2(20, 20));

#if UNITY_2021_3_OR_NEWER
                    // upload 按钮响应
                    uploadBtn.clicked += () =>
                    {
                        LauncherWindow.OpenServiceWindow(config.displayName);
                    };
                    Tooltip.Listen(uploadBtn, config.uploadNotification, new Vector2(20, 20));
#endif
                    var enableServiceButton = new ServiceButton(enable);
                    // enable 按钮相应
                    enable.clicked += async () =>
                    {
                        enableServiceButton.Loading(true);
                        await LauncherUI.OnEnable(config);
                        enableServiceButton.Loading(false);
                        DrawButtonStatus(Config);
                    };
                    // enable 按钮跳转 dev portal
                    if (!string.IsNullOrEmpty(Config.enableServiceUrl))
                    {
                        var enableLinkButton = enable.Q<VisualElement>("EnableLinkButton");
                        Utils.Hide(enableLinkButton, false);
                        enable.style.width = 72;
                        enableButtons.style.width = 101;
                    }
                    
                    // 设置示例项目按钮
                    var hasSampleUrl = !string.IsNullOrEmpty(config.sampleUrl);
                    var hasSample = !hasSampleUrl && config.samples != null && config.samples.Any();
                    var sampleNotImported = hasSample && !config.sampleIsImported;
                    var sampleImported = hasSample && config.sampleIsImported;
                    
                    Utils.Hide(sampleUrlButton, !hasSampleUrl);
                    Utils.Hide(sampleButton, !sampleNotImported);
                    Utils.Hide(sampleImportedButton, !sampleImported);
                    
                    // 示例项目 url 跳转
                    if (hasSampleUrl)
                    {
                        sampleUrlButton.clicked += () =>
                        {
                            Application.OpenURL(config.sampleUrl);
                        };
                        Tooltip.Listen(sampleUrlButton, "Go to sample project tutorial.", new Vector2(20, 20));
                    }
                    
                    // 导入示例项目按钮
                    if (sampleNotImported)
                    {
                        var sampleServiceButton = new ServiceButton(sampleButton);
                        sampleButton.clicked += () =>
                        {
                            sampleServiceButton.Loading(true);
                            var success = PackageSampleManager.Import(config);
                            if (success)
                            {
                                Debug.Log("示例项目导入成功");
                                Utils.Hide(sampleButton, true);
                                Utils.Hide(sampleImportedButton, false);
                            }
                            else
                            {
                                Debug.LogError("示例项目导入失败");
                            }
                            sampleServiceButton.Loading(false);
                        };
                        Tooltip.Listen(sampleButton, "Click to import sample.", new Vector2(20, 20));
                    }
                    
                    // 示例项目已导入
                    if (sampleImported)
                    {
                        // sampleImportedButton.SetEnabled(false);
                        Tooltip.Listen(sampleImportedButton, "Sample has been imported.", new Vector2(20, 20));
                    }
                    
                    DrawButtonStatus(Config);
                }
                return item;
            }
            
            private void DrawButtonStatus(ServiceConfig config)
            {
                if(string.IsNullOrEmpty(config.gitUrl)) return;
                
                // 开启状态变化
                var manageButtons = _selfRoot.Q<VisualElement>("ManageButtons");
                var enableButtons = _selfRoot.Q<VisualElement>("EnableButtons");
                
                Utils.Hide(manageButtons, config.status != ServiceStatus.Enabled);
                Utils.Hide(enableButtons, config.status == ServiceStatus.Enabled);
                
                // 安装状态变化
                var installButton = _selfRoot.Q<Button>("Install");
                var installedButton = _selfRoot.Q<Button>("Installed");
                
                Utils.Hide(installButton, config.installed);
                Utils.Hide(installedButton, !config.installed);
                Tooltip.Listen(installedButton, $"{config.displayName} has been installed.", new Vector2(96, 20));

                // 设置 enable 按钮状态
                var enable = _selfRoot.Q<Button>("Enable");
                var disabled = !string.IsNullOrEmpty(config.statusReason);
                enable.SetEnabled(!disabled);
                
                // Tooltip 更新
                Tooltip.Listen(enable, config.statusReason, new Vector2(96, 20));
            }

            public void Draw(ServiceConfig config)
            {
                Config.status = config.status;
                Config.statusReason = config.statusReason;
                Config.subType = config.subType;
                DrawButtonStatus(Config);
            }
            
            public void Draw()
            {
                DrawButtonStatus(Config);
            }

            public void SetUpgradeButton()
            {
                var upgradeButton = _selfRoot.Q<Button>("UpgradeButton");
                var upgradeServiceButton = new ServiceButton(upgradeButton);
                if (PackageUpgradeManager.HasNew(Config))
                {
                    Utils.Hide(upgradeButton, false);
                    upgradeButton.clicked += async () =>
                    {
                        upgradeServiceButton.Loading(true);
                        await LauncherUI.OnInstall(Config);
                        upgradeServiceButton.Loading(false);
                        DrawButtonStatus(Config);
                    };
                    Tooltip.Listen(upgradeButton, $"{Config.displayName} v{Config.version} is available, click to upgrade!",  new Vector2(20, 20));
                }
                else
                {
                    Utils.Hide(upgradeButton);
                }
            }

            public void RemoveFromParent(VisualElement parent)
            {
                parent.Remove(_selfRoot);
            }
        }
}