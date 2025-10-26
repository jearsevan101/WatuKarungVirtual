using System;
using UnityEngine;


public enum currentActiveMinigame{
    coconut,
    sand,
    clamp
}
public static class EventManager
{
    // --- Events ---
    public static event Action OnConversationEnded;
    public static event Action OnCountdownEnded;
    public static event Action OnCountDownObtainedEnded;
    public static event Action OnStopCountDown;
    public static event Action<float> OnCountdownTick; // sends remaining time
    public static event Action<float> OnCountdownInstructionTick; // sends remaining time
    public static event Action<int> OnCurrentItemObtained;
    public static event Action<bool> OnInstructionResponsesClicked;
    public static event Action OnBackToMainMenu;
    public static event Action<bool> OnModeNormal;
    public static event Action<bool, GameObject> OnDisabilitySit;

    public static event Action OnClampObtained;
    public static event Action<currentActiveMinigame> OnActiveMinigame;
    public static event Action<currentActiveMinigame> OnMinigameEnded;

    // --- Invokers ---

    public static void MinigameEnded(currentActiveMinigame currentMinigame)
    {
        OnMinigameEnded?.Invoke(currentMinigame);
    }
    public static void ChangeActiveMinigame(currentActiveMinigame activeMinigame)
    {
        OnActiveMinigame?.Invoke(activeMinigame);
    }
    public static void DisabilitySit(bool disabilitySit, GameObject newParent)
    {
        OnDisabilitySit?.Invoke(disabilitySit, newParent);
    }
    public static void ModeNormal(bool isModeNormal)
    {
        OnModeNormal?.Invoke(isModeNormal);
    }
    public static void BackToMainMenu()
    {
        OnBackToMainMenu?.Invoke();
    }

    public static void ConversationEnded()
    {
        OnConversationEnded?.Invoke();
    }
    public static void CurrentItemObtained(int count)
    {
        OnCurrentItemObtained?.Invoke(count);
    }
    public static void CountdownEnded()
    {
        OnCountdownEnded?.Invoke();
    }
    public static void CountdownObtainedEnded()
    {
        OnCountDownObtainedEnded?.Invoke();
    }
    public static void StopCountDown()
    {
        OnStopCountDown?.Invoke();
    }
    public static void CountdownTick(float timeRemaining)
    {
        OnCountdownTick?.Invoke(timeRemaining);
    }

    public static void CountdownInstructionTick(float timeRemaining)
    {
        OnCountdownInstructionTick?.Invoke(timeRemaining);
    }

    public static void InstructionResponses(bool response)
    {
        OnInstructionResponsesClicked?.Invoke(response);
    }

    public static void ClampObtained()
    {
        OnClampObtained?.Invoke();
    }
}
