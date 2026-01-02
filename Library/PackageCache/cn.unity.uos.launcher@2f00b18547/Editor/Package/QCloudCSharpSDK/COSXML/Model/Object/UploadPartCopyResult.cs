using System;
using System.Collections.Generic;

using System.Text;
using Unity.UOS.COSXML.Model.Tag;
using Unity.UOS.COSXML.Transfer;

namespace Unity.UOS.COSXML.Model.Object
{
    /// <summary>
    /// 分片复制返回的结果
    /// <see href="https://cloud.tencent.com/document/product/436/12633"/>
    /// </summary>
    public sealed class UploadPartCopyResult : CosDataResult<CopyPartResult>
    {
        /// <summary>
        /// 分片复制的结果信息
        /// <see href="Model.Tag.CopyObject"/>
        /// </summary>
        public CopyPartResult copyPart { 
            get {return _data; } 
        }
    }
}
