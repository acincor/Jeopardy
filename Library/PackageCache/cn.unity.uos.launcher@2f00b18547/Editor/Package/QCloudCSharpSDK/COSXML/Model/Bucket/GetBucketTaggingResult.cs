using System;
using System.Collections.Generic;

using System.Text;
using Unity.UOS.COSXML.Model.Tag;
using Unity.UOS.COSXML.Transfer;

namespace Unity.UOS.COSXML.Model.Bucket
{
    public sealed class GetBucketTaggingResult : CosDataResult<Tagging>
    {
        public Tagging tagging { 
            get {return _data; } 
        }
    }
}
