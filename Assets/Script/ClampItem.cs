using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ClampItem : MonoBehaviour
{
    [SerializeField] private CountDownObtainItem countDownUI;
    [SerializeField] private float timeCountDown;
    [SerializeField] private List<GameObject> prefabClamp;
    [SerializeField] private GameObject prefabParent;
    [SerializeField] private Transform playerTransform; // XR Origin Camera or root


    private GameObject spawnedPrefab; // reference to spawned prefab
    private XRGrabInteractable grabInteractable;
    private Coroutine countdownCoroutine;
    private Coroutine hapticCoroutine;

    private bool isCountingDown = false;

    private Vector3 previousLocalPosition;
    private Quaternion previousLocalRotation;

    private void Start()
    {
        countDownUI.gameObject.SetActive(false);
        SpawnClamp(); // spawn once at start
    }
    public void SpawnClamp()
    {
        if (prefabClamp.Count > 0 && prefabParent != null)
        {
            if (spawnedPrefab != null)
            {
                spawnedPrefab.SetActive(true);
                spawnedPrefab.transform.localPosition = previousLocalPosition;
                spawnedPrefab.transform.localRotation = previousLocalRotation;
                return;
            }

            int randomIndex = Random.Range(0, prefabClamp.Count);
            GameObject randomPrefab = prefabClamp[randomIndex];

            spawnedPrefab = Instantiate(randomPrefab, prefabParent.transform);
            spawnedPrefab.transform.localPosition = Vector3.zero;

            previousLocalPosition = spawnedPrefab.transform.localPosition;
            previousLocalRotation = spawnedPrefab.transform.localRotation;

            // Make sure prefab has XRGrabInteractable
            grabInteractable = spawnedPrefab.GetComponent<XRGrabInteractable>();
            if (grabInteractable == null)
            {
                grabInteractable = spawnedPrefab.AddComponent<XRGrabInteractable>();
            }

            // Listen for grab event
            grabInteractable.selectEntered.AddListener(OnGrabbed);
            grabInteractable.selectExited.AddListener(OnReleased);
        }
        else
        {
            Debug.LogWarning("PrefabClamp list is empty or prefabParent is not assigned!");
        }
    }

    private void OnGrabbed(SelectEnterEventArgs args)
    {
        if (spawnedPrefab.CompareTag("QuestItem")) // check tag
        {
            Debug.Log("QuestItem grabbed!");
            StartCountDown(args.interactorObject);
        }
    }
    private void OnReleased(SelectExitEventArgs args) 
    {
        if (isCountingDown)
        {
            Debug.Log("Clamp released before countdown finished!");
            StopAllCoroutines();
            isCountingDown = false;

            // Stop UI and vibration
            if (countDownUI != null)
                countDownUI.gameObject.SetActive(false);

            EventManager.OnCountDownObtainedEnded -= HandleCountDownEnded;
        }
    }
    public void StartCountDown(IXRSelectInteractor interactor)
    {
        if (countDownUI == null)
        {
            Debug.LogWarning("CountDownUI is not assigned!");
            return;
        }

        countDownUI.gameObject.SetActive(true);
        Vector3 uiPosition = playerTransform.position + playerTransform.forward * 1.0f + Vector3.up * 0.2f;
        countDownUI.transform.position = uiPosition;

        StartCoroutine(SetCountDownFacingPlayerNextFrame());
        countDownUI.StartCountDown(timeCountDown);

        EventManager.OnCountDownObtainedEnded += HandleCountDownEnded;

        // Start countdown and haptic feedback coroutines
        countdownCoroutine = StartCoroutine(CountdownRoutine());
        hapticCoroutine = StartCoroutine(HapticFeedbackRoutine(interactor));

        isCountingDown = true;
    }
    private IEnumerator CountdownRoutine() 
    {
        yield return new WaitForSeconds(timeCountDown);
        HandleCountDownEnded();
    }
    private IEnumerator HapticFeedbackRoutine(IXRSelectInteractor interactor)
    {
        float halfTime = timeCountDown / 2f;
        float elapsed = 0f;

        var controller = (interactor as XRBaseControllerInteractor)?.xrController;

        while (elapsed < halfTime)
        {
            if (controller != null)
            {
                // Low frequency, medium intensity
                controller.SendHapticImpulse(0.5f, 0.1f);
            }

            yield return new WaitForSeconds(0.1f);
            elapsed += 0.1f;
        }
    }
    /*public void StartCountDown()
    {
        if (countDownUI != null)
        {
            countDownUI.gameObject.SetActive(true);
            // Position the countdown UI in front of the player
            Vector3 uiPosition = playerTransform.position + playerTransform.forward * 1.0f + Vector3.up * 0.2f;
            countDownUI.transform.position = uiPosition;

            StartCoroutine(SetCountDownFacingPlayerNextFrame());
            countDownUI.StartCountDown(timeCountDown);

            // Subscribe to countdown ended event
            EventManager.OnCountDownObtainedEnded += HandleCountDownEnded;
        }
        else
        {
            Debug.LogWarning("CountDownUI is not assigned!");
        }
    }*/
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
        if (!isCountingDown) return;

        isCountingDown = false;
        // Unsubscribe so we don't get multiple calls
        EventManager.OnCountDownObtainedEnded -= HandleCountDownEnded;

        EventManager.ClampObtained();


        // Destroy the prefab we spawned earlier
        if (spawnedPrefab != null)
        {
            spawnedPrefab.SetActive(false);
            Debug.Log("Spawned prefab destroyed after countdown ended!");
        }
        // Hide countdown UI
        if (countDownUI != null)
        {
            countDownUI.gameObject.SetActive(false);
        }

        if (hapticCoroutine != null)
            StopCoroutine(hapticCoroutine);
    }
}
