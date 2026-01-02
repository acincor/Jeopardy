using UnityEditor;

namespace UOSLauncher.Editor.Environment
{
    public class AssetUpdateWatcher : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(
            string[] importedAssets,
            string[] deletedAssets,
            string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            foreach (string assetPath in importedAssets)
            {
                if (assetPath == EnvironmentManager.DEFAULT_ASSETS_PATH)
                {
                    EnvironmentManager.SetEnvironmentInfo();
                }
            }
        }
    }
}