using System.Collections.Generic;
using UnityEditor;

namespace Unity.UOS.Launcher
{
    public class DouyinMinigameDefineSymbol : MinigameDefineSymbol
    {
        protected override List<string> DefineSymbols => new List<string>() { "UOS_DOUYINMINIGAME" };
        protected override string PlatformName => "Douyin";
    }
}
