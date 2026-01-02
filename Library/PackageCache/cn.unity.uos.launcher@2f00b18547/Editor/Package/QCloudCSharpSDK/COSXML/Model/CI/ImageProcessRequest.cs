using System;
using System.Collections.Generic;

using System.Text;
using Unity.UOS.COSXML.Common;
using Unity.UOS.COSXML.Model.Object;
using Unity.UOS.COSXML.CosException;

namespace Unity.UOS.COSXML.Model.CI
{
    public sealed class ImageProcessRequest : ObjectRequest
    {
        public ImageProcessRequest(string bucket, string key, string operationRules)
            : base(bucket, key)
        {

            if (operationRules == null)
            {
                throw new CosClientException((int)CosClientError.InvalidArgument, "operationRules = null");
            }

            this.method = CosRequestMethod.POST;
            this.queryParameters.Add("image_process", null);
            this.headers.Add("Pic-Operations", operationRules);
        }

    }
}
