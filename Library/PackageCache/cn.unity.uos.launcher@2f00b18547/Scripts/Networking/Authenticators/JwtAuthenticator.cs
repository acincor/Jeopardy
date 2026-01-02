using System.Threading.Tasks;
using Unity.UOS.Auth;
using UnityEngine.Networking;

namespace Unity.UOS.Networking
{
    public class JwtAuthenticator: AuthenticatorBase
    {
        private readonly string _appID;

        private string _appSecret;

        // _nonceTokenAuthenticator is used to apply the nonce header in the request.
        // In some cases, the API requires both jwt and nonce validation (Such as Passport Achievement UnlockPersonaAchievement).
        private readonly NonceTokenAuthenticator _nonceTokenAuthenticator;
        public JwtAuthenticator() {}

        // This constructor is used to create a JwtAuthenticator with nonce header applied.
        public JwtAuthenticator(string appID, string appSecret)
        {
            _appID = appID;
            _appSecret = appSecret;
            _nonceTokenAuthenticator = new NonceTokenAuthenticator(appID, appSecret);
        }

        public override async Task ConfigureAuth(UnityWebRequest request)
        {
            var jwt = await AuthTokenManager.GetAccessToken(_appID);
            request.SetRequestHeader(KnownHeaders.Authorization, $"Bearer {jwt}");
            if (_nonceTokenAuthenticator != null)
            {
                await _nonceTokenAuthenticator.ConfigureAuth(request);
            }
        }
    }
}
