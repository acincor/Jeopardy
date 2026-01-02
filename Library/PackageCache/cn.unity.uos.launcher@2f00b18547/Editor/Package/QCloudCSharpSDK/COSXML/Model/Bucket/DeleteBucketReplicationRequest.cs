using System;
using System.Collections.Generic;

using System.Text;
using Unity.UOS.COSXML.Common;

namespace Unity.UOS.COSXML.Model.Bucket
{
    public sealed class DeleteBucketReplicationRequest : BucketRequest
    {
        public DeleteBucketReplicationRequest(string bucket)
            : base(bucket)
        {
            this.method = CosRequestMethod.DELETE;
            this.queryParameters.Add("replication", null);
        }
    }
}
