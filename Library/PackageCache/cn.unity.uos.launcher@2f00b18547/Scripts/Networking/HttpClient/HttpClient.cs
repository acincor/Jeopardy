using System.Collections.Generic;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Unity.UOS.Networking
{
    public class HttpClient
    {
        private const string TraceIdHeader = "Grpc-Metadata-X-Trace-Id";
        public const int DefaultTimeout = 30; // 单位：秒

        private readonly HttpClientOptions _options;
        public HttpClient(HttpClientOptions options)
        {
            _options = options;
        }

        public async Task<T> Get<T>(string url, Dictionary<string, object> queryParams = null,
            Dictionary<string, string> headers = null, int timeout = 0)
        {
            return await Request<T>(url, UnityWebRequest.kHttpVerbGET, "", queryParams, headers, timeout);
        }

        public async Task<T> Post<T>(string url, string data = "", Dictionary<string, object> queryParams = null,
            Dictionary<string, string> headers = null, int timeout = 0)
        {
            return await Request<T>(url, UnityWebRequest.kHttpVerbPOST, data, queryParams, headers, timeout);
        }

        public async Task<T> Put<T>(string url, string data = "", Dictionary<string, object> queryParams = null,
            Dictionary<string, string> headers = null, int timeout = 0)
        {
            return await Request<T>(url, UnityWebRequest.kHttpVerbPUT, data, queryParams, headers, timeout);
        }

        public async Task<T> Delete<T>(string url, string data = "", Dictionary<string, object> queryParams = null,
            Dictionary<string, string> headers = null, int timeout = 0)
        {
            return await Request<T>(url, UnityWebRequest.kHttpVerbDELETE, data, queryParams, headers, timeout);
        }

        public async Task<T> Patch<T>(string url, string data = "", Dictionary<string, object> queryParams = null, 
            Dictionary<string, string> headers = null, int timeout = 0)
        {
            return await Request<T>(url, "PATCH", data, queryParams, headers, timeout);
        }

        public async Task<T> Request<T>(
            string url,
            string method,
            string data = "",
            Dictionary<string, object> queryParams = null,
            Dictionary<string, string> headers = null,
            int timeout = 0,
            bool? logEnable = null
        )
        {
#if UOS_LOG_LEVEL_INFO 
            bool actualLogEnable = logEnable ?? true;
#else
            bool actualLogEnable = logEnable ?? false;
#endif
            var requestUrl = ConstructUrl(url, queryParams);
            using var uwr = new UnityWebRequest(requestUrl, method);
            await _options.ConfigRequest(uwr);

            if (headers != null)
            {
                foreach (var kvp in headers)
                {
                    uwr.SetRequestHeader(kvp.Key, kvp.Value);
                }
            }

            uwr.disposeDownloadHandlerOnDispose = true;
            uwr.disposeUploadHandlerOnDispose = true;
            
            if (data != "")
            {
                uwr.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(data));
            }
            uwr.downloadHandler = new DownloadHandlerBuffer();
            uwr.timeout = timeout > 0 ? timeout : _options.Timeout;

            if (actualLogEnable)
            {
                Debug.Log($"UOS Request {method} {url}");
            }
            var sendOperation = uwr.SendWebRequest();
            while (!sendOperation.isDone)
            {
                await Task.Yield();
            }

            var traceId = uwr.GetResponseHeader(TraceIdHeader);
            if (actualLogEnable && !string.IsNullOrEmpty(traceId))
            {
                Debug.Log($"UOS Request: {method} {url}, Response TraceId: {traceId}");
            }
            if (uwr.error != null)
            {
                if (_options.ExceptionHandler != null)
                {
                    _options.ExceptionHandler(uwr);
                }
                else
                {
                    throw new Exception(uwr.downloadHandler.text);
                }
            }

            var settings = new JsonSerializerSettings
            {
                ContractResolver = new TimestampContractResolver()
            };
            return JsonConvert.DeserializeObject<T>(uwr.downloadHandler.text, settings);
        }

        public Task<UnityWebRequest> GetRaw(string url, Dictionary<string, object> queryParams = null,
            Dictionary<string, string> headers = null, int timeout = 0)
        {
            return RequestRaw(url, UnityWebRequest.kHttpVerbGET, "", queryParams, headers, timeout);
        }

        public Task<UnityWebRequest> PostRaw(string url, string data = "", Dictionary<string, object> queryParams = null,
            Dictionary<string, string> headers = null, int timeout = 0)
        {
            return RequestRaw(url, UnityWebRequest.kHttpVerbPOST, data, queryParams, headers, timeout);
        }

        public Task<UnityWebRequest> PutRaw(string url, string data = "", Dictionary<string, object> queryParams = null,
            Dictionary<string, string> headers = null, int timeout = 0)
        {
            return RequestRaw(url, UnityWebRequest.kHttpVerbPUT, data, queryParams, headers, timeout);
        }

        public Task<UnityWebRequest> DeleteRaw(string url, string data = "", Dictionary<string, object> queryParams = null,
            Dictionary<string, string> headers = null, int timeout = 0)
        {
            return RequestRaw(url, UnityWebRequest.kHttpVerbDELETE, data, queryParams, headers, timeout);
        }

        public Task<UnityWebRequest> PatchRaw(string url, string data = "", Dictionary<string, object> queryParams = null,
            Dictionary<string, string> headers = null, int timeout = 0)
        {
            return RequestRaw(url, "PATCH", data, queryParams, headers, timeout);
        }

        public async Task<UnityWebRequest> RequestRaw(
            string url,
            string method,
            string data = "",
            Dictionary<string, object> queryParams = null,
            Dictionary<string, string> headers = null,
            int timeout = 0)
        {
            var requestUrl = ConstructUrl(url, queryParams);
            var uwr = new UnityWebRequest(requestUrl, method);
            await _options.ConfigRequest(uwr);

            if (headers != null)
            {
                foreach (var kvp in headers)
                {
                    uwr.SetRequestHeader(kvp.Key, kvp.Value);
                }
            }

            uwr.disposeDownloadHandlerOnDispose = true;
            uwr.disposeUploadHandlerOnDispose = true;

            if (data != "")
            {
                uwr.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(data));
            }
            uwr.downloadHandler = new DownloadHandlerBuffer();
            uwr.timeout = timeout > 0 ? timeout : _options.Timeout;

            var sendOperation = uwr.SendWebRequest();
            while (!sendOperation.isDone)
            {
                await Task.Yield();
            }

            return uwr;
        }

        private string ConstructUrl(string url, Dictionary<string, object> queryParams)
        {
            var requestUrl = url;
            if (_options.BaseUrl != null)
            {
                requestUrl = new Uri(_options.BaseUrl, url).ToString();
            }

            if (queryParams != null)
            {
                var str2 = string.Join("&", queryParams.Where(kv => kv.Value != null).Select(kv =>
                {
                    if (!kv.Value.GetType().IsArray)
                    {
                        return kv.Key + "=" + Uri.EscapeDataString(kv.Value.ToString());
                    }

                    if (!(kv.Value is IEnumerable values))
                    {
                        return "";
                    }

                    return string.Join("&",
                        values.Cast<object>().Select(value => kv.Key + "=" + Uri.EscapeDataString(value.ToString())));
                }));
                requestUrl += $"?{str2}";
            }

            return requestUrl;
        }
    }
}
