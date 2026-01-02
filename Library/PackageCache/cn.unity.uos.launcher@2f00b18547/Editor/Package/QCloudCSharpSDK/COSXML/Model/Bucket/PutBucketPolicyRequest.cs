using Unity.UOS.COSXML.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Unity.UOS.COSXML.Model.Bucket
{
    public sealed class PutBucketPolicyRequest : BucketRequest
    {
        public PutBucketPolicyRequest(string bucket)
            : base(bucket)
        {
            this.method = CosRequestMethod.PUT;
            this.queryParameters.Add("policy ", null);
        }
    }
}
