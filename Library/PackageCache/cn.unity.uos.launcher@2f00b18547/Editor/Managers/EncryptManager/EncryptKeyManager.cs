using UnityEngine;
using System;
using System.IO;
using Unity.UOS.Common;

namespace Unity.UOS.Launcher
{
    public class EncryptKeyManager
    {
        public static void GenEncryptKey()
        {
            var template = @"namespace Unity.UOS.Encrypt
{{
    public class EncryptKey
    {{
        public static string Value = ""{0}"";
    }}
}}";
            var encryptKeyPath =  PackageResourcesManager.GetResourceFullPath() + "/EncryptKey.cs";
            var uuid = Guid.NewGuid().ToString();
            Settings.ChangeEncryptKey(uuid);
            
            var content = String.Format(template, uuid);
            
            try
            {
                File.WriteAllText(encryptKeyPath, content);
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }
        }
    }
}