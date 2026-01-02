using Unity.UOS.Networking;

namespace Unity.UOS.Common
{
    public class InitOptions
    {
        public string UosAppID;
        public string UosAppSecret;
        public int HttpClientTimeout;

        public InitOptions()
        {
            HttpClientTimeout = HttpClient.DefaultTimeout;
        }

        public InitOptions(string uosAppID, string uosAppSecret, int httpClientTimeout = HttpClient.DefaultTimeout)
        {
            UosAppID = uosAppID;
            UosAppSecret = uosAppSecret;
            HttpClientTimeout = httpClientTimeout;
        }
    }
}