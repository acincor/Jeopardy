using Unity.UOS.COSXML.Model.Tag;
using Unity.UOS.COSXML.Transfer;

namespace Unity.UOS.COSXML.Model.Bucket
{
    public sealed class GetBucketDomainResult : CosDataResult<DomainConfiguration>
    {
        public DomainConfiguration domainConfiguration { 
            get{ return _data; } 
        }
    }
}
