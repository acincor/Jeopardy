using System;
using System.Collections.Generic;

using System.Text;
using Unity.UOS.COSXML.Common;
using Unity.UOS.COSXML.Model.Object;
using Unity.UOS.COSXML.CosException;
using Unity.UOS.COSXML.Utils;

namespace Unity.UOS.COSXML.Model.CI
{
    /// <summary>
    /// 图片审核
    /// <see href="https://cloud.tencent.com/document/product/436/45434"/>
    /// </summary>
    public sealed class SensitiveContentRecognitionRequest : ObjectRequest
    {
        public SensitiveContentRecognitionRequest(string bucket, string key, string type)
            : base(bucket, key)
        {

            if (type == null)
            {
                throw new CosClientException((int)CosClientError.InvalidArgument, "type = null");
            }

            this.method = CosRequestMethod.GET;
            this.queryParameters.Add("ci-process", "sensitive-content-recognition");
            this.queryParameters.Add("detect-type", URLEncodeUtils.Encode(type));
        }

    }
}
