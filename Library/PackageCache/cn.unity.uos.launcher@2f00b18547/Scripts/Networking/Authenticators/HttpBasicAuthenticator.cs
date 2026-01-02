using System.Text;

namespace Unity.UOS.Networking
{
    public class HttpBasicAuthenticator: AuthenticatorBase
    {
        public HttpBasicAuthenticator(string username, string password)
        {
            var authorizationToken =  "Basic " + System.Convert.ToBase64String(Encoding.GetEncoding(28591)
                .GetBytes(username + ":" + password));
            _headers.Add(KnownHeaders.Authorization, authorizationToken);
        }
    }
}
