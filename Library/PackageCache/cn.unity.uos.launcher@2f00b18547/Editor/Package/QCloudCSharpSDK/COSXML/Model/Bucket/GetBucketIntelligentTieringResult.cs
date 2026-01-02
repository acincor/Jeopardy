using Unity.UOS.COSXML.Model.Tag;
using Unity.UOS.COSXML.Transfer;

namespace Unity.UOS.COSXML.Model.Bucket
{
    public sealed class GetBucketIntelligentTieringResult : CosDataResult<IntelligentTieringConfiguration>
    {
        public IntelligentTieringConfiguration configuration { 
            get{ return _data; } 
        }
    }
}
