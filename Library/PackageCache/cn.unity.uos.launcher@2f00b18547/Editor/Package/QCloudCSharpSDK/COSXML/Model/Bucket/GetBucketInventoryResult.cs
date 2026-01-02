using Unity.UOS.COSXML.Model.Tag;
using Unity.UOS.COSXML.Transfer;
namespace Unity.UOS.COSXML.Model.Bucket
{
    public sealed class GetBucketInventoryResult : CosDataResult<InventoryConfiguration>
    {

        public InventoryConfiguration inventoryConfiguration { 
            get{ return _data; } 
        }
    }
}
