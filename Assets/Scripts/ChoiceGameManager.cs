using UnityEngine;
using TMPro;  // Assuming you use TextMeshPro for UI text
using UnityEngine.UI;

public class ChoiceGameManager : MonoBehaviour
{
    public ChoiceNodeSO currentNode;

    public TextMeshProUGUI descriptionText;
    public Button optionAButton;
    public Button optionBButton;
    public TextMeshProUGUI optionAText;
    public TextMeshProUGUI optionBText;

    private int score = 0;

    void Start()
    {
        if (currentNode == null)
        {
            Debug.LogError("Current node is not set.");
            return;
        }

        SetupUI();
        optionAButton.onClick.AddListener(OnOptionAClicked);
        optionBButton.onClick.AddListener(OnOptionBClicked);
    }

    void SetupUI()
    {
        descriptionText.text = currentNode.description;
        optionAText.text = currentNode.optionAText;
        optionBText.text = currentNode.optionBText;
    }

    public void OnOptionAClicked()
    {
        score += currentNode.optionAScoreChange;
        if (currentNode.nextNodeA != null)
        {
            currentNode = currentNode.nextNodeA;
            SetupUI();
            CheckForEnding();
        }
    }

    public void OnOptionBClicked()
    {
        score += currentNode.optionBScoreChange;
        if (currentNode.nextNodeB != null)
        {
            currentNode = currentNode.nextNodeB;
            SetupUI();
            CheckForEnding();
        }
    }

    void CheckForEnding()
    {
        if (currentNode.isFinalNode)
        {
            // Handle ending — for example, display score and disable buttons
            descriptionText.text = $"The End!\nYour score: {score}";
            optionAButton.gameObject.SetActive(false);
            optionBButton.gameObject.SetActive(false);
        }
    }
}
