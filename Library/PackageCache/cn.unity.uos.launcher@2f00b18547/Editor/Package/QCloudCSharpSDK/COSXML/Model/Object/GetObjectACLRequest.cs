using System;
using System.Collections.Generic;

using System.Text;
using Unity.UOS.COSXML.Common;

namespace Unity.UOS.COSXML.Model.Object
{
    /// <summary>
    /// 获取某个存储桶下的某个对象的访问权限
    /// <see href="https://cloud.tencent.com/document/product/436/7744"/>
    /// </summary>
    public sealed class GetObjectACLRequest : ObjectRequest
    {
        public GetObjectACLRequest(string bucket, string key)
            : base(bucket, key)
        {
            this.method = CosRequestMethod.GET;
            this.queryParameters.Add("acl", null);
        }
    }
}
