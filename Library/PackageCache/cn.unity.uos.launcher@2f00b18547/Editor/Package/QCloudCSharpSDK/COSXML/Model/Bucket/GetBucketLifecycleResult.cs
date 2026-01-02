using System;
using System.Collections.Generic;

using System.Text;
using Unity.UOS.COSXML.Model.Tag;
using Unity.UOS.COSXML.Transfer;

namespace Unity.UOS.COSXML.Model.Bucket
{
    /// <summary>
    /// 查询 Bucket 的生命周期配置 返回的结果
    /// <see href="https://cloud.tencent.com/document/product/436/8278"/>
    /// </summary>
    public sealed class GetBucketLifecycleResult : CosDataResult<LifecycleConfiguration>
    {
        /// <summary>
        /// 生命周期配置信息
        /// <see href="COSXML.Model.Tag.LifecycleConfiguration"/>
        /// </summary>
        public LifecycleConfiguration lifecycleConfiguration { 
            get{ return _data; } 
        }
    }
}
