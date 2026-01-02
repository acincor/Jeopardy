using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.UOS.Networking
{
    public static class KnownHeaders
    {
        public const string Authorization = "Authorization";
        public const string XTimestamp = "X-Timestamp";
        public const string XNonce = "X-Nonce";
        public const string XNonceToken = "X-Nonce-Token";
        public const string XAppID = "X-AppID";
        public const string XSdkVersion = "X-SDK-Version";
        public const string XSdkName = "X-SDK-Name";
    }
}