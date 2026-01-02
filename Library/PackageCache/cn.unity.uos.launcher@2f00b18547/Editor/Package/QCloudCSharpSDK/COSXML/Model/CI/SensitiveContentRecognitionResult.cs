using System;
using System.IO;
using System.Collections.Generic;
using Unity.UOS.COSXML.Model.Tag;
using Unity.UOS.COSXML.Transfer;

namespace Unity.UOS.COSXML.Model.CI
{
    /// <summary>
    /// 图片审核结果
    /// </summary>
    public sealed class SensitiveContentRecognitionResult : CosDataResult<SensitiveRecognitionResult>
    {

        /// <summary>
        /// 图片审核结果
        /// </summary>
        /// <value></value>
        public SensitiveRecognitionResult recognitionResult { 
            get {return _data; } 
        }
    }
}
