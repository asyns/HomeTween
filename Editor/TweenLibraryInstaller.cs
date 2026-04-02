using UnityEngine;
using UnityEditor;
using System.IO;

namespace HomeTween
{
    [InitializeOnLoad]
    public class TweenLibraryInstaller
    {
        static TweenLibraryInstaller()
        {
            // We run this on the next editor update to ensure the database is ready
            EditorApplication.delayCall += CheckAndCreateConfig;
        }

        private static void CheckAndCreateConfig()
        {
            string folderPath = "Assets/Resources";
            string assetPath = folderPath + "/TweenData.asset";

            // 1. Check if the file already exists so we don't overwrite user changes
            if (File.Exists(assetPath)) return;

            // 2. Ensure the Resources folder exists
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
                AssetDatabase.ImportAsset(folderPath);
            }

            // 3. Create the instance of your ScriptableObject
            var config = ScriptableObject.CreateInstance<TweenData>();

            // 4. Save it to the Assets folder
            AssetDatabase.CreateAsset(config, assetPath);
            AssetDatabase.SaveAssets();

            Debug.Log($"<b>TweenLib:</b> Created default configuration at {assetPath}");
        }
    }
}