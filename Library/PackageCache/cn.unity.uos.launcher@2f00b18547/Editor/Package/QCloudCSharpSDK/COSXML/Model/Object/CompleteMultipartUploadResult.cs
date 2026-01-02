using System;
using System.Collections.Generic;

using System.Text;
using Unity.UOS.COSXML.Model.Tag;
using Unity.UOS.COSXML.Transfer;

namespace Unity.UOS.COSXML.Model.Object
{
    /// <summary>
    /// 完成整个分块上传返回的结果
    /// <see href="https://cloud.tencent.com/document/product/436/7742"/>
    /// </summary>
    public sealed class CompleteMultipartUploadResult : CosDataResult<CompleteResult>
    {
        /// <summary>
        /// Complete返回信息
        /// <see href="Model.Tag.CompleteResult"/>
        /// </summary>
        public CompleteResult completeResult { 
            get {return _data; } 
        }
    }
}
