using System;

namespace Unity.UOS.Models
{
    [Serializable]
    public class Detail
    {
        public int code;
        public string message;
        public string errCode;
        public string type;
    }

    [Serializable]
    public class UosServiceError
    {
        public int code;
        public string message;
        public Detail detail;
        
        public override string ToString()
        {
            return $"code: {code}, message: {message}";
        }
    }
}
