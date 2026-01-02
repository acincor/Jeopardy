using Unity.UOS.COSXML.Model.Tag;
using Unity.UOS.COSXML.Transfer;
using System;
using System.Collections.Generic;
using System.Text;

namespace Unity.UOS.COSXML.Model.Bucket
{
    public sealed class GetBucketLoggingResult : CosDataResult<BucketLoggingStatus>
    {
        public BucketLoggingStatus bucketLoggingStatus { 
            get{ return _data; } 
        }
    }
}
