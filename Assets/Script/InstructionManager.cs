using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InstructionManager : MonoBehaviour
{
    [SerializeField] Canvas UICanvasInstruction;
    [SerializeField] TextMeshProUGUI instructionText;
    [SerializeField] TextMeshProUGUI yesButtonText;
    [SerializeField] Button yesButton;

    private void Start()
    {
        yesButton.onClick.AddListener(() =>
        {
            instructionText.text = "";
            UICanvasInstruction.gameObject.SetActive(false);
            EventManager.InstructionResponses(true);
        });
    }
    public void StartInstruction(string instruction, string buttonAText)
    {
        UICanvasInstruction.gameObject.SetActive(true);
        instructionText.text = instruction;
        yesButtonText.text = buttonAText;
    }
}
