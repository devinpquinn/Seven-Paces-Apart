using UnityEngine;

[CreateAssetMenu(fileName = "ChoiceNode", menuName = "Narrative/ChoiceNode")]
public class ChoiceNodeSO : ScriptableObject
{
    [TextArea] public string description;
    public string optionAText;
    public int optionAScoreChange;
    public ChoiceNodeSO nextNodeA;

    public string optionBText;
    public int optionBScoreChange;
    public ChoiceNodeSO nextNodeB;

    public bool isFinalNode;
}
