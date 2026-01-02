using System;
using UnityEngine;
using System.Security.Cryptography;

namespace Unity.UOS.Security
{
    public class PersistManager
    {
        private static string _saltForKey;
        
        private static int blockSize = 128;
        private static int _hashLen = 32;

        static PersistManager()
        {
            byte[] saltBytes = new byte[] { 25, 36, 77, 51, 43, 14, 75, 93 };
            string randomSeedForKey = "5b6fcb4aaa0a42acae649eba45a506ec";
            {
                Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(randomSeedForKey, saltBytes, 1000);
                _saltForKey = System.Convert.ToBase64String(key.GetBytes(blockSize / 8));
            }
        }

        private static string MakeHash(string original)
        {
            using (MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider())
            {
                byte[] bytes = System.Text.Encoding.UTF8.GetBytes(original);
                byte[] hashBytes = md5.ComputeHash(bytes);

                string hashToString = "";
                for (int i = 0; i < hashBytes.Length; ++i)
                    hashToString += hashBytes[i].ToString("x2");

                return hashToString;
            }
        }

        private static void SetSecurityValue(string key, string value)
        {
            string hideKey = MakeHash(key + _saltForKey);
#if UNITY_UOS_SECURITY
            value = Encrypt.EncryptManager.Encrypt(value + MakeHash(value));
#endif
            #if UNITY_EDITOR
            UnityEngine.PlayerPrefs.SetString(hideKey, value);
            #else
            PlayerPrefs.SetString(hideKey, value);
            #endif
        }

        private static string GetSecurityValue(string key)
        {
            string hideKey = MakeHash(key + _saltForKey);

            #if UNITY_EDITOR
            string value = UnityEngine.PlayerPrefs.GetString(hideKey);
            #else
            string value = PlayerPrefs.GetString(hideKey);
            #endif
            if (string.IsNullOrEmpty(value))
                return string.Empty;
#if UNITY_UOS_SECURITY
            try
            {
                value = Encrypt.EncryptManager.Decrypt(value);
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }
            if (_hashLen > value.Length)
                return string.Empty;
#endif
            string savedValue = value.Substring(0, value.Length - _hashLen);
            string savedHash = value.Substring(value.Length - _hashLen);

            if (MakeHash(savedValue) != savedHash)
                return string.Empty;

            return savedValue;
        }

        public static void DeleteKey(string key)
        {
            #if UNITY_EDITOR
            UnityEngine.PlayerPrefs.DeleteKey(MakeHash(key + _saltForKey));
            #else
            PlayerPrefs.DeleteKey(MakeHash(key + _saltForKey));
            #endif
        }

        public static void DeleteAll()
        {
            #if UNITY_EDITOR
            UnityEngine.PlayerPrefs.DeleteAll();
            #else
            PlayerPrefs.DeleteAll();
            #endif
        }

        public static void Save()
        {
            #if UNITY_EDITOR
            UnityEngine.PlayerPrefs.Save();
            #else
            PlayerPrefs.Save();
            #endif
        }

        public static void SetInt(string key, int value)
        {
            SetSecurityValue(key, value.ToString());
        }

        public static void SetLong(string key, long value)
        {
            SetSecurityValue(key, value.ToString());
        }

        public static void SetFloat(string key, float value)
        {
            SetSecurityValue(key, value.ToString());
        }

        public static void SetString(string key, string value)
        {
            SetSecurityValue(key, value);
        }

        public static int GetInt(string key, int defaultValue)
        {
            string originalValue = GetSecurityValue(key);
            if (true == string.IsNullOrEmpty(originalValue))
                return defaultValue;

            int result = defaultValue;
            if (false == int.TryParse(originalValue, out result))
                return defaultValue;

            return result;
        }

        public static long GetLong(string key, long defaultValue)
        {
            string originalValue = GetSecurityValue(key);
            if (true == string.IsNullOrEmpty(originalValue))
                return defaultValue;

            long result = defaultValue;
            if (false == long.TryParse(originalValue, out result))
                return defaultValue;

            return result;
        }

        public static float GetFloat(string key, float defaultValue)
        {
            string originalValue = GetSecurityValue(key);
            if (true == string.IsNullOrEmpty(originalValue))
                return defaultValue;

            float result = defaultValue;
            if (false == float.TryParse(originalValue, out result))
                return defaultValue;

            return result;
        }

        public static string GetString(string key, string defaultValue)
        {
            string originalValue = GetSecurityValue(key);
            if (true == string.IsNullOrEmpty(originalValue))
                return defaultValue;

            return originalValue;
        }
    }
}