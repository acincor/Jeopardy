using System;

using System.Text;
using Unity.UOS.COSXML.Common;
using Unity.UOS.COSXML.Model.Tag;
using Unity.UOS.COSXML.Network;
using Unity.UOS.COSXML.CosException;

namespace Unity.UOS.COSXML.Model.Bucket
{
    /// <summary>
    /// 设置 Bucket 生命周期
    /// <see href="https://cloud.tencent.com/document/product/436/8280"/>
    /// </summary>
    public sealed class GetBucketIntelligentTieringRequest : BucketRequest
    {

        public GetBucketIntelligentTieringRequest(string bucket)
            : base(bucket)
        {
            this.method = CosRequestMethod.GET;
            this.queryParameters.Add("intelligenttiering", null);
        }
    }
}
