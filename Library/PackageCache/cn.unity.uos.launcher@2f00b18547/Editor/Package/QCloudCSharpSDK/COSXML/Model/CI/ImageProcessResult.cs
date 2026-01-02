using System;
using System.IO;
using System.Collections.Generic;
using Unity.UOS.COSXML.Model.Tag;
using Unity.UOS.COSXML.Transfer;

namespace Unity.UOS.COSXML.Model.CI
{
    /// <summary>
    /// 图片处理结果
    /// </summary>
    public sealed class ImageProcessResult : CosDataResult<PicOperationUploadResult>
    {

        /// <summary>
        /// 图片处理结果
        /// </summary>
        /// <value></value>
        public PicOperationUploadResult uploadResult { 
            get {return _data; } 
        }
    }
}
