#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Nekobox.ImportSearcher
{
    public class Import
    {
        [SerializeField] private static Package buffer = null;

        [InitializeOnLoadMethod]
        static void RegisterCallback()
        {
            AssetDatabase.importPackageCompleted += (packageName) => 
            {
                buffer = new Package
                {
                    Name = packageName,
                    Path = null,
                    ImportTime = System.DateTime.Now,
                };
            };
            AssetDatabase.onImportPackageItemsCompleted += (paths) => 
            {
                if (buffer == null) return;

                var samplingRate = 10;
                var sample = UniformSample(paths, samplingRate);
                var path = sample[0];
                for (int i = 1; i < samplingRate; i++)
                {
                    path = GetCommonDirectory(path, sample[i]);
                }
                buffer.Path = path;
                
                Data.Packages.Insert(0, buffer);
                Data.NotifyDataChanged(Defines.LOG_PREFIX + "Add Package: " + buffer.Name);
                buffer = null;
            };
        }

        public static string GetCommonDirectory(string path1, string path2)
        {
            if (string.IsNullOrEmpty(path1) || string.IsNullOrEmpty(path2)) return string.Empty;

            var parts1 = path1.Replace(@"\", "/").Split("/");
            var parts2 = path2.Replace(@"\", "/").Split("/");

            var commonParts = parts1.Zip(parts2, (a, b) => a == b ? a : null)
                .TakeWhile(part => part != null)
                .ToArray();

            return string.Join("/", commonParts);
        }

        public static string[] UniformSample(string[] paths, int samplingRate)
        {
            var result = new string[samplingRate];
            for (int i = 0; i < samplingRate; i++)
            {
                int index = Mathf.RoundToInt((float)i / (samplingRate - 1) * (paths.Length - 1));
                result[i] = paths[Mathf.Clamp(index, 0, paths.Length - 1)];
            }
            return result;
        }
    }
}

#endif // UNITY_EDITOR