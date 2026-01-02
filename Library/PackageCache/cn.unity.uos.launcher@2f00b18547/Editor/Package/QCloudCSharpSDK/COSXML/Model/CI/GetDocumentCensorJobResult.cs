using System;
using System.IO;
using System.Collections.Generic;
using Unity.UOS.COSXML.Model.Tag;
using Unity.UOS.COSXML.Transfer;

namespace Unity.UOS.COSXML.Model.CI
{
    /// <summary>
    /// 文档审核结果
    /// </summary>
    public sealed class GetDocumentCensorJobResult : CosDataResult<DocumentCensorResult>
    {
        /// <summary>
        /// 文档审核结果
        /// </summary>
        /// <value></value>
        public DocumentCensorResult resultStruct { 
            get {return _data; } 
        }
    }
}
