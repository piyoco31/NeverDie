using System.IO;
using ND.Data;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MonsterDataSO))]
public class MonsterDataEditor : Editor
{
    private MonsterDataSO targetSO;

    private void OnEnable()
    {
        targetSO = (MonsterDataSO)target;
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

        targetSO.monsterDataList.Clear();

        JArray jArray = JArray.Parse(data);

        foreach (var json in jArray)
        {
            MonsterData monsterData = json.ToObject<MonsterData>();
            targetSO.monsterDataList.Add(monsterData);
        }
    }

    private void Save()
    {
        if (targetSO.monsterDataList == null || targetSO.monsterDataList.Count == 0)
            return;

        string path = AssetDatabase.GetAssetPath(targetSO);

        var jArray = new JArray();

        foreach (var d in targetSO.monsterDataList)
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