using UnityEngine;
using UnityEditor;
using System.IO;

public class ChoiceNodeGeneratorRecursive : EditorWindow
{
    private const int MaxDepth = 7;
    private const string BaseFolder = "Assets/ScriptableObjects/ChoiceNodes";

    [MenuItem("Tools/Generate Recursive Choice Nodes")]
    public static void GenerateRecursiveNodes()
    {
        if (!AssetDatabase.IsValidFolder(BaseFolder))
            AssetDatabase.CreateFolder("Assets/ScriptableObjects", "ChoiceNodes");

        // Create EndingNode folder and asset
        string endingFolder = Path.Combine(BaseFolder, "EndingNode");
        if (!AssetDatabase.IsValidFolder(endingFolder))
            AssetDatabase.CreateFolder(BaseFolder, "EndingNode");

        string endingAssetPath = Path.Combine(endingFolder, "EndingNode.asset");
        ChoiceNodeSO endingNode = AssetDatabase.LoadAssetAtPath<ChoiceNodeSO>(endingAssetPath);
        if (endingNode == null)
        {
            endingNode = ScriptableObject.CreateInstance<ChoiceNodeSO>();
            endingNode.name = "EndingNode";
            endingNode.description = "This is the ending node.";
            endingNode.optionAText = "";
            endingNode.optionBText = "";
            endingNode.optionAScoreChange = 0;
            endingNode.optionBScoreChange = 0;
            endingNode.isFinalNode = true;
            AssetDatabase.CreateAsset(endingNode, endingAssetPath);
            Debug.Log("Created EndingNode asset.");
        }

        // Generate the StartNode recursively
        GenerateNodeRecursive("", BaseFolder, endingNode);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("✅ Recursive choice nodes generated and linked.");
    }

    // Recursive node generator
    // path: current binary path ("" for StartNode)
    // folder: folder path to create node asset and subfolders inside
    // endingNode: reference to the ending node asset
    private static void GenerateNodeRecursive(string path, string folder, ChoiceNodeSO endingNode)
    {
        // Create folder for this node if it doesn't exist
        if (!AssetDatabase.IsValidFolder(folder))
        {
            string parentFolder = Path.GetDirectoryName(folder);
            string newFolderName = Path.GetFileName(folder);
            AssetDatabase.CreateFolder(parentFolder, newFolderName);
        }

        // Determine node name and asset path
        string nodeName = path == "" ? "StartNode" : $"Choice_{path}";
        string assetPath = Path.Combine(folder, nodeName + ".asset");

        // Load or create the node asset
        ChoiceNodeSO node = AssetDatabase.LoadAssetAtPath<ChoiceNodeSO>(assetPath);
        if (node == null)
        {
            node = ScriptableObject.CreateInstance<ChoiceNodeSO>();
            node.name = nodeName;
            node.description = path == "" ? "Start of the narrative." : $"Choice node at path {path}";
            node.optionAText = "Option A";
            node.optionBText = "Option B";
            node.optionAScoreChange = 0;
            node.optionBScoreChange = 0;
            AssetDatabase.CreateAsset(node, assetPath);
            Debug.Log($"Created node: {nodeName} at {assetPath}");
        }

        // If max depth reached, link both options to endingNode and mark as final
        if (path.Length == MaxDepth)
        {
            node.nextNodeA = endingNode;
            node.nextNodeB = endingNode;
            node.isFinalNode = true;
            EditorUtility.SetDirty(node);
            return;
        }

        // Create subfolders for options
        string optionAFolder = Path.Combine(folder, "OptionA");
        string optionBFolder = Path.Combine(folder, "OptionB");

        if (!AssetDatabase.IsValidFolder(optionAFolder))
            AssetDatabase.CreateFolder(folder, "OptionA");
        if (!AssetDatabase.IsValidFolder(optionBFolder))
            AssetDatabase.CreateFolder(folder, "OptionB");

        // Recursively generate child nodes
        GenerateNodeRecursive(path + "0", optionAFolder, endingNode);
        GenerateNodeRecursive(path + "1", optionBFolder, endingNode);

        // Load child nodes to assign references
        string nextAName = path + "0";
        string nextBName = path + "1";
        string nextAAssetPath = Path.Combine(optionAFolder, $"Choice_{nextAName}.asset");
        string nextBAssetPath = Path.Combine(optionBFolder, $"Choice_{nextBName}.asset");

        ChoiceNodeSO nextNodeA = AssetDatabase.LoadAssetAtPath<ChoiceNodeSO>(nextAAssetPath);
        ChoiceNodeSO nextNodeB = AssetDatabase.LoadAssetAtPath<ChoiceNodeSO>(nextBAssetPath);

        node.nextNodeA = nextNodeA != null ? nextNodeA : endingNode;
        node.nextNodeB = nextNodeB != null ? nextNodeB : endingNode;
        node.isFinalNode = false;

        EditorUtility.SetDirty(node);
    }
}
