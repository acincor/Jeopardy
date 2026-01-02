using System;
using UnityEditor;
using UnityEngine;

namespace Unity.UOS.Launcher
{
    [Serializable]
    public class HelpWindow : EditorWindow
    {
        private const float ImageSize = 200;
        private const string WechatLabel = "微信扫码咨询客服";
        private const string GroupLabel = "UOS技术交流群";
        private Texture2D _wechatImg;
        private Texture2D _groupImg;
        
        void OnGUI()
        {
            {
                var labelStyle = new GUIStyle(EditorStyles.label);
                labelStyle.fontSize = 16;
                
                GUILayout.Space(20);
                GUILayout.BeginHorizontal();
                GUILayout.Space(16);

                GUILayout.BeginVertical();

                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.Label(WechatLabel, labelStyle);
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                
                GUILayout.Space(16);

                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                
                GUILayout.Label(_groupImg, GUILayout.MaxWidth(ImageSize), GUILayout.MaxHeight(ImageSize));
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                GUILayout.EndVertical();
                
                GUILayout.BeginVertical();

                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.Label(GroupLabel, labelStyle);
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                
                GUILayout.Space(16);

                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.Label(_wechatImg, GUILayout.MaxWidth(ImageSize), GUILayout.MaxHeight(ImageSize));
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                GUILayout.EndVertical();

                GUILayout.Space(16);
                GUILayout.EndHorizontal();
            }
        }

        private void OnEnable()
        {
            _groupImg = AssetDatabase.LoadAssetAtPath<Texture2D>($"{PackageResourcesManager.GetPackageRelativePath()}/Editor/Sprites/UOSWeChat.png");
            _wechatImg = AssetDatabase.LoadAssetAtPath<Texture2D>($"{PackageResourcesManager.GetPackageRelativePath()}/Editor/Sprites/UOSQQGroup.png");
        }
    }
}