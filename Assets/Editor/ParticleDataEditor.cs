using ND.Data;
using UnityEditor;
using UnityEngine;
using Newtonsoft.Json.Linq;
using System.IO;

[CustomEditor(typeof(ParticleDataSO))]
public class ParticleDataEditor : Editor
{
    private ParticleDataSO targetSO;

    private void OnEnable()
    {
        targetSO = (ParticleDataSO)target;
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

        targetSO.particleDataList.Clear();

        JArray jArray = JArray.Parse(data);

        foreach (var json in jArray)
        {
            ParticleData particleData = json.ToObject<ParticleData>();
            targetSO.particleDataList.Add(particleData);
        }
    }

    private void Save()
    {
        if (targetSO.particleDataList == null || targetSO.particleDataList.Count == 0)
            return;

        string path = AssetDatabase.GetAssetPath(targetSO);

        var jArray = new JArray();

        foreach (var d in targetSO.particleDataList)
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