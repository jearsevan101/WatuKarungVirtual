using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CountDownObtainItem : MonoBehaviour
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

            // Wait for next frame
            yield return null;

            // Decrease timer
            timeLeft -= Time.deltaTime;
        }

        // Final state (0)
        countDownText.text = "0";
        countDownImage.fillAmount = 0f;

        EventManager.CountdownObtainedEnded();

        countdownRoutine = null;
    }
}
