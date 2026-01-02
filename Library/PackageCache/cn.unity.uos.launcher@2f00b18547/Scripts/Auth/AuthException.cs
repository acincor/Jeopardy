using System;
using System.Collections.Generic;

namespace Unity.UOS.Common.UOSLauncher.Scripts.Auth
{
    public enum ErrorCode
    {
        NotInitialized,
        InvalidUosAppID,
        InvalidJwt,
        NeedLogin,
        RefreshFailed,
        GenerateTokenFailed,
        NonceTimestampExpired,
        InvalidOperation,
        PlayerBanned,
        Unknown
    }

    public class ErrorDetailBase
    {
    }

    public class PersonaBannedDetail : ErrorDetailBase
    {
        public string BanReason;
        public string PersonaID;
        public string PersonaDisplayName;
        public DateTime? UnbanAt = null;
    }

    public class AuthException : System.Exception
    {
        public ErrorCode ErrorCode { get; }
        
        public Dictionary<string, string> ErrorDetail { get; }
        
        public ErrorDetailBase DetailInfo { get; }

        public AuthException(ErrorCode errorCode, string message) : base(message)
        {
            ErrorCode = errorCode;
        }

        public AuthException(ErrorCode errorCode, string message, Dictionary<string, string> detail) : base(message)
        {
            ErrorCode = errorCode;
            ErrorDetail = detail;
            DetailInfo = ParseErrorDetail(errorCode, detail);
        }

        private static ErrorDetailBase ParseErrorDetail(ErrorCode errorCode, Dictionary<string, string> detail)
        {
            return errorCode switch
            {
                ErrorCode.PlayerBanned => ParseBannedDetail(detail),
                _ => new ErrorDetailBase()
            };
        }

        private static PersonaBannedDetail ParseBannedDetail(Dictionary<string, string> detail)
        {
            var banDetail = new PersonaBannedDetail { };
            if (detail.TryGetValue("BanReason", out var banReason))
            {
                banDetail.BanReason = banReason;
            }
            if (detail.TryGetValue("UnbanAt", out var unbanAtStr))
            {
                if (DateTime.TryParse(unbanAtStr, out var unbanAt))
                {
                    banDetail.UnbanAt = unbanAt;
                }
            }
            if (detail.TryGetValue("PersonaDisplayName", out var personaName))
            {
                banDetail.PersonaDisplayName = personaName;
            }
            if (detail.TryGetValue("PersonaID", out var personaId))
            {
                banDetail.PersonaID = personaId;
            }
            return banDetail;
        }
    }
}