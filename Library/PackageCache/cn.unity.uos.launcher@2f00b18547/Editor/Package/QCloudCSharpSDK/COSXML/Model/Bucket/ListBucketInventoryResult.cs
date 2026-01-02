using Unity.UOS.COSXML.Model.Tag;
using Unity.UOS.COSXML.Transfer;


namespace Unity.UOS.COSXML.Model.Bucket
{
    public sealed class ListBucketInventoryResult : CosDataResult<ListInventoryConfiguration>
    {
        public ListInventoryConfiguration listInventoryConfiguration { 
            get {return _data; } 
        }
    }
}
