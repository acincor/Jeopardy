using System;
using System.Collections.Generic;

using System.Text;
using Unity.UOS.COSXML.Common;
using Unity.UOS.COSXML.Utils;

namespace Unity.UOS.COSXML.Common
{
    public enum CosMetaDataDirective
    {
        [CosValue("Copy")]
        Copy = 0,

        [CosValue("Replaced")]
        Replaced
    }
}
