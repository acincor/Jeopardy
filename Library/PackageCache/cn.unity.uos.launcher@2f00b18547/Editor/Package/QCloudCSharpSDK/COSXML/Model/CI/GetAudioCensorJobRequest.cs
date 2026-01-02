using System;
using System.Collections.Generic;

using System.Text;
using Unity.UOS.COSXML.Common;
using Unity.UOS.COSXML.Model.Object;
using Unity.UOS.COSXML.Model.Tag;
using Unity.UOS.COSXML.CosException;
using Unity.UOS.COSXML.Utils;

namespace Unity.UOS.COSXML.Model.CI
{
    /// <summary>
    /// 查询音频审核任务
    /// <see href="https://cloud.tencent.com/document/product/436/54064"/>
    /// </summary>
    public sealed class GetAudioCensorJobRequest : CIRequest
    {
        public GetAudioCensorJobRequest(string bucket, string JobId)
            : base(bucket)
        {
            this.method = CosRequestMethod.GET;
            this.SetRequestPath("/audio/auditing/" + JobId);
        }
    }
}
