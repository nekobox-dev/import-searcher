#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Nekobox.ImportSearcher
{
    [Serializable]
    public class Package
    {
        [SerializeField] private string name;
        [SerializeField] private string path;
        [SerializeField] private DateTime importTime;

        public string Name { get => name; set => name = value; }
        public string Path { get => path; set => path = value; }
        public DateTime ImportTime { get => importTime; set => importTime = value; }
    }

    [FilePath(Defines.SAVE_FOLDER_PATH + "/Data.dat", FilePathAttribute.Location.ProjectFolder)]
    public class Data : ScriptableSingleton<Data>
    {
        public static event Action<string> OnDataChanged;
        public static void NotifyDataChanged(string message)
        {
            OnDataChanged?.Invoke(message);
            instance.Save(true);
        }

        [SerializeField] private List<Package> packages;

        public static List<Package> Packages
        {
            get => instance.packages;
            set
            {
                instance.packages = value;
                instance.Save(true);
            }
        }

        public void OnEnable()
        {
            if (packages == null)
            {
                packages = new List<Package>();
                instance.Save(true);
            }
        }

        public void OnDisable()
        {
            instance.Save(true);
        }
    }
}

#endif // UNITY_EDITOR
