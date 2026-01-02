using System.Linq;
using UnityEditor.PackageManager.UI;
using UnityEngine;

namespace Unity.UOS.Launcher
{
    public class PackageSampleManager
    {
        public static bool Import(ServiceConfig config)
        {
            if (config.samples == null || !config.samples.Any()) return false;
            var sample = config.samples[0];
            return sample.Import();
        }

        public static void GetSamples(ServiceConfig config)
        {
            var samples = Sample.FindByPackage(config.name, config.installedVersion).ToList();
            config.samples = samples;
            if (samples.Any())
            {
                config.sampleIsImported = samples[0].isImported;
            }
        }
    }
}