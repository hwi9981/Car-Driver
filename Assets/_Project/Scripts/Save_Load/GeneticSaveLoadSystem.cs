using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class GeneticSaveLoadSystem
{
    private static string folderPath => Path.Combine(Application.dataPath, "Resources/GeneticSaves");

    public static void SaveGenerationData(string fileName, GenerationData data)
    {
        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);

        string json = JsonUtility.ToJson(data, true);
        string path = Path.Combine(folderPath, fileName);
        File.WriteAllText(path, json);
        Debug.Log($"Saved generation data to: {path}");
    }

    public static GenerationData LoadGenerationData(string fileName)
    {
        TextAsset jsonFile = Resources.Load<TextAsset>($"GeneticSaves/{fileName}");
        if (jsonFile == null)
        {
            Debug.LogError($"Cannot find file: {fileName}");
            return null;
        }

        return JsonUtility.FromJson<GenerationData>(jsonFile.text);
    }

    public static List<string> GetSavedFiles()
    {
        List<string> fileNames = new List<string>();

        if (!Directory.Exists(folderPath)) return fileNames;

        var files = Directory.GetFiles(folderPath, "*.json");
        foreach (var file in files)
        {
            string name = Path.GetFileNameWithoutExtension(file);
            fileNames.Add(name);
        }
        return fileNames;
    }
}