using System.Collections.Generic;
using UnityEditor;

namespace Unity.UOS.Launcher
{
    public abstract class MinigameDefineSymbol
    {
        protected abstract List<string> DefineSymbols { get; }
        protected bool _isMinigame;
        protected abstract string PlatformName { get;  }
        
        public bool IsMinigame()
        {
#if !UNITY_WEBGL
            return false;
#endif
            string currentDefines = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.WebGL);
            HashSet<string> defines = new HashSet<string>(currentDefines.Split(';'));
            _isMinigame = defines.IsSupersetOf(DefineSymbols);
            return _isMinigame;
        }

        public void SetValue(bool enableMinigame)
        {
            if (enableMinigame == _isMinigame) return;
            _isMinigame = enableMinigame;
            if (enableMinigame)
            {
                foreach(var ds in DefineSymbols)
                {
                    Launcher.DefineSymbols.Add(BuildTargetGroup.WebGL, ds);
                }
            }
            else
            {
                foreach(var ds in DefineSymbols)
                {
                    Launcher.DefineSymbols.Remove(BuildTargetGroup.WebGL, ds);
                }
            }
        }

        public bool DisplayDialog()
        {
            var result =
                EditorUtility.DisplayDialog("Switch Target", $"Before using {PlatformName} Minigame, you need to switch the build target to WebGL firstly. ", "Switch to WebGL", "Cancel");
            if (result)
            {
                SwitchBuildTarget();
                SetValue(true);
            }

            return result;
        }

        private void SwitchBuildTarget()
        {
            EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.WebGL, BuildTarget.WebGL);
        }
    }
}