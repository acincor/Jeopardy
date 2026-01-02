using System;
using System.Text;

using Unity.UOS.COSXML.Common;
using Unity.UOS.COSXML.Model.Tag;
using Unity.UOS.COSXML.Network;

namespace Unity.UOS.COSXML.Model.Bucket
{
    /// <summary>
    /// 检索 Bucket 是否存在
    /// <see href="https://cloud.tencent.com/document/product/436/7735"/>
    /// </summary>
    public sealed class PutBucketDomainRequest : BucketRequest
    {
        private DomainConfiguration domain;

        public PutBucketDomainRequest(string bucket, DomainConfiguration domain)
            : base(bucket)
        {
            this.method = CosRequestMethod.PUT;
            this.domain = domain;
            this.queryParameters.Add("domain", null);
        }

        public override Network.RequestBody GetRequestBody()
        {
            return GetXmlRequestBody(domain);
        }
    }
}
