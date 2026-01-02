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
    /// 查询视频审核任务
    /// <see href="https://cloud.tencent.com/document/product/436/47316"/>
    /// </summary>
    public sealed class GetVideoCensorJobRequest : CIRequest
    {
        public GetVideoCensorJobRequest(string bucket, string JobId)
            : base(bucket)
        {
            this.method = CosRequestMethod.GET;
            this.SetRequestPath("/video/auditing/" + JobId);
        }
    }
}
