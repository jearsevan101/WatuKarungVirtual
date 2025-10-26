using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SandMinigameManager : MonoBehaviour
{
    [SerializeField] private ConversationManager conversationCanvas;
    [SerializeField] private GameObject triggerSandMinigame;
    [SerializeField] private GameObject locomotionRoot;
    [SerializeField] private GameObject npcObject;
    [SerializeField] private float triggerRadius = 2f;
    [SerializeField] private ConversationSO conversationDialogueSO;
    [SerializeField] private Transform playerTransform; // XR Origin Camera or root
    [SerializeField] private Transform NPCPositionModeNormal;
    [SerializeField] private Transform NPCPositionModeDisability;
    [SerializeField] private GameObject floatingObject;

    [SerializeField] private InstructionManager instructionManager;

    private bool hasTriggered = false;
    private bool isCurrentMinigameActive = false;

    private void Start()
    {
        EventManager.OnModeNormal += HandleModeGame;
        EventManager.OnActiveMinigame += HandleActiveMinigameChange;
    }
    private void HandleActiveMinigameChange(currentActiveMinigame activeMinigame)
    {
        isCurrentMinigameActive = (activeMinigame == currentActiveMinigame.sand) ? true : false;
        floatingObject.SetActive(isCurrentMinigameActive);
    }
    private void HandleModeGame(bool isGameNormal)
    {
        if (isGameNormal)
        {
            transform.position = NPCPositionModeNormal.position;
            transform.rotation = NPCPositionModeNormal.rotation;
        }
        else
        {
            transform.position = NPCPositionModeDisability.position;
            transform.rotation = NPCPositionModeDisability.rotation;
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
            LockPlayerMovement(true);
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
        LockPlayerMovement(false);
        conversationCanvas.gameObject.SetActive(false);
        StartCoroutine(ResetTrigger());

        instructionManager.StartInstruction("Permainan pasirnya sudah selesai, silahkan lanjutkan perjalanan", "Oke");
        EventManager.MinigameEnded(currentActiveMinigame.sand);
    }

    private IEnumerator ResetTrigger()
    {
        // Wait for 5 seconds
        yield return new WaitForSeconds(5f);

        // Reset the trigger
        hasTriggered = false;
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
    private void LockPlayerMovement(bool isLocked)
    {
        if (locomotionRoot != null)
            locomotionRoot.SetActive(!isLocked);
    }
}
