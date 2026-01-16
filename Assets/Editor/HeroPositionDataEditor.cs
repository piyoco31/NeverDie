using ND.Data;
using UnityEditor;
using UnityEngine;
using Newtonsoft.Json.Linq;
using System.IO;

[CustomEditor(typeof(HeroPositionDataSO))]
public class HeroPositionDataEditor : Editor
{
    private HeroPositionDataSO targetSO;

    private void OnEnable()
    {
        targetSO = (HeroPositionDataSO)target;
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

        targetSO.heroPositionDataList.Clear();

        JArray jArray = JArray.Parse(data);

        foreach (var json in jArray)
        {
            HeroPositionData heroPositionData = json.ToObject<HeroPositionData>();
            targetSO.heroPositionDataList.Add(heroPositionData);
        }
    }

    private void Save()
    {
        if (targetSO.heroPositionDataList == null || targetSO.heroPositionDataList.Count == 0)
            return;

        string path = AssetDatabase.GetAssetPath(targetSO);

        var jArray = new JArray();

        foreach (var d in targetSO.heroPositionDataList)
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