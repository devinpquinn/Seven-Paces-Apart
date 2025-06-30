
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

public class ChoiceNodeLinker : EditorWindow
{
    [MenuItem("Tools/Link Choice Nodes")]
    public static void LinkNodes()
    {
        string basePath = "Assets/ScriptableObjects/ChoiceNodes";
        string[] guids = AssetDatabase.FindAssets("t:ChoiceNodeSO", new[] { basePath });
        Dictionary<string, ChoiceNodeSO> nodeMap = new Dictionary<string, ChoiceNodeSO>();

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            ChoiceNodeSO node = AssetDatabase.LoadAssetAtPath<ChoiceNodeSO>(path);
            if (node != null && node.name.StartsWith("Choice_"))
            {
                nodeMap[node.name.Replace("Choice_", "")] = node;
            }
        }

        foreach (var kvp in nodeMap)
        {
            string key = kvp.Key;
            ChoiceNodeSO node = kvp.Value;

            if (key.Length < 7)
            {
                string aKey = key + "0";
                string bKey = key + "1";
                node.nextNodeA = nodeMap.ContainsKey(aKey) ? nodeMap[aKey] : null;
                node.nextNodeB = nodeMap.ContainsKey(bKey) ? nodeMap[bKey] : null;
                node.isFinalNode = false;
            }
            else
            {
                node.nextNodeA = null;
                node.nextNodeB = null;
                node.isFinalNode = true;
            }

            EditorUtility.SetDirty(node);
        }

        AssetDatabase.SaveAssets();
        Debug.Log("Choice nodes linked successfully.");
    }
}
