using System;
using System.Threading.Tasks;
using Unity.UOS.Security;
using UnityEngine.Networking;

namespace Unity.UOS.Networking
{
    public class UosNonceAuthenticator : AuthenticatorBase
    {
         public string UosAppID { get; set; }
         public string UosAppSecret { get; set; }

         public UosNonceAuthenticator(string appID, string appSecret)
         {
             UosAppID = appID;
             UosAppSecret = appSecret;
         }

         public override Task ConfigureAuth(UnityWebRequest request)
         {
             var nonce = Guid.NewGuid().ToString();
             var timestamp = EncryptManager.GetUnixTimeStampSeconds(DateTime.UtcNow);
             var tokenContent = $"{UosAppID}:{UosAppSecret}:{timestamp}:{nonce}";
             var token = EncryptManager.HexString(EncryptManager.Sha256(tokenContent));
             request.SetRequestHeader(KnownHeaders.XTimestamp, $"{timestamp}");
             request.SetRequestHeader(KnownHeaders.XNonce, nonce);
             request.SetRequestHeader(KnownHeaders.XAppID, UosAppID);
             request.SetRequestHeader(KnownHeaders.Authorization, $"nonce {token}");
             return Task.CompletedTask;
         }
        
    }
}
