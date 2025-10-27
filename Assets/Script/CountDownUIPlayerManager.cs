using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CountDownUIPlayerManager : MonoBehaviour
{
    [SerializeField] private CountDownObtainItem countDownItem;

    private void Start()
    {
        VisibilityCanvas(false);
        EventManager.OnStartCountDown += HandleStartCountDown;
        EventManager.OnHideCountdown += VisibilityCanvas;
    }

    private void HandleStartCountDown(float countDown)
    {
        VisibilityCanvas(true);
        countDownItem.StartCountDown(countDown);
    }

    private void VisibilityCanvas(bool isVisible)
    {
        countDownItem.gameObject.SetActive(isVisible);
    }
}
