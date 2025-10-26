using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoconutDrinkManager : MonoBehaviour
{
    [SerializeField] private CountDownUI countDownUI;
    [SerializeField] private float timeCountDown;
    public void StartCountDown()
    {
        if (countDownUI != null)
        {
            countDownUI.StartCountDown(timeCountDown);

            // Subscribe to countdown ended event
            EventManager.OnCountdownEnded += HandleCountDownEnded;
        }
        else
        {
            Debug.LogWarning("CountDownUI is not assigned!");
        }
    }

    private void HandleCountDownEnded()
    {
        // Unsubscribe so we don't get multiple calls
        EventManager.OnCountdownEnded -= HandleCountDownEnded;

        // Destroy this coconut object
        Destroy(gameObject);

        Debug.Log(" Coconut destroyed after countdown ended!");
    }
}
