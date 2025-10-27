using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static DirectionInfoManager;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private Canvas mainCanvas;
    [SerializeField] private Button normalButton;
    [SerializeField] private Button disabilitasButton;

    [SerializeField] private Transform playerTransform; // XR Origin Camera or root
    [SerializeField] private GameObject locomotionRoot;

    [SerializeField] private Transform spawnDisabilitasParent;
    [SerializeField] private Transform spawnNormalParent;

    [SerializeField] private Transform playerSpawnStart;

    [SerializeField] private float exploringTime = 180f;
    [SerializeField] private GameObject informationUI;
    [SerializeField] private DirectionInfoManager directionInfo;

    [SerializeField] private InformationCanvas informationCanvas;

    [SerializeField] private InstructionManager instructionManager;

    private Coroutine countdownRoutine;
    private void Start()
    {
        LockPlayerMovement(true);
        normalButton.onClick.AddListener(() =>
        {
            SpawnPlayerNonDisabilitas();
            EventManager.ModeNormal(true);
            HandleGameStart();
        });

        disabilitasButton.onClick.AddListener(() =>
        {
            SpawnPlayerDisabilitas();
            EventManager.ModeNormal(false);
            HandleGameStart();
        });
        EventManager.OnBackToMainMenu += HandleBackToMainMenu;
    }

    private void HandleGameStart()
    {
        LockPlayerMovement(false);
        instructionManager.StartInstruction("Selamat datang! Nikmati 5 menit pertama ini untuk menjelajahi keindahan Pantai Watu Karung virtual secara bebas. Gunakan waktu ini sebaik-baiknya!", "Oke");

        EventManager.OnInstructionResponsesClicked += HandleInstructionResponse;
    }

    private void HandleInstructionResponse(bool response)
    {
        EventManager.OnInstructionResponsesClicked -= HandleInstructionResponse;
        instructionManager.StartInstruction("Setelah waktu eksplorasi selesai, kami akan memberikan petunjuk detail untuk memulai tantangan dan permainan yang sesungguhnya.", "Oke");

        EventManager.OnInstructionResponsesClicked += HandleInstruction;
    }
    private void HandleInstruction(bool response)
    {
        EventManager.OnInstructionResponsesClicked -= HandleInstruction;
        StartCountDown(exploringTime);
        informationCanvas.UpdateInstruksi("Silahkan Menjelajah");
    }
    private void HandleBackToMainMenu()
    {
        LockPlayerMovement(true);
        ShowMainCanvas();
        playerTransform.position = playerSpawnStart.position;
        playerTransform.rotation = playerSpawnStart.rotation;
    }
    private void ShowMainCanvas()
    {
        mainCanvas.gameObject.SetActive(true);
        SetVisibilityInformationUI(false);
    }

    private void SpawnPlayerDisabilitas()
    {
        if (spawnDisabilitasParent.childCount == 0) return;

        int randomIndex = Random.Range(0, spawnDisabilitasParent.childCount);
        Transform randomSpawn = spawnDisabilitasParent.GetChild(randomIndex);

        playerTransform.position = randomSpawn.position;
        playerTransform.rotation = randomSpawn.rotation;
    }

    private void SpawnPlayerNonDisabilitas()
    {
        if (spawnNormalParent.childCount == 0) return;

        int randomIndex = Random.Range(0, spawnNormalParent.childCount);
        Transform randomSpawn = spawnNormalParent.GetChild(randomIndex);

        playerTransform.position = randomSpawn.position;
        playerTransform.rotation = randomSpawn.rotation;
    }

    private void LockPlayerMovement(bool isLocked)
    {
        if (locomotionRoot != null)
            locomotionRoot.SetActive(!isLocked);
    }

    private void SetVisibilityInformationUI(bool isVisible)
    {
        informationUI.SetActive(isVisible);
    }

    private void SetDirectionToDestination()
    {
        GameManager.Instance.SetTheFirstDestination();
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
        // Unsubscribe to prevent multiple calls
        EventManager.OnCountdownEnded -= HandleStopCountDown;
        // Stop the coroutine if running
        if (countdownRoutine != null)
        {
            StopCoroutine(countdownRoutine);
            countdownRoutine = null;
        }
        informationCanvas.HideUI();
        SetVisibilityInformationUI(true);
        SetDirectionToDestination();

        instructionManager.StartInstruction("Waktu menjelajah telah habis, silahkan ikuti petunjuk arah", "Oke");
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
}
