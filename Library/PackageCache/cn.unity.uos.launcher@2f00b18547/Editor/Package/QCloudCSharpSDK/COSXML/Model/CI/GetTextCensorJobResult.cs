using System;
using System.IO;
using System.Collections.Generic;
using Unity.UOS.COSXML.Model.Tag;
using Unity.UOS.COSXML.Transfer;

namespace Unity.UOS.COSXML.Model.CI
{
    /// <summary>
    /// 文本审核结果
    /// </summary>
    public sealed class GetTextCensorJobResult : CosDataResult<TextCensorResult>
    {
        /// <summary>
        /// 文本审核结果
        /// </summary>
        /// <value></value>
        public TextCensorResult resultStruct {
            get {return _data; }
        }
    }
}
