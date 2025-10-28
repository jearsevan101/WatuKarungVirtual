using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;
using static DirectionInfoManager;

public class CoconutMinigameManager : MonoBehaviour
{
    [SerializeField] private GameObject triggerConversation; // assign the trigger object
    [SerializeField] private GameObject triggerSitAndDrink; // assign the trigger object
    [SerializeField] private Transform sitPoint; // assign the trigger object
    [SerializeField] private ConversationManager conversationCanvas;
    [SerializeField] private ConversationSO conversationDialogueSO;
    [SerializeField] private Transform playerTransform; // XR Origin Camera or root
    [SerializeField] private float triggerRadius = 2f;
    [SerializeField] private CoconutDrinkManager coconutDrinkPrefab;
    [SerializeField] private GameObject attachParent;
    [SerializeField] private InstructionManager instructionManager;
    [SerializeField] private GameObject locomotionRoot;
    [SerializeField] private GameObject sitWheelChairPosition;
    [SerializeField] private int drinkAttempAmount;
    [SerializeField] private InformationCanvas informationCanvas;
    [SerializeField] private float restingTime=60f;
    [SerializeField] private DirectionInfoManager directionInfo;
    [SerializeField] private GameObject floatingObject;

    private bool hasTriggered = false;
    private bool hasCoconut = false;

    private int drinkAmount = 0;

    private bool isCurrentModeNormal = true;
    private bool isCurrentMinigameActive = false;

    private CoconutDrinkManager currentCoconut;
    private Coroutine countdownRoutine;
    // Store player original position/rotation
    private Vector3 originalPlayerPosition;
    private Quaternion originalPlayerRotation;
    private void Start()
    {
        EventManager.OnModeNormal += HandleModeChange;
        EventManager.OnActiveMinigame += HandleActiveMinigameChange;
    }
    private void Update()
    {
        if (!isCurrentMinigameActive)
        {
            return;
        }
        if (!hasTriggered && Vector3.Distance(playerTransform.position, triggerConversation.transform.position) <= triggerRadius)
        {
            hasTriggered = true;
            StartConversation();
        }
        if (hasCoconut && Vector3.Distance(playerTransform.position, triggerSitAndDrink.transform.position) <= triggerRadius*2)
        {
            hasCoconut = false;
            InstructionToSit();
        }
    }

    private void HandleActiveMinigameChange(currentActiveMinigame activeMinigame)
    {
        isCurrentMinigameActive = (activeMinigame == currentActiveMinigame.coconut)? true : false;
        floatingObject.SetActive(isCurrentMinigameActive);
    }
    private void HandleModeChange(bool isNormal)
    {
        isCurrentModeNormal = isNormal;
    }
    private void InstructionToSit()
    {
        instructionManager.StartInstruction("Apakah Kamu Mau Duduk dan Minum?", "Ya");
        EventManager.OnInstructionResponsesClicked += HandleInstrucionResponse;
    }
    private void HandleInstrucionResponse(bool response)
    {
        EventManager.OnInstructionResponsesClicked -= HandleInstrucionResponse;

        if (response)
        {
            StartSitAndDrink();
        }
    }
    private IEnumerator ResetTrigger()
    {
        // Wait for 5 seconds
        yield return new WaitForSeconds(5f);

        // Reset the trigger
        hasCoconut = true;

    }
    private void StartSitAndDrink()
    {
        if (!isCurrentModeNormal)
        {
            EventManager.DisabilitySit(true, sitWheelChairPosition);
        }

        // Save original position and rotation
        originalPlayerPosition = playerTransform.position;
        originalPlayerRotation = playerTransform.rotation;

        // Move player to sitPoint
        playerTransform.position = sitPoint.position;
        playerTransform.rotation = sitPoint.rotation;

        // Lock player movement
        LockPlayerMovement(true);
        DrinkCoconutCountDown();


    }

    private void DrinkCoconutCountDown()
    {
        // Start coconut countdown
        if (currentCoconut != null)
        {
            currentCoconut.StartCountDown();
            drinkAmount++;
        }

        // Listen for countdown finished
        EventManager.OnCountdownEnded += HandleCountDownEnded;
    }

    public void StartCountDown(float duration)
    {
        // If a countdown is already running, stop it
        if (countdownRoutine != null)
        {
            StopCoroutine(countdownRoutine);
        }

        // Start new countdown
        countdownRoutine = StartCoroutine(CountDownCoroutine(duration));
        EventManager.OnCountdownEnded += HandleStopCountDown;
    }

    private void HandleStopCountDown()
    {
        // Stop the coroutine if running
        if (countdownRoutine != null)
        {
            StopCoroutine(countdownRoutine);
            countdownRoutine = null;
        }
        informationCanvas.HideUI();
        DrinkCoconutCountDown();
        // Unsubscribe to prevent multiple calls
        EventManager.OnCountdownEnded -= HandleStopCountDown;
    }
    private IEnumerator CountDownCoroutine(float duration)
    {
        float timeLeft = duration;

        while (timeLeft > 0f)
        {

            EventManager.CountdownInstructionTick(timeLeft);

            // Wait for next frame
            yield return null;

            // Decrease timer
            timeLeft -= Time.deltaTime;
        }

        EventManager.CountdownEnded();

        countdownRoutine = null;
    }
    private void HandleCountDownEnded()
    {
        EventManager.OnCountdownEnded -= HandleCountDownEnded;
        if (drinkAmount < drinkAttempAmount)
        {
            informationCanvas.UpdateInstruksi("Istirahat");
            SpawnCoconutInThePlayer();
            StartCountDown(restingTime);
        }
        else
        {
            instructionManager.StartInstruction("Minumnya sudah habis, silahkan lanjutkan perjalanan", "Oke");
            EventManager.OnInstructionResponsesClicked += HandleInstrucionResponseEndDrink;
        }
    }
    private void HandleInstrucionResponseEndDrink(bool response)
    {
        EventManager.OnInstructionResponsesClicked -= HandleInstrucionResponseEndDrink;

        if (!isCurrentModeNormal)
        {
            playerTransform.position = originalPlayerPosition;
            playerTransform.rotation = originalPlayerRotation;
            EventManager.DisabilitySit(false, sitWheelChairPosition);
        }
        // Unlock movement
        LockPlayerMovement(false);

        hasTriggered = false;

        EventManager.MinigameEnded(currentActiveMinigame.coconut);
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
            conversationCanvas.StartConversation(conversationDialogueSO);
            EventManager.OnConversationEnded += HandleConversationEnded;
        }
    }
    private void HandleConversationEnded()
    {
        // Unsubscribe so it only triggers once
        EventManager.OnConversationEnded -= HandleConversationEnded;

        conversationCanvas.gameObject.SetActive(false);

        // Spawn the coconut when conversation ends
        SpawnCoconutInThePlayer();

        instructionManager.StartInstruction("Silahkan menuju gubuk untuk minum kelapa", "Oke");
        directionInfo.CreateRoute(sitPoint.position);
    }
    private void SpawnCoconutInThePlayer()
    {
        if (coconutDrinkPrefab == null || attachParent == null)
        {
            Debug.LogWarning("Coconut prefab or attach parent not assigned!");
            return;
        }

        // Instantiate coconut and parent to attach point
        CoconutDrinkManager coconut = Instantiate(coconutDrinkPrefab, attachParent.transform);
        currentCoconut = coconut;
        if (drinkAmount == 0)
        {
            hasCoconut = true;
        }
        // Reset local position/rotation so it aligns with attach point
        coconut.transform.localPosition = Vector3.zero;
        coconut.transform.localRotation = Quaternion.identity;

        // Optional: scale adjustment (in case prefab scale is off)
        coconut.transform.localScale = Vector3.one;
    }

    private void LockPlayerMovement(bool isLocked)
    {
        if (locomotionRoot != null)
            locomotionRoot.SetActive(!isLocked);
    }
}
