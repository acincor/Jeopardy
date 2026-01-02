using System;
using System.Collections.Generic;

using System.Text;
using Unity.UOS.COSXML.Model.Tag;
using Unity.UOS.COSXML.Transfer;

namespace Unity.UOS.COSXML.Model.Object
{
    /// <summary>
    /// 实现初始化分片上传返回的结果
    /// <see href="https://cloud.tencent.com/document/product/436/7746"/>
    /// </summary>
    public sealed class InitMultipartUploadResult : CosDataResult<InitiateMultipartUpload>
    {
        /// <summary>
        /// 返回信息
        /// <see href="Model.Tag.InitiateMultipartUpload"/>
        /// </summary>
        public InitiateMultipartUpload initMultipartUpload { 
            get {return _data;} 
        }
    }
}
