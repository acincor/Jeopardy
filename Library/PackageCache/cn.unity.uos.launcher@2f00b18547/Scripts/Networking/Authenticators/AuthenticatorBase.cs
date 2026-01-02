using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine.Networking;

namespace Unity.UOS.Networking
{
    public class AuthenticatorBase
    {
        protected readonly Dictionary<string, string> _headers = new Dictionary<string, string>();
        protected AuthenticatorBase() {}

        public virtual Task ConfigureAuth(UnityWebRequest request)
        {
            foreach (var header in _headers)
            {
                request.SetRequestHeader(header.Key, header.Value);
            }

            return Task.CompletedTask;
        }
    }
}
