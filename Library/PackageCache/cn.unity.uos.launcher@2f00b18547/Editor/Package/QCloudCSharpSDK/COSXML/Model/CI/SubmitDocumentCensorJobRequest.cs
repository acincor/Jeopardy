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
    /// 提交文档审核任务
    /// <see href="https://cloud.tencent.com/document/product/436/59381"/>
    /// </summary>
    public sealed class SubmitDocumentCensorJobRequest : CIRequest
    {
        public DocumentCensorJobInfo documentCensorJobInfo;
        public SubmitDocumentCensorJobRequest(string bucket)
            : base(bucket)
        {
            documentCensorJobInfo = new DocumentCensorJobInfo();
            this.method = CosRequestMethod.POST;
            this.SetRequestPath("/document/auditing");
            this.SetRequestHeader("Content-Type", "application/xml");
            documentCensorJobInfo.input = new DocumentCensorJobInfo.Input();
            documentCensorJobInfo.conf = new DocumentCensorJobInfo.Conf();
        }

        public void SetUrl(string Url)
        {
            documentCensorJobInfo.input.url = Url;
        }

        public void SetType(string type)
        {
            documentCensorJobInfo.input.type = type;
        }
        
        public void SetDetectType(string detectType)
        {
            documentCensorJobInfo.conf.detectType = detectType;
        }

        public void SetCallback(string callbackUrl)
        {
            documentCensorJobInfo.conf.callback = callbackUrl;
        }

        public void SetBizType(string BizType)
        {
            documentCensorJobInfo.conf.bizType = BizType;
        }

        public override Network.RequestBody GetRequestBody()
        {
            return GetXmlRequestBody(documentCensorJobInfo);
        }

    }
}
