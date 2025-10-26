using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KerangMinigameManager : MonoBehaviour
{
    [SerializeField] private ConversationManager conversationCanvas;
    [SerializeField] private GameObject triggerSandMinigame;
    [SerializeField] private GameObject locomotionRoot;
    [SerializeField] private GameObject npcObject;
    [SerializeField] private float triggerRadius = 2f;
    [SerializeField] private ConversationSO conversationDialogueSO;
    [SerializeField] private Transform playerTransform; // XR Origin Camera or root
    [SerializeField] private CountDownUI countDownUI;
    [SerializeField] private float countDownMinigame;
    [SerializeField] private InstructionManager instructionManager;
    [SerializeField] private GameObject parentClampItem;
    [SerializeField] private int requiredClampObtained;

    [SerializeField] private InformationCanvas informationCanvas;

    [SerializeField] private Transform NPCPositionModeNormal;
    [SerializeField] private Transform NPCPositionModeDisability;
    [SerializeField] private Transform KerangPositionModeNormal;
    [SerializeField] private Transform KerangPositionModeDisability;
    [SerializeField] private GameObject floatingObject;
    private bool hasTriggered = false;
    private bool isCurrentMinigameActive = false;
    private int currentClampScore = 0;

    private List<ClampItem> clampItems = new List<ClampItem>();

    private void Awake()
    {
        if (parentClampItem != null)
        {
            // Get all ClampItem scripts in children
            clampItems.AddRange(parentClampItem.GetComponentsInChildren<ClampItem>(true));
        }
        EventManager.OnModeNormal += HandleModeGame;
        EventManager.OnActiveMinigame += HandleActiveMinigameChange;
    }
    private void HandleActiveMinigameChange(currentActiveMinigame activeMinigame)
    {
        isCurrentMinigameActive = (activeMinigame == currentActiveMinigame.clamp) ? true : false;
        floatingObject.SetActive(isCurrentMinigameActive);
    }
    private void HandleModeGame(bool isGameNormal)
    {
        if (isGameNormal)
        {
            transform.position = NPCPositionModeNormal.position;
            transform.rotation = NPCPositionModeNormal.rotation;
            parentClampItem.transform.position = KerangPositionModeNormal.position;
            parentClampItem.transform.rotation = KerangPositionModeNormal.rotation;
        }
        else
        {
            transform.position = NPCPositionModeDisability.position;
            transform.rotation = NPCPositionModeDisability.rotation;
            parentClampItem.transform.position = KerangPositionModeDisability.position;
            parentClampItem.transform.rotation = KerangPositionModeDisability.rotation;
        }
    }

    private void Update()
    {
        if (!isCurrentMinigameActive)
        {
            return;
        }
        if (!hasTriggered && Vector3.Distance(playerTransform.position, triggerSandMinigame.transform.position) <= triggerRadius)
        {
            hasTriggered = true;
            StartConversation();
        }

    }

    private void StartConversation()
    {

        if (conversationDialogueSO == null)
        {
            Debug.LogWarning("No Conversation Dialogue SO assigned!");
            return;
        }

        if (conversationCanvas != null)
        {
            conversationCanvas.gameObject.SetActive(true);
            StartCoroutine(SetCanvasFacingPlayerNextFrame());
            FaceNPCToPlayer();
            conversationCanvas.StartConversation(conversationDialogueSO);
            EventManager.OnConversationEnded += HandleConversationEnded;
        }
    }
    private IEnumerator SetCanvasFacingPlayerNextFrame()
    {
        yield return null; // wait 1 frame
        FaceCanvasToPlayer();
    }
    private void HandleConversationEnded()
    {
        // Unsubscribe so it only triggers once
        EventManager.OnConversationEnded -= HandleConversationEnded;

        informationCanvas.UpdateInstruksi("Kumpulkan Kerang");
        conversationCanvas.gameObject.SetActive(false);

        parentClampItem.SetActive(true);
        foreach (ClampItem clamp in clampItems)
        {
            clamp.gameObject.SetActive(true);
            clamp.SpawnClamp();
        }

        countDownUI.gameObject.SetActive(true);
        StartCoroutine(SetCountDownFacingPlayerNextFrame());
        countDownUI.StartCountDown(countDownMinigame);
        EventManager.OnCountdownEnded += HandleCountDownEnded;
        EventManager.OnClampObtained += HandleClampObtained;
    }
    private void HandleClampObtained()
    {
        currentClampScore++;
        EventManager.CurrentItemObtained(currentClampScore);
        Debug.Log($"Clamp obtained! Current score: {currentClampScore}");

        if (currentClampScore >= requiredClampObtained)
        {
            FinishGame();
        }
    }
    private void FinishGame()
    {
        Debug.Log("All required clamps obtained! Minigame finished.");
        EventManager.OnClampObtained -= HandleClampObtained;

        // Disable clamp items
        parentClampItem.SetActive(false);

        EventManager.StopCountDown();
        countDownUI.gameObject.SetActive(false);
        // You can show instruction, UI, or trigger next quest here
        instructionManager.StartInstruction("Semua Kerang Didapatkan!", "Oke");
        currentClampScore = 0;
        hasTriggered = false;
        informationCanvas.HideUI();

        instructionManager.StartInstruction("Permainan kerangnya sudah selesai, silahkan lanjutkan perjalanan", "Oke");
        EventManager.MinigameEnded(currentActiveMinigame.clamp);
    }
    private IEnumerator SetCountDownFacingPlayerNextFrame()
    {
        yield return null; // wait 1 frame
        FaceCountDownUIToPlayer();
    }
    private void FaceCountDownUIToPlayer()
    {
        if (countDownUI == null) return;

        Vector3 direction = playerTransform.position - countDownUI.transform.position;
        direction.y = 0f; // keep upright
        if (direction.sqrMagnitude > 0.001f)
        {
            // Look at player
            countDownUI.transform.rotation = Quaternion.LookRotation(direction);

            // Optional: rotate 180° if the forward is inverted
            countDownUI.transform.Rotate(0f, 180f, 0f);
        }
    }
    private void HandleCountDownEnded()
    {
        currentClampScore = 0;
        countDownUI.gameObject.SetActive(false);
        hasTriggered = false;
        EventManager.OnCountdownEnded -= HandleCountDownEnded;
        instructionManager.StartInstruction("Waktu Habis", "Oke");
        informationCanvas.HideUI();
        EventManager.OnInstructionResponsesClicked += HandleInstrucionResponse;
    }

    private void HandleInstrucionResponse(bool response)
    {
        EventManager.OnInstructionResponsesClicked -= HandleInstrucionResponse;
    }

    private void FaceCanvasToPlayer()
    {
        Vector3 direction = playerTransform.position - conversationCanvas.transform.position;
        direction.y = 0f; // keep upright
        if (direction.sqrMagnitude > 0.001f)
        {
            // Look at player
            conversationCanvas.transform.rotation = Quaternion.LookRotation(direction);

            // Rotate 180° around Y to fix inverted forward
            conversationCanvas.transform.Rotate(0f, 180f, 0f);
        }
    }

    private void FaceNPCToPlayer()
    {
        if (npcObject == null) return;

        Vector3 direction = playerTransform.position - npcObject.transform.position;
        direction.y = 0f; // keep NPC upright
        if (direction.sqrMagnitude > 0.001f)
        {
            npcObject.transform.rotation = Quaternion.LookRotation(direction);
        }
    }
}
