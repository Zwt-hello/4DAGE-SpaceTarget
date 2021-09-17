using System;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class ARFoundationPackageConfig
{
    const string ARFOUNDATION_VERSION = "3.1.6";
    const string ARFOUNDATIONPACKAGE_KEY = "\"com.unity.xr.arfoundation\"";

    const string ARKIT_VERSION = "3.1.8";
    const string ARKIT_PACKAGE_KEY = "\"com.unity.xr.arkit\"";

    const string ARCORE_VERSION = "3.1.8";
    const string ARCORE_PACKAGE_KEY = "\"com.unity.xr.arcore\"";

    static readonly string sPackagesPath = Path.Combine(Application.dataPath, "..", "Packages");
    static readonly string sManifestJsonPath = Path.Combine(sPackagesPath, "manifest.json");

    private const string pkgPath = "Assets/4DAGE-SpaceTarget/Samples/ARFoundationSample.unitypackage";
    //private const string pkg_upm = "Packages/com.4dage.spacetarget/Samples~/ARFoundationSample.unitypackage";
    private const string samplePath = "Assets/4DAGE-SpaceTarget/Samples/Base on ARFoundation Example";

    static ARFoundationPackageConfig()
    {
        if (Application.isBatchMode)
            return;

        var manifest = Manifest.JsonDeserialize(sManifestJsonPath);
        if (!CheckARisExist(manifest))
        {
            //check dependence
            UpdateManifest(manifest);
            //if (EditorUtility.DisplayDialog("Add ARFoundation Package",
            //"SpaceTarget Sample base on ARFoundation(ARKit/ARCore), Please allow to import the packages\n" , "Import", "Cancel"))
            //{
            //    UpdateManifest(manifest);
            //}
        }
        else
        {
            //check sample
            if (!AssetDatabase.IsValidFolder(samplePath))
            {
                //check file
                string isPath_proj = Path.GetFullPath(pkgPath);
                FileInfo fi = new FileInfo(isPath_proj);
                if (fi.Exists)
                {
                    AssetDatabase.ImportPackage(pkgPath, false);
                }
            }
        }
    }
    static void UpdateManifest(Manifest manifest)
    {
        SetARVersion(manifest, ARFOUNDATIONPACKAGE_KEY, ARFOUNDATION_VERSION);
        SetARVersion(manifest, ARKIT_PACKAGE_KEY, ARKIT_VERSION);
        SetARVersion(manifest, ARCORE_PACKAGE_KEY, ARCORE_VERSION);
        manifest.JsonSerialize(sManifestJsonPath);

        AssetDatabase.Refresh();
    }

    static void SetARVersion(Manifest manifest,string packageKey,string version)
    {
        var dependencies = manifest.Dependencies.Split(',').ToList();
        var versionSet = false;
        for (var i = 0; i < dependencies.Count; i++)
        {
            if (!dependencies[i].Contains(packageKey))
                continue;

            var kvp = dependencies[i].Split(':');

            kvp[1] = $"\"{version}\""; //version string of the package

            dependencies[i] = string.Join(":", kvp);

            versionSet = true;
        }

        if (!versionSet)
            dependencies.Insert(0, $"\n    {packageKey}: \"{version}\"");

        manifest.Dependencies = string.Join(",", dependencies);
    }

    static bool CheckARisExist(Manifest manifest)
    {
        bool flag = false;
        int num = 0;
        var dependencies = manifest.Dependencies.Split(',').ToList();
        for(var i = 0; i < dependencies.Count; i++)
        {
            if (dependencies[i].Contains(ARFOUNDATIONPACKAGE_KEY)|| dependencies[i].Contains(ARCORE_PACKAGE_KEY)|| dependencies[i].Contains(ARKIT_PACKAGE_KEY))
            {
                num++;
            }
        }
        if (num < 3)
            flag = false;
        else
            flag = true;

        return flag;
    }

    class Manifest
    {
        const int INDEX_NOT_FOUND = -1;
        const string DEPENDENCIES_KEY = "\"dependencies\"";

        public ScopedRegistry[] ScopedRegistries;
        public string Dependencies;

        public void JsonSerialize(string path)
        {
            var jsonString = JsonUtility.ToJson(
                new UnitySerializableManifest { scopedRegistries = ScopedRegistries, dependencies = new DependencyPlaceholder() },
                true);

            var startIndex = GetDependenciesStart(jsonString);
            var endIndex = GetDependenciesEnd(jsonString, startIndex);

            var stringBuilder = new StringBuilder();

            stringBuilder.Append(jsonString.Substring(0, startIndex));
            stringBuilder.Append(Dependencies);
            stringBuilder.Append(jsonString.Substring(endIndex, jsonString.Length - endIndex));

            File.WriteAllText(path, stringBuilder.ToString());
        }

        public static Manifest JsonDeserialize(string path)
        {
            var jsonString = File.ReadAllText(path);

            var registries = JsonUtility.FromJson<UnitySerializableManifest>(jsonString).scopedRegistries ?? new ScopedRegistry[0];
            var dependencies = DeserializeDependencies(jsonString);

            return new Manifest { ScopedRegistries = registries, Dependencies = dependencies };
        }

        static string DeserializeDependencies(string json)
        {
            var startIndex = GetDependenciesStart(json);
            var endIndex = GetDependenciesEnd(json, startIndex);

            if (startIndex == INDEX_NOT_FOUND || endIndex == INDEX_NOT_FOUND)
                return null;

            var dependencies = json.Substring(startIndex, endIndex - startIndex);
            return dependencies;
        }

        static int GetDependenciesStart(string json)
        {
            var dependenciesIndex = json.IndexOf(DEPENDENCIES_KEY, StringComparison.InvariantCulture);
            if (dependenciesIndex == INDEX_NOT_FOUND)
                return INDEX_NOT_FOUND;

            var dependenciesStartIndex = json.IndexOf('{', dependenciesIndex + DEPENDENCIES_KEY.Length);

            if (dependenciesStartIndex == INDEX_NOT_FOUND)
                return INDEX_NOT_FOUND;

            dependenciesStartIndex++; //add length of '{' to starting point

            return dependenciesStartIndex;
        }

        static int GetDependenciesEnd(string jsonString, int dependenciesStartIndex)
        {
            return jsonString.IndexOf('}', dependenciesStartIndex);
        }
    }

    class UnitySerializableManifest
    {
        public ScopedRegistry[] scopedRegistries;
        public DependencyPlaceholder dependencies;
    }

    [Serializable]
    struct ScopedRegistry
    {
        public string name;
        public string url;
        public string[] scopes;

        public override bool Equals(object obj)
        {
            if (!(obj is ScopedRegistry))
                return false;

            var other = (ScopedRegistry)obj;

            return name == other.name &&
                   url == other.url &&
                   scopes.SequenceEqual(other.scopes);
        }

        public static bool operator ==(ScopedRegistry a, ScopedRegistry b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(ScopedRegistry a, ScopedRegistry b)
        {
            return !a.Equals(b);
        }

        public override int GetHashCode()
        {
            var hash = 17;

            foreach (var scope in scopes)
                hash = hash * 23 + (scope == null ? 0 : scope.GetHashCode());

            hash = hash * 23 + (name == null ? 0 : name.GetHashCode());
            hash = hash * 23 + (url == null ? 0 : url.GetHashCode());

            return hash;
        }
    }

    [Serializable]
    struct DependencyPlaceholder { }
}
