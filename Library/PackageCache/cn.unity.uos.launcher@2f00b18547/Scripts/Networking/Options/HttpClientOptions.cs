using System;
using System.Threading.Tasks;
using UnityEngine.Networking;

namespace Unity.UOS.Networking
{
    public class HttpClientOptions
    {
        public string SdkVersion;
        public string SdkName;
        public AuthenticatorBase Authenticator { get; set; }
        public Action<UnityWebRequest> ExceptionHandler { get; set; }

        /// <summary>
        /// Base URL for all requests made with this client instance
        /// </summary>
        public Uri BaseUrl = null;

        public int Timeout = HttpClient.DefaultTimeout;

        public async Task ConfigRequest(UnityWebRequest request)
        {
            if (Authenticator != null)
            {
                await Authenticator.ConfigureAuth(request);
            }

            if (!string.IsNullOrEmpty(SdkName))
            {
                request.SetRequestHeader(KnownHeaders.XSdkName, SdkName);
            }

            if (!string.IsNullOrEmpty(SdkVersion))
            {
                request.SetRequestHeader(KnownHeaders.XSdkVersion, SdkVersion);
            }
        }
    }
}
