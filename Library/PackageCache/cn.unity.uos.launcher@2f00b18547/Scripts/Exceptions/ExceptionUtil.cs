using System;
using Newtonsoft.Json;
using Unity.UOS.Models;
using UnityEngine;
using UnityEngine.Networking;

namespace Unity.UOS.Exceptions
{
    public class ExceptionUtil<TClientException, TServerException>
        where TClientException : Exception
        where TServerException : Exception
    {
        public static readonly Action<UnityWebRequest> ExceptionHandler = request =>
        {
            try
            {
                if (!string.IsNullOrEmpty(request?.downloadHandler?.text))
                {
                    var uosServiceError = JsonConvert.DeserializeObject<UosServiceError>(request.downloadHandler.text);

                    if (request.responseCode >= 500)
                    {
                        throw (TServerException)Activator.CreateInstance(typeof(TServerException), ErrorCode.Unknown, uosServiceError.ToString(), null);
                    }

                    var code = request.responseCode switch
                    {
                        401 => ErrorCode.InvalidToken,
                        403 => ErrorCode.Forbidden,
                        404 => ErrorCode.NotFound,
                        _ => ErrorCode.InvalidRequest
                    };
                    throw (TClientException)Activator.CreateInstance(typeof(TClientException), code, uosServiceError.ToString(), null);
                }

                if (!string.IsNullOrEmpty(request?.error))
                {
                    switch (request.result)
                    {
                        case UnityWebRequest.Result.ConnectionError:
                            throw (TClientException)Activator.CreateInstance(typeof(TClientException), ErrorCode.NetworkError, $"Network connection error: {request.error}", null);

                        case UnityWebRequest.Result.DataProcessingError:
                            throw (TClientException)Activator.CreateInstance(typeof(TClientException), ErrorCode.Unknown, $"Data processing error: {request.error}", null);

                        case UnityWebRequest.Result.ProtocolError:
                            throw (TServerException)Activator.CreateInstance(typeof(TServerException), ErrorCode.Unknown, $"Protocol error: {request.error}");
                        default:
                            Debug.LogErrorFormat("occurred unexpected error when send http request, url: {0}, method: {1}. err: {2}", request.url, request.method, request.error);
                            throw (TClientException)Activator.CreateInstance(typeof(TClientException), ErrorCode.Unknown, $"Request failed: {request.error}", null);
                    }
                }
                
                throw (TClientException)Activator.CreateInstance(typeof(TClientException), ErrorCode.Unknown, "Request failed without specific error information", null);
            }
            catch (Exception ex) when (ex is not TClientException and not TServerException)
            {
                Debug.LogErrorFormat("occurred unexpected exception when send http request, url: {0}, method: {1}, ex: {2}", request?.url, request?.method, ex);
                throw (TServerException)Activator.CreateInstance(typeof(TServerException), ErrorCode.Unknown, ex.Message, ex);
            }
        };
    }
    
    public enum ErrorCode
    {
        /// <summary>
        /// 未知错误
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// 请求超时
        /// </summary>
        Timeout,

        /// <summary>
        /// 服务不可用
        /// </summary>
        ServiceUnavailable,

        /// <summary>
        /// 拒绝服务
        /// </summary>
        RequestRejected,

        /// <summary>
        /// 无效的 Token
        /// </summary>
        InvalidToken,

        /// <summary>
        /// Token 过期
        /// </summary>
        TokenExpired,

        /// <summary>
        /// 权限不足
        /// </summary>
        Forbidden,

        /// <summary>
        /// 资源未找到
        /// </summary>
        NotFound,

        /// <summary>
        /// 无效的请求
        /// </summary>
        InvalidRequest,
        
        /// <summary>
        /// 网络错误
        /// </summary>
        NetworkError
    }
}
