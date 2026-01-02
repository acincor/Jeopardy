using System;
using System.Text;
using Newtonsoft.Json;
using Unity.UOS.Common.UOSLauncher.Scripts.Auth;

namespace Unity.UOS.Auth
{
    public struct JsonWebToken
    {
        private static readonly char[] JwtSeparator = { '.' };
        public static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);

        public DateTime Expiration { get; }
        public string UserID { get; }
        public string PersonaID { get; }

        [JsonConstructor]
        public JsonWebToken(long exp, string userID, string personaID)
        {
            Expiration = UnixEpoch.AddSeconds(exp);
            UserID = userID;
            PersonaID = personaID;
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }

        public static JsonWebToken Decode(string token)
        {
            var parts = token.Split(JwtSeparator);
            if (parts.Length != 3)
            {
                throw new AuthException(ErrorCode.InvalidJwt, "invalid jwt");
            }

            var payloadString = parts[1];
            var payloadJson = Encoding.UTF8.GetString(Base64UrlDecode(payloadString));
            var payload = JsonConvert.DeserializeObject<Payload>(payloadJson);
            return new JsonWebToken(payload.exp, payload.sub, payload.ppid);
        }

        static byte[] Base64UrlDecode(string input)
        {
            var output = input;
            output = output.Replace('-', '+'); // 62nd char of encoding
            output = output.Replace('_', '/'); // 63rd char of encoding

            var mod4 = input.Length % 4;
            if (mod4 > 0)
            {
                output += new string('=', 4 - mod4);
            }

            return Convert.FromBase64String(output);
        }
    }

    [Serializable]
    internal struct Payload
    {
        // Expiration Time
        public int exp;
        // Subject, in UOS means userId
        public string sub;
        // PersonaId
        public string ppid;
    }
}