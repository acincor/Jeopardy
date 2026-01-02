using Unity.UOS.COSXML.Model.Tag;
using Unity.UOS.COSXML.Transfer;

namespace Unity.UOS.COSXML.Model.Bucket
{
    public sealed class GetBucketWebsiteResult : CosDataResult<WebsiteConfiguration>
    {
        public WebsiteConfiguration websiteConfiguration { 
            get {return _data; } 
        }
    }
}
