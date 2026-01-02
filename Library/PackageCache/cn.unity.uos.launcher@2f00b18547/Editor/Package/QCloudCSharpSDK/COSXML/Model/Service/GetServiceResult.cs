using System;
using System.Collections.Generic;

using System.Text;
using Unity.UOS.COSXML.Model.Tag;
using Unity.UOS.COSXML.Transfer;
using System.IO;

namespace Unity.UOS.COSXML.Model.Service
{
    /// <summary>
    /// 获取所有 Bucket 列表返回的结果
    /// <see href="https://cloud.tencent.com/document/product/436/8291"/>
    /// </summary>
    public sealed class GetServiceResult : CosDataResult<ListAllMyBuckets>
    {
        /// <summary>
        /// list all buckets for users
        /// <see href="COSXML.Model.Tag.ListAllMyBuckets"/>
        /// </summary>
        public ListAllMyBuckets listAllMyBuckets { 
            get {return _data; } 
        }
    }
}
