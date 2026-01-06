using ND.Data;
using UnityEditor;
using UnityEngine;
using Newtonsoft.Json.Linq;
using System.IO;

[CustomEditor(typeof(HeroDataSO))]
public class HeroDataEditor : Editor
{
    private HeroDataSO targetSO;

    private void OnEnable()
    {
        targetSO = (HeroDataSO)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (targetSO != null)
        {
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("TextDataLoad") == true)
                Load(targetSO.textAsset.text);

            if (GUILayout.Button("TextDataSave") == true)
                Save();

            EditorGUILayout.EndHorizontal();
        }
        else
            Debug.Log("SO is Null");

    }

    private void Load(string data)
    {
        if (string.IsNullOrEmpty(data))
            return;

        targetSO.heroDataList.Clear();

        JArray jArray = JArray.Parse(data);

        foreach (var json in jArray)
        {
            HeroData heroData = json.ToObject<HeroData>();
            targetSO.heroDataList.Add(heroData);
        }
    }

    private void Save()
    {
        if (targetSO.heroDataList == null || targetSO.heroDataList.Count == 0)
            return;

        string path = AssetDatabase.GetAssetPath(targetSO);

        var jArray = new JArray();

        foreach (var d in targetSO.heroDataList)
        {
            JObject jObject = JObject.FromObject(d);
            jArray.Add(jObject);
        }

        string directory = Path.GetDirectoryName(path);
        string assetName = Path.GetFileNameWithoutExtension(path);
        string jsonFilePath = Path.Combine(directory, $"{assetName}Data.json");

        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        File.WriteAllText(jsonFilePath, jArray.ToString());
        AssetDatabase.Refresh();

        Debug.Log($"Saved JSON file at: {jsonFilePath}");
    }
}
