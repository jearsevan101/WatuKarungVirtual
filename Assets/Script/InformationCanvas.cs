using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InformationCanvas : MonoBehaviour
{
    [SerializeField] private Canvas mainCanvas;
    [SerializeField] private GameObject informationPanel;
    [SerializeField] private GameObject timerPanel;
    [SerializeField] private GameObject kerangPanel;
    [SerializeField] private TextMeshProUGUI instructionText;
    [SerializeField] private TextMeshProUGUI countDownText;
    [SerializeField] private TextMeshProUGUI itemObtained;

    private void Start()
    {
        HideUI();
    }

    public void UpdateInstruksi(string instruksi)
    {
        mainCanvas.gameObject.SetActive(true);
        informationPanel.SetActive(true);
        instructionText.text = instruksi;

        EventManager.OnCountdownInstructionTick += HandleCountDownInstructionTick;

        if (instruksi == "Kumpulkan Kerang")
        {
            EventManager.OnCurrentItemObtained += HandleCurrentItemObtained;
        }
    }
    public void HideUI()
    {
        mainCanvas.gameObject.SetActive(false);
        informationPanel.SetActive(false);
        timerPanel.SetActive(false);
        kerangPanel.SetActive(false);
    }

    private void HandleCountDownInstructionTick(float time)
    {
        timerPanel.SetActive(true);
        UpdateTimer(time);
        if (time <= 0) 
        {
            EventManager.OnCountdownInstructionTick -= HandleCountDownInstructionTick;
        }
    }
    private void HandleCurrentItemObtained(int count)
    {
        kerangPanel.SetActive(true);
        UpdateItemObtained(count);
    }
    private void UpdateTimer(float time)
    {
        countDownText.text = ((int)time).ToString();
    }

    private void UpdateItemObtained(int count)
    {
        itemObtained.text = count.ToString()+"/10";
    }
}
