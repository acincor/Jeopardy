# UOS Launcher CHANGELOG

## [2.7.5] - 2025-12-12
### Changed
- 初次登录时可以附加渠道信息

## [2.7.4] - 2025-11-17
### Fixed
- 开启服务时兼容多服务模式

## [2.7.2] - 2025-11-17
### Fixed
- 开启服务时清空本地登陆信息

## [2.7.2] - 2025-11-03
### Fixed
- 优化RefreshToken过期时的异常信息

## [2.7.1] - 2025-09-10
### Fixed
- 修改 ExceptionUtil 所属 Namespace

## [2.7.0] - 2025-09-10
### Added
- 添加一个公用的 ExceptionUtil 方法

## [2.6.11] - 2025-09-08
### Fixed
- 修复切换环境不生效问题

## [2.6.10] - 2025-09-04
### Changed
- 修改AuthTokenManager中异常Log等级

## [2.6.9] - 2025-09-03
### Changed
- 修改AuthTokenManager中GenerateAccessToken抛出异常的说明

## [2.6.8] - 2025-09-02
### Added
- 重新启用GenerateAccessToken方法，给未开通Passport的UOS App使用

## [2.6.7] - 2025-08-13
### Added
- Launcher 面板支持切换和配置环境

## [2.6.6] - 2025-07-10
### Fixed
- 修复版本升级提示问题

## [2.6.5] - 2025-07-07
### Changed
- 迁移代码仓库至 cnb 平台

## [2.6.4] - 2025-07-04
### Added
- 新增微信和抖音小游戏宏定义

## [2.6.3] - 2025-07-02
### Added
- 新增菜单项 UOS -> Launcher -> Enable Log，支持开启和关闭详细日志输出

## [2.6.2] - 2025-06-30
### Added
- 在AuthTokenManager中为RefreshToken添加localTimestamp参数

## [2.6.1] - 2025-06-26
### Changed
- 优化玩家被封禁后登录的报错信息内容，在其中添加玩家的基本信息

## [2.6.0] - 2025-06-18
### Added
- 新增 Push Storage SDK

## [2.5.2] - 2025-06-05
### Changed
- 更新RemoteConfig Setting，支持获取key的列表

## [2.5.1] - 2025-06-04
### Added
- 添加RemoteConfig Setting中的OverrideName和VariantName属性

## [2.5.0] - 2025-06-04
### Added
- ExternalLogin支持传入IDProvider参数

## [2.4.12] - 2025-05-29
### Fixed
- 修复RemoteConfig的PlayerSettings和PlayerOverrides的Get方法在key不存在的情况下的异常

## [2.4.11] - 2025-05-28
### Added
- 在UOSLauncher中添加RemoteConfig的PlayerSettings和PlayerOverrides类型

## [2.4.10] - 2025-05-20
### Added
- Sync Realtime SDK 添加 cn.unity.uos.transport.kcp2k 依赖

## [2.4.9] - 2025-05-15
### Added
- ExternalLogin时，如果玩家被封禁，将在ErrorDetail中返回封禁信息

## [2.4.8] - 2025-05-13
### Fixed
- 修复 Dedicated Server 宏问题

## [2.4.7] - 2025-04-27
### Fixed
- 修复UOS Settings的单例初始化问题

## [2.4.6] - 2025-04-24
### Fixed
- 优化 Launcher 首屏加载速度

## [2.4.5] - 2025-04-23
### Fixed
- 安装push/sync sdk时自动安装kcp package

## [2.4.4] - 2025-04-17
### Fixed
- 兼容快手小游戏 KSUnityOpenSDK

## [2.4.3] - 2025-04-15
### Added
- 添加WindSDK

## [2.4.2] - 2025-04-15
### Fixed
- 修复UOS Settings的单例初始化问题

## [2.4.1] - 2025-04-14
### Added
- 支持团结引擎 OpenHarmony 平台、HMI Android 平台

## [2.4.0] - 2025-04-11
### Added
- 在TokenInfo中添加PersonaID
- HttpClient初始化时支持传入Timeout参数

## [2.3.30] - 2025-04-08
### Added
- Launcher 支持自定义「配置文件加载」方法

## [2.3.29] - 2025-03-31
### Fixed
- 修复欢迎窗口在 Multiplayer Play Mode 下多次展示的问题

## [2.3.28] - 2025-03-31
### Added
- 外部登录支持传入服务器ID（realmID）

## [2.3.27] - 2025-03-17
### Added
- 优化Launcher及SDK升级逻辑

## [2.3.26] - 2025-03-13
### Added
- 添加baking服务Endpoint

## [2.3.25] - 2025-03-05
### Fixed
- 修复 Cloud Save 服务开启问题

## [2.3.24] - 2025-03-03
### Changed
- 修复TimestampContractResolver类型转换问题

## [2.3.23] - 2025-02-17
### Changed
- Enable MetricsSDK时跳转「联系我们」

## [2.3.22] - 2025-02-14
### Changed
- 优化Multiverse Image Build窗口样式

## [2.3.21] - 2025-02-08
### Changed
- 升级内置 Websocket 依赖

## [2.3.20] - 2025-02-05
### Added
- 修复 MetricsSDK Enable 状态显示问题

## [2.3.19] - 2025-01-26
### Added
- 添加MetricsSDK

## [2.3.18] - 2025-01-24
### Added
- Launcher 中过滤已归档项目

## [2.3.17] - 2025-01-17
### Added
- ExternalLogin返回TokenInfo

## [2.3.16] - 2025-01-16
### Fixed
- 优化欢迎窗口展示

## [2.3.15] - 2025-01-14
### Fixed
- 兼容 Unity 2020 LTS

## [2.3.14] - 2024-12-30
### Added
- 兼容 Sync Type 为空时使用 Realtime Type

## [2.3.13] - 2024-12-11
### Added
- 添加UOS Baking Service配置

## [2.3.12] - 2024-12-03
### Fixed
- 支持 Unity 2021.2.5f1c303

## [2.3.11] - 2024-11-29
### Fixed
- 修复文件路径引用警告

## [2.3.10] - 2024-11-27
### Refactor
- 升级重构 UOS Launcher 面板关联 UOS APP 功能

## [2.3.9] - 2024-11-22
### Changed
- 优化AuthTokenManager的初始化方法

## [2.3.8] - 2024-11-18
### Added
- 为AuthTokenManager添加Passport自适应初始化方式

## [2.3.7] - 2024-11-19
### Refactor
-  移除 Save SDK 对 UniTask 包的引用

## [2.3.6] - 2024-11-18
### Changed
- 优化COSXML命名空间

## [2.3.5] - 2024-11-18
### Fixed
- 修复欢迎页未自动展示问题

## [2.3.4] - 2024-11-14
### Fixed
- 修复 Unity 项目选择问题

## [2.3.3] - 2024-11-12
- 修改默认超时时长配置

## [2.3.2] - 2024-11-11
- 修复 UOS Launcher 面板未显示关联 APP 详情问题

## [2.3.1] - 2024-11-06
### Added
- UOS Launcher 新增欢迎页面
- 支持关联 Unity 项目

## [2.3.0] - 2024-10-25
### Added
- 新增 Safe SDK

## [2.2.6] - 2024-10-18
### Fixed
- 修复微信平台 websocket 问题

## [2.2.5] - 2024-10-12
### Fixed
- 修复 auth 缓存问题

## [2.2.4] - 2024-09-26
### Fixed
- 修复CosXml.dll文件冲突的问题
- 优化Multiverse镜像Build成功后的提示信息

## [2.2.3] - 2024-09-10
### Added
- 支持在ExternalLogin中指定RealmID

## [2.2.2] - 2024-09-09
### Changed
- 优化 UOSLauncher 资源加载

## [2.2.1] - 2024-09-09
### Added
- 增加崩溃堆栈还原SDK

## [2.2.0] - 2024-09-05
### Added
- 新增 Push SDK

## [2.1.4] - 2024-09-02
### Changed
- 新增GetTokenInfo方法

## [2.1.3] - 2024-08-28
### Changed
- 修复AuthTokenManager中RefreshToken的问题

## [2.1.2] - 2024-08-23
### Added
- 增加崩溃堆栈还原SDK
- 修复传入UosAppID时的Auth问题

## [2.1.1] - 2024-08-15
### Fixed
- 修复生成Multiverse Image的问题

## [2.1.0] - 2024-08-14
### Added
- 支持 Unity 小游戏平台

## [2.0.32] - 2024-08-13
### Added
- 支持团结引擎小游戏平台

## [2.0.30] - 2024-08-08
### Updated
- 调整 UOS 服务列表顺序

## [2.0.29] - 2024-08-07
### Added
- 上线 Hello SDK

## [2.0.28] - 2024-08-07
### Fixed
- 修复 Passport 服务开启问题

## [2.0.26] - 2024-07-29
### Fixed
- UOS Launcher 适配 Unity 2023.2 及更新版本

## [2.0.25] - 2024-07-25
### Fixed
- 修复服务开启问题

## [2.0.24] - 2024-07-17
### Added
- 支持更新 UOS APP 信息

## [2.0.23] - 2024-07-02
### Added
- Launcher 增加按钮 loading 状态提示
- Launcher 增加 SDK 示例项目导入功能

## [2.0.22] - 2024-06-25
### Added
- Launcher APP 信息增加遮罩，提高安全性

## [2.0.21] - 2024-06-07
### Fixed
- 更新 UOS 开发者门户跳转链接

## [2.0.20] - 2024-06-07
### Added
- 上线 Func Stateless SDK
- 增加 Sync SDK Endpoint地址

## [2.0.19] - 2024-06-04
### Fixed
- 修复 2020 版本编辑器 SDK 下载问题
- External Login 支持用户昵称

## [2.0.18] - 2024-05-29
### Added
- 新增UOS Help窗口

## [2.0.17] - 2024-05-27
### Updated
- 优化Multiverse模块Build参数的存储方式

## [2.0.16] - 2024-05-27
### Fixed
- 部分版本编辑器 iOS 构建失败问题 

## [2.0.15] - 2024-05-24
### Added
- 兼容 2020 版本编辑器

## [2.0.14] - 2024-05-23
### Updated
- Multiverse模块Build Image时，将同步使用Build Settings中的Scenes配置

## [2.0.13] - 2024-05-13
### Added
- 卸载 Launcher 时自动移除宏 UNITY_UOS_SECURITY

## [2.0.12] - 2024-05-11
### Fixed
- 添加宏 UNITY_UOS_SECURITY

## [2.0.11] - 2024-05-10
### Fixed
- 修复程序集编译平台问题
## [2.0.10] - 2024-05-09
- Changed
- 删除宏 UNITY_UOS_SECURITY
- 修改菜单逻辑
- 
## [2.0.9] - 2024-04-30
### Fixed
- 修复配置文件损坏问题
- 修复 Dedicated Server 宏添加问题
- 在重新生成AccessToken的时候调用SaveToken

## [2.0.8] - 2024-04-23
### Fixed
- 修复 AppID 等配置丢失问题
- 修复 Encrypt Key 变化导致解析失败问题

## [2.0.7] - 2024-04-16
### Changed
- 统一API Endpoint地址
- 新增过时 SDK 强制升级提示

## [2.0.6] - 2024-4-12
### Fixed
- 修复AuthTokenManager RefreshToken的并发问题
### Changed
- 删除 UNITY_UOS_COMMON 条件编译

## [2.0.5] - 2024-4-7
### Added
- UOS Launcher 新增浅色主题
### Fixed
- 增强安全性
- 修复偶现绘制失败问题
- 优化Multiverse Build Image界面

## [2.0.4] - 2024-4-2
### Updated
- 给HttpClient 添加 LogEnable 参数

## [2.0.3] - 2024-4-2
### Fixed
- 修复 Passport 版本升级提示问题

## [2.0.2] - 2024-4-1
### Added
- JwtAuthenticator支持添加Nonce Header

## [2.0.1] - 2024-3-30
### Fixed
- 修复 CRUD - Save 依赖下载失败问题

## [2.0.0] - 2024-3-29
### Added
- Multiverse模块支持一键Build Dedicated Server并上传制作镜像
- 引入 Passport Login & Features SDK
- 所有SDK统一的Auth/Token管理

## [1.2.2] - 2024-3-28
### Added
- 升级 dll 依赖

## [1.2.1] - 2024-3-27
### Added
- 更新 CRUD - Save 依赖配置

## [1.2.0] - 2024-3-22
### Added
- UI 界面升级
- 新增服务 SDK 升级提示

### Refactor
- 迁移UOS服务配置

## [1.1.4] - 2024-3-19
### Added
- 新增 Func Stateful 服务
- 新增服务依赖管理

## [1.1.3] - 2024-3-14
### Added
- 网络模块新增 grpc 反序列化功能

## [1.1.2] - 2024-3-12
### Added
- 新增 Multiverse SDK 和 Matchmaking Server SDK
### Refactored
- 重构网络模块，新增 API token 管理

## [1.1.1] - 2024-3-6
### Added
- 支持更换加密密钥

## [1.1.0] - 2024-1-31
### Added
- 新增 AppID 和 AppSecret 管理
- 新增 UOS APP 关联，支持开启 UOS 服务
- 新增 UOS SDK 公用模块

### Refactored
- 使用 UI Toolkit 绘制窗口界面
- 重构项目结构

## [1.0.2] - 2024-1-24
### Added
- 新增 Save 服务

## [1.0.1] - 2024-1-24
### Fixed
- 修复部分图标问题

## [1.0.0] - 2023-12-20
### Added
- UOS Launcher 初版