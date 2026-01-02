using System;
using System.Collections.Generic;

using System.Text;
using Unity.UOS.COSXML.Common;

namespace Unity.UOS.COSXML.Model.Bucket
{
    public sealed class GetBucketReplicationRequest : BucketRequest
    {
        public GetBucketReplicationRequest(string bucket)
            : base(bucket)
        {
            this.method = CosRequestMethod.GET;
            this.queryParameters.Add("replication", null);
        }
    }
}
