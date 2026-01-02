using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Unity.UOS.COSXML;
using Unity.UOS.COSXML.Auth;
using Unity.UOS.COSXML.Model.Object;
using Unity.UOS.COSXML.Model.Tag;
using System.Threading.Tasks;
using UnityEditor.Build.Reporting;
using Unity.UOS.Common;
using Unity.UOS.Launcher;
#if UNITY_2021_3_OR_NEWER
namespace Unity.UOS.Launcher.UI
{
    [Serializable]
    public class MultiverseImage : EditorWindow
    {
        public Vector2 scrollPosition;
        
        private string _buildFolderPath;
        private string _imageTag;
        private string _imageId;
        private string _validateInfo;
        private string _appName;
        private bool _success;
        private bool _serviceEnabled;
        private const string BaseUrl = "https://uos.unity.cn";
        private const string ExeFilename = "server.x86_64";

        void OnGUI()
        {
            if (_serviceEnabled)
            {
                InitInfo();
            }
            else
            {
                GUILayout.Space(4);
                GUILayout.Label("Please enable Multiverse Service first");
                GUILayout.Space(8);

                if (GUILayout.Button("Open Launcher", GUILayout.Width(100)))
                {
                    LauncherWindow.Open();
                }

            }
        }

        async void OnEnable()
        {
            _success = false;
            _serviceEnabled = LauncherWindow.MultiverseEnabled();
            _validateInfo = "";
            _imageTag = UnityEngine.PlayerPrefs.GetString("MultiverseImageTag");
            _buildFolderPath = UnityEngine.PlayerPrefs.GetString("MultiverseBuildFolder");
            try
            {
                var appInfo = await API.GetUosApp(Settings.AppID, Settings.AppServiceSecret);
                _appName = appInfo.ProjectName;
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
        }

        private void InitInfo()
        {
            var helpIcon = EditorGUIUtility.IconContent("_Help").image;
            GUILayout.Space(4);
            GUILayout.BeginHorizontal();
            GUILayout.Label("You can build Linux dedicated server and generate Multiverse image with one click");
            var tooltip =
                "Scenes config can be set in [ File -> Build Settings -> Scenes In Build ]. The config will be synchronized here.";
            GUILayout.Label(new GUIContent(helpIcon, tooltip));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(8);
            
            GUILayout.BeginHorizontal();
            GUILayout.Label("Current UOS APP", GUILayout.Width(150));
            var uosApp = string.IsNullOrEmpty(_appName) ? Settings.AppID : _appName;
            GUILayout.Label(uosApp);
            GUILayout.EndHorizontal();
            GUILayout.Space(2);

            GUILayout.BeginHorizontal();
            GUILayout.BeginHorizontal(GUILayout.Width(150));
            GUILayout.Label("Target Directory");
            GUILayout.Label(new GUIContent(helpIcon, "Choose the directory to build dedicated server. The folder of dedicated server will be uploaded and used to create Multiverse image."));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            using (new EditorGUI.DisabledGroupScope(true))
            {
                _buildFolderPath = GUILayout.TextField(_buildFolderPath, GUILayout.MaxWidth(position.width - 230));
            }
            GUILayout.Space(4);
            if (GUILayout.Button("Choose", GUILayout.Width(60)))
            {
                var folderPath = EditorUtility.OpenFolderPanel("Choose Location of Built Game", "", "");
                if (!string.IsNullOrEmpty(folderPath))
                {
                    _buildFolderPath = folderPath;
                }
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(2);

            GUILayout.BeginHorizontal();
            GUILayout.BeginHorizontal(GUILayout.Width(150));
            GUILayout.Label("Image Tag");
            GUILayout.Label(new GUIContent(helpIcon, "Tag should be composed of lowercase letters, numbers and [.-], starting and ending with lowercase letters or numbers"));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            _imageTag = GUILayout.TextField(_imageTag);
            GUILayout.EndHorizontal();
            GUILayout.Space(16);
            
            if (GUILayout.Button("Build Image", GUILayout.Height(32)))
            {
                RunBuild(_buildFolderPath, _imageTag);
            }
            GUILayout.Space(16);
            
            if (_success)
            {
                GUILayout.BeginHorizontal();
                var sprite = AssetDatabase.LoadAssetAtPath<Sprite>($"{PackageResourcesManager.GetPackageRelativePath()}/Editor/Sprites/Success.png");
                GUILayout.Label(new GUIContent(sprite.texture), GUILayout.Width(16), GUILayout.Height(16));
                GUILayout.Label("Build and upload success");
                GUILayout.EndHorizontal();
                GUILayout.Space(8);
                
                GUILayout.BeginHorizontal();
                GUILayout.Label("Executable Filename", GUILayout.Width(150));
                GUILayout.Label(ExeFilename, GUILayout.Width(100));
                if (GUILayout.Button("Copy", GUILayout.Width(60)))
                {   
                    EditorGUIUtility.systemCopyBuffer = ExeFilename;
                }
                if (GUILayout.Button("Show in Explorer", GUILayout.Width(120)))
                {
                    Application.OpenURL($"file:///{_buildFolderPath + '/' + _imageTag}");
                }
                GUILayout.EndHorizontal();
            }
            else if (!string.IsNullOrEmpty(_validateInfo))
            {
                var s = new GUIStyle(EditorStyles.label);
                s.normal.textColor = Color.red;
                s.wordWrap = true;
                GUILayout.Label(_validateInfo, s);
            }
        }

        public async void RunBuild(string path, string tag)
        {
            _success = false;
            if (string.IsNullOrEmpty(path) || string.IsNullOrEmpty(tag))
            {
                _validateInfo = "Target folder and image tag cannot be empty";
                return;
            }
            if (!Directory.Exists(path))
            {
                _validateInfo = "Target folder is not exist";
                return;
            }
            if (!CheckTagName(tag))
            {
                _validateInfo = "Image tag format is invalid.\nTag should be composed of lowercase letters, numbers and [.-], starting and ending with lowercase letters or numbers.";
                return;
            }

            _validateInfo = "";

            SaveSettings();

            var dirPath = path + "/" + tag;
            if (!Directory.Exists(dirPath))
            {
                try
                {
                    Directory.CreateDirectory(dirPath);
                }
                catch (Exception e)
                {
                    Debug.LogError(e.Message);
                    return;
                }
            }
            var success = BuildServer(dirPath);
            var failedInfo = "build dedicated server";
            if (success)
            {
                success = await BuildImage(dirPath, tag);
                EditorUtility.ClearProgressBar();
                if (!success)
                {
                    failedInfo = "build image";
                }
            }

            _success = success;
            if (success)
            {
                var message =
                    "Dedicated server has been built and uploaded successfully.\nMultiverse is making docker image. You can visit UOS Dev-portal to track the process.";
                if (EditorUtility.DisplayDialog("Multiverse Image", message, "Dev Portal", "OK"))
                {
                    Application.OpenURL($"{BaseUrl}/services/{Settings.AppID}/multiverse/profiles");
                }
            }
            else
            {
                EditorUtility.DisplayDialog("Multiverse Image","Failed to " + failedInfo, "OK");
            }
        }
        
        private bool BuildServer(string path)
        {
            var buildPlayerOptions = new BuildPlayerOptions();
            buildPlayerOptions.target = BuildTarget.StandaloneLinux64;
            buildPlayerOptions.subtarget = (int)StandaloneBuildSubtarget.Server;
            buildPlayerOptions.options = BuildOptions.None;
            buildPlayerOptions.locationPathName = path + "/" + ExeFilename;
            buildPlayerOptions.scenes = EditorBuildSettings.scenes.Where(x => x.enabled).Select(x => x.path).ToArray();
            
            var report = BuildPipeline.BuildPlayer(buildPlayerOptions);
            var summary = report.summary;
            
            if (summary.result == BuildResult.Succeeded)
            {
                // Debug.Log("Build succeeded: " + summary.totalSize + " bytes");
                return true;
            }

            return false;
        }

        private async Task<bool> BuildImage(string path, string tag)
        {
            try
            {
                var zipFile = path + ".zip";
                SetProcess("Compressing file", 0);
                CompressFile(path, zipFile);
                
                SetProcess("Uploading file", (float)0.1);
                var token = await API.GetImageToken();
                var url = UploadFile(token, zipFile, (float)0.1, (float)0.95);
                File.Delete(zipFile);

                SetProcess("Creating image", (float)0.99);
                var gameImage = await CreateImage(url, tag, token.objectName);
                return !string.IsNullOrEmpty(gameImage.imageId);
                
                //return await WaitImageSuccess();
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
                return false;
            }
        }
        
        private void CompressFile(string dirPath, string destFile)
        {
            System.IO.Compression.ZipFile.CreateFromDirectory(dirPath, destFile);
            if (!File.Exists(destFile))
            {
                throw new Exception("Failed to compress folder: " + dirPath);
            }
        }

        private string UploadFile(GetImageTransferTokenResponse token, string file, float startProcess, float endProcess)
        {
            try
            {
                var config = new CosXmlConfig.Builder().SetRegion(token.region).Build();
                var cosCredentialProvider = new DefaultSessionQCloudCredentialProvider(
                    token.tmpSecretId, token.tmpSecretKey, token.startTime, DateTimeToUnix(token.expiredTime), token.token);
                var cosXml = new CosXmlServer(config, cosCredentialProvider);

                var request = new PutObjectRequest(token.bucket, token.objectName, file);
                request.SetCosProgressCallback(delegate (long completed, long total)
                {
                    SetProcess("Uploading file", startProcess + (endProcess-startProcess) * completed / total);
                });
                cosXml.PutObject(request);
                
                var preSignatureStruct = new PreSignatureStruct();
                preSignatureStruct.appid = token.bucket.Split('-')[^1];
                preSignatureStruct.region = token.region; 
                preSignatureStruct.bucket = token.bucket;
                preSignatureStruct.key = token.objectName;
                preSignatureStruct.httpMethod = "GET";
                preSignatureStruct.signDurationSecond = 600;
                preSignatureStruct.headers = null;
                preSignatureStruct.queryParameters = null;
                return cosXml.GenerateSignURL(preSignatureStruct);
            }
            catch (COSXML.CosException.CosClientException clientEx)
            {
                throw new Exception("CosClientException: " + clientEx);
            }
            catch (COSXML.CosException.CosServerException serverEx)
            {
                throw new Exception("CosServerException: " + serverEx.GetInfo());
            }
        }

        private async Task<GameImage> CreateImage(string url, string imageName, string objectName)
        {
            var res =  await API.CreateImage(url, imageName, objectName);
            return res.gameImage;
        }
        private long DateTimeToUnix(string time)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var timeSpan = DateTime.Parse(time).ToUniversalTime() - epoch;
            return (long)timeSpan.TotalSeconds;
        }

        private async Task<bool> WaitImageSuccess()
        {
            var finished = false;
            var count = 100;
            var status = "";
            while (!finished && count > 0)
            {
                status = await GetImageStatus();
                finished = status != "processing";
                count--;
                if (!finished)
                {
                    await Task.Delay(2000);
                }
            }
            
            return status == "success";
        }
        
        private async Task<string> GetImageStatus()
        {
            var res = await API.GetImage(_imageId);
            var gameImage = res.gameImage;
            switch (gameImage.imageStatus)
            {
                case "pending":
                    // Debug.Log("正在等待");
                    SetProcess("Pending", (float)0.55);
                    break;
                case "clone":
                    // Debug.Log("正在克隆");
                    SetProcess("Cloning", (float)0.6);
                    break;
                case "download":
                    // Debug.Log("正在下载");
                    SetProcess("Downloading", (float)0.65);
                    break;
                case "check":
                    // Debug.Log("正在检测镜像");
                    SetProcess("Checking image", (float)0.7);
                    break;
                case "chmod":
                    // Debug.Log("正在修改文件权限");
                    SetProcess("Doing chmod", (float)0.75);
                    break;
                case "decompress":
                    // Debug.Log("正在解压缩");
                    SetProcess("Decompressing", (float)0.8);
                    break;
                case "prepare_dockerfile":
                    // Debug.Log("正在准备docker文件");
                    SetProcess("Preparing docker file", (float)0.85);
                    break;
                case "build_docker_image":
                    // ebug.Log("正在制作镜像");
                    SetProcess("Building docker image", (float)0.9);
                    break;
                case "success":
                    // Debug.Log("镜像制作成功");
                    SetProcess("Building docker image", 1);
                    return "success";
                case "failure":
                    // Debug.Log("制作失败");
                    return "failed";
                case "error":
                    Debug.Log("内部错误，请重试");
                    return "failed";
            }

            return "processing";
        }
        
        private void SetProcess(string info, float process)
        {
            EditorUtility.DisplayProgressBar("Build Image", info, process);
        }

        private void ClearProcess()
        {
            EditorUtility.ClearProgressBar();
        }

        private bool CheckTagName(string text)
        {
            var pattern = @"^[a-z0-9][a-z0-9\[\.\-\]]*[a-z0-9]$";
            return System.Text.RegularExpressions.Regex.IsMatch(text, pattern);
        }

        private void SaveSettings()
        {
             UnityEngine.PlayerPrefs.SetString("MultiverseImageTag", _imageTag);
             UnityEngine.PlayerPrefs.SetString("MultiverseBuildFolder", _buildFolderPath);
        }
    }
}
#endif