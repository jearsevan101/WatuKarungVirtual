using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    [Header("Minigame Order")]
    [Tooltip("Set the order of minigames manually here.")]
    public List<currentActiveMinigame> minigameOrder = new List<currentActiveMinigame>
    {
        currentActiveMinigame.coconut,
        currentActiveMinigame.sand,
        currentActiveMinigame.clamp
    };

    private int currentMinigameIndex = 0;

    private void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    void Start()
    {
        EventManager.OnMinigameEnded += HandleMinigameEnded;
    }

    public void SetTheFirstDestination()
    {
        if (minigameOrder.Count > 0)
        {
            EventManager.ChangeActiveMinigame(minigameOrder[0]);
        }
    }

    private void OnDestroy()
    {
        EventManager.OnMinigameEnded -= HandleMinigameEnded;
    }

    private void HandleMinigameEnded(currentActiveMinigame currentMinigame)
    {
        // Find current index safely (if manually triggered out of order)
        currentMinigameIndex = minigameOrder.IndexOf(currentMinigame);

        if (currentMinigameIndex == -1)
        {
            Debug.LogWarning($"Minigame {currentMinigame} not found in order list!");
            return;
        }

        // Move to next minigame
        int nextIndex = (currentMinigameIndex + 1) % minigameOrder.Count;
        var nextMinigame = minigameOrder[nextIndex];

        Debug.Log($"Minigame {currentMinigame} ended. Next minigame: {nextMinigame}");

        EventManager.ChangeActiveMinigame(nextMinigame);
    }
}
