using System;
using System.IO;
using System.Collections.Generic;
using Unity.UOS.COSXML.Model.Tag;
using Unity.UOS.COSXML.Transfer;

namespace Unity.UOS.COSXML.Model.CI
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class SubmitCensorJobResult : CosDataResult<CensorJobsResponse>
    {

        /// <summary>
        /// 图片审核结果
        /// </summary>
        /// <value></value>
        public CensorJobsResponse censorJobsResponse { 
            get {return _data; } 
        }
    }
}
