using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace Unity.UOS.Launcher.UI
{
    public class ServiceIcon
    {
        private static readonly Dictionary<string, Texture> IconDictionary = new Dictionary<string, Texture>();

        static ServiceIcon()
        {
            // 初始化图标
            foreach (var name in KnownService.ServiceList)
            {
                if (!IconDictionary.ContainsKey(name))
                {
                    Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>($"{PackageResourcesManager.GetPackageRelativePath()}/Editor/Sprites/Services/{name}.png");
                    if (sprite?.texture != null)
                    {
                        IconDictionary.Add(name, sprite.texture);
                    }
                }
            }
        }

        public static Texture2D GetIcon(string name)
        {
            if (IconDictionary.TryGetValue(name, out var texture))
            {
                return (Texture2D)texture;
            }
            // 匹配前缀
            foreach (var kvp in IconDictionary)
            {
                if (name.StartsWith(kvp.Key))
                {
                    return (Texture2D)kvp.Value;
                }
            }
            return null;
        }
    }
}