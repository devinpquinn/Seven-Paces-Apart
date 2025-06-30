using UnityEngine;
using UnityEditor;
using System.IO;

public class ChoiceNodeGenerator : EditorWindow
{
    [MenuItem("Tools/Generate 7-Level Choice Nodes")]
    public static void GenerateChoiceNodes()
    {
        string path = "Assets/ScriptableObjects/ChoiceNodes";
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);

        // Step 1: Create nodes
        ChoiceNodeSO[] nodes = new ChoiceNodeSO[128];
        for (int i = 0; i < 128; i++)
        {
            string binary = System.Convert.ToString(i, 2).PadLeft(7, '0');
            ChoiceNodeSO node = ScriptableObject.CreateInstance<ChoiceNodeSO>();
            node.name = $"Choice_{binary}";
            node.description = $"Placeholder description for {binary}";
            node.optionAText = "Choose A";
            node.optionBText = "Choose B";
            node.optionAScoreChange = 0;
            node.optionBScoreChange = 0;

            AssetDatabase.CreateAsset(node, $"{path}/Choice_{binary}.asset");
            nodes[i] = node;
        }

        // Step 2: Link nodes
        for (int i = 0; i < 128; i++)
        {
            string binary = System.Convert.ToString(i, 2).PadLeft(7, '0');
            ChoiceNodeSO node = nodes[i];

            if (binary.Length < 7)
            {
                string nextA = binary + "0";
                string nextB = binary + "1";
                node.nextNodeA = GetNode(nodes, nextA);
                node.nextNodeB = GetNode(nodes, nextB);
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
        AssetDatabase.Refresh();
        Debug.Log("Generated and linked 128 choice nodes.");
    }

    private static ChoiceNodeSO GetNode(ChoiceNodeSO[] nodes, string binary)
    {
        int index = System.Convert.ToInt32(binary, 2);
        return index >= 0 && index < nodes.Length ? nodes[index] : null;
    }
}
