using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ChoiceGameManager : MonoBehaviour
{
    public ChoiceNodeSO currentNode;

    public TextMeshProUGUI descriptionText;
    public Button optionButton1;
    public Button optionButton2;
    public TextMeshProUGUI optionText1;
    public TextMeshProUGUI optionText2;

    private int score = 0;

    // Track which button corresponds to choice A or B this round
    private bool isOption1ChoiceA;

    void Start()
    {
        if (currentNode == null)
        {
            Debug.LogError("Current node is not set.");
            return;
        }

        SetupUI();

        optionButton1.onClick.AddListener(OnOption1Clicked);
        optionButton2.onClick.AddListener(OnOption2Clicked);
    }

    void SetupUI()
    {
        descriptionText.text = currentNode.description;

        // Randomize option order
        if (Random.value < 0.5f)
        {
            // Option1 = Choice A, Option2 = Choice B
            optionText1.text = currentNode.optionAText;
            optionText2.text = currentNode.optionBText;
            isOption1ChoiceA = true;
        }
        else
        {
            // Option1 = Choice B, Option2 = Choice A
            optionText1.text = currentNode.optionBText;
            optionText2.text = currentNode.optionAText;
            isOption1ChoiceA = false;
        }

        // Enable buttons and texts (in case previously disabled)
        optionButton1.gameObject.SetActive(true);
        optionButton2.gameObject.SetActive(true);
    }

    public void OnOption1Clicked()
    {
        if (isOption1ChoiceA)
            MakeChoice(isChoiceA: true);
        else
            MakeChoice(isChoiceA: false);
    }

    public void OnOption2Clicked()
    {
        if (isOption1ChoiceA)
            MakeChoice(isChoiceA: false);
        else
            MakeChoice(isChoiceA: true);
    }

    void MakeChoice(bool isChoiceA)
    {
        // Award points
        score += isChoiceA ? 100 : 0;

        // Move to next node
        if (isChoiceA)
            currentNode = currentNode.nextNodeA;
        else
            currentNode = currentNode.nextNodeB;

        // Update UI or handle end
        if (currentNode == null)
        {
            Debug.LogError("Next node is null!");
            return;
        }

        if (currentNode.isFinalNode)
        {
            descriptionText.text = $"The End!\nYour final score: {score}";
            optionButton1.gameObject.SetActive(false);
            optionButton2.gameObject.SetActive(false);
        }
        else
        {
            SetupUI();
        }
    }
}
