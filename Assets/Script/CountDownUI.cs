using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CountDownUI : MonoBehaviour
{
    [SerializeField] private Image countDownImage;
    [SerializeField] private TextMeshProUGUI countDownText;

    private Coroutine countdownRoutine;

    public void StartCountDown(float duration)
    {
        // If a countdown is already running, stop it
        if (countdownRoutine != null)
        {
            StopCoroutine(countdownRoutine);
        }

        // Start new countdown
        countdownRoutine = StartCoroutine(CountDownCoroutine(duration));
        EventManager.OnStopCountDown += HandleStopCountDown;
    }

    private void HandleStopCountDown()
    {
        // Stop the coroutine if running
        if (countdownRoutine != null)
        {
            StopCoroutine(countdownRoutine);
            countdownRoutine = null;
        }

        // Reset UI
        if (countDownText != null) countDownText.text = "0";
        if (countDownImage != null) countDownImage.fillAmount = 0f;

        // Unsubscribe to prevent multiple calls
        EventManager.OnStopCountDown -= HandleStopCountDown;
    }
    private IEnumerator CountDownCoroutine(float duration)
    {
        float timeLeft = duration;

        while (timeLeft > 0f)
        {
            // Show as integer (no decimals)
            int seconds = Mathf.CeilToInt(timeLeft);
            countDownText.text = seconds.ToString();

            // Update radial/bar image
            countDownImage.fillAmount = timeLeft / duration;
            
            EventManager.CountdownTick(timeLeft);
            EventManager.CountdownInstructionTick(timeLeft);

            // Wait for next frame
            yield return null;

            // Decrease timer
            timeLeft -= Time.deltaTime;
        }

        // Final state (0)
        countDownText.text = "0";
        countDownImage.fillAmount = 0f;

        EventManager.CountdownEnded();
        EventManager.OnStopCountDown -= HandleStopCountDown;

        countdownRoutine = null;
    }
}
