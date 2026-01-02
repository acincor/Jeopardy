# UOS Launcher
## 功能简介
- 在 Unity 编辑器中管理 UOS 服务。支持开启 UOS APP 关联的服务，支持安装、更新和卸载服务 SDK。
- 提供公共方法获取 UOS AppID 和 AppSecret，一处配置，多处使用，无需重复填写。
- 更多功能正在开发中...

## 安装方式

1. 在 Unity 编辑器中，菜单栏选择 Window -> Package Manager，打开包管理窗口。
2. 点击包管理窗口左上方「+」按钮，选择 「Add package from git URL」。
3. 在输入框中填入 https://cnb.cool/unity/uos/UOSLauncher.git ， 点击 「Add」，等待安装完成即可。

## 使用教程
### UOS Launcher 面板
1. 在 Unity 编辑器中，菜单栏选择 UOS -> Open Launcher，打开 UOS Launcher 面板。
2. 在 [UOS官网](https://uos.unity.cn/apps) 中选择你想要关联的 UOS APP，并将 **AppID**、**AppSecret** 和 **AppServerSecret** 复制填入 Launcher 面板中，点击 「Link App」关联到你的 UOS APP。
3. 在你想要启用的服务上，点击「Enable」，开启服务。
4. 查看所选服务的官方文档，在 UOS 官网 上进行必要的配置。
5. 点击「Install SDK」，将服务 SDK 安装到当前项目中。

其他操作：
6. 移除服务 SDK：点击服务项中的下拉按钮，点击「Remove」即可移除 SDK。

### 生成加密密钥
UOS Launcher 储存 AppID 等信息时，将使用加密密钥（Encrypt Key）进行加密。默认情况下，密钥是固定的。如果您想要更高的安全性，**推荐您重新生成密钥**。操作方式如下：

在 Unity 编辑器中，菜单栏选择 UOS -> Regenerate Encrypt Key，即可重新生成密钥。注意，该操作将会清空在 UOS Launcher 面板中填写的 AppID 等信息。

*注：加密密钥的存储依赖 UOSLauncherEncrypt 下的脚本，如果没有引入，可点击 UOS -> Import Package Resources，向当前项目导入相关的资源。

### 使用 UOS Common
本项目提供了公共的方法获取在 Launcher 面板中填写的 **AppID** 和 **AppSecret**。
```C#
using Unity.UOS.Common;
public class MyClass
{
    public static string UosAppId = Settings.AppID;
    public static string UosAppSecret = Settings.AppSecret;
}
```