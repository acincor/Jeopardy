using System.Collections.Generic;

namespace Unity.UOS.Launcher
{
    public class WeixinMinigameDefineSymbol : MinigameDefineSymbol
    {
        protected override List<string> DefineSymbols => new List<string>() { "UNITY_WEIXINMINIGAME", "UOS_WEIXINMINIGAME" };
        protected override string PlatformName => "Weixin";
    }
}
