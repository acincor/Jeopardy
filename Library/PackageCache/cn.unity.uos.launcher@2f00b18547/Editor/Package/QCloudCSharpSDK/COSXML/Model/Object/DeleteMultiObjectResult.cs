using System;
using System.Collections.Generic;

using System.Text;
using Unity.UOS.COSXML.Model.Tag;
using Unity.UOS.COSXML.Transfer;

namespace Unity.UOS.COSXML.Model.Object
{
    /// <summary>
    /// 批量删除 Object返回的结果
    /// <see href="https://cloud.tencent.com/document/product/436/8289"/>
    /// </summary>
    public sealed class DeleteMultiObjectResult : CosDataResult<DeleteResult>
    {
        /// <summary>
        /// 本次删除返回结果的方式和目标 Object
        /// <see href="Model.Tag.DeleteResult"/>
        /// </summary>
        public DeleteResult deleteResult { 
            get {return _data; } 
        }
    }
}
