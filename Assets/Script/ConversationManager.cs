using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ConversationManager : MonoBehaviour
{
    [SerializeField] private Image profileImage;
    [SerializeField] private TextMeshProUGUI npcNameText;
    [SerializeField] private TextMeshProUGUI conversationText;
    [SerializeField] private Button optionOneButton;
    [SerializeField] private Button optionTwoButton;
    [SerializeField] private TextMeshProUGUI optionOneText;
    [SerializeField] private TextMeshProUGUI optionTwoText;
    [SerializeField] private TextMeshProUGUI countDownText;
    [SerializeField] private GameObject parentCountDown;
    [SerializeField] private float typingSpeed = 0.03f; // speed of typewriter effect
    [SerializeField] private AudioSource audioSource;

    private ConversationSO conversationDialogueSO;
    private int currentIndex = 0;
    private Coroutine typingCoroutine;

    public void StartConversation(ConversationSO conversationDialogue)
    {
        conversationDialogueSO = conversationDialogue;
        if (conversationDialogueSO != null)
        {
            currentIndex = 0;

            // set NPC info
            profileImage.sprite = conversationDialogueSO.npcImage;
            npcNameText.text = conversationDialogueSO.npcName;

            DisplayDialogue();
        }
    }

    private void DisplayDialogue()
    {
        if (currentIndex < 0 || currentIndex >= conversationDialogueSO.dialogueEntries.Count)
        {
            EndConversation();
            return;
        }

        DialogueEntry entry = conversationDialogueSO.dialogueEntries[currentIndex];

        // stop old typing effect if running
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }
        if (entry.audioLine != null)
        {
            audioSource.clip = entry.audioLine;
            audioSource.Play();
        }
        else
        {
            audioSource.Stop();
        }

        typingCoroutine = StartCoroutine(TypeText(entry.dialogueLine));

        // Default: hide both buttons until text is fully typed
        optionOneButton.gameObject.SetActive(false);
        optionTwoButton.gameObject.SetActive(false);
    }

    private IEnumerator TypeText(string text)
    {
        conversationText.text = "";
        foreach (char c in text)
        {
            conversationText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }

        // Once typing finishes, show responses
        SetupResponses(conversationDialogueSO.dialogueEntries[currentIndex]);
    }

    private void SetupResponses(DialogueEntry entry)
    {
        // Clear button listeners
        optionOneButton.onClick.RemoveAllListeners();
        optionTwoButton.onClick.RemoveAllListeners();

        // Show response 1 if available
        if (entry.responses.Count > 0 && !string.IsNullOrEmpty(entry.responses[0].responseText))
        {
            optionOneButton.gameObject.SetActive(true);
            optionOneText.text = entry.responses[0].responseText;

            int nextIndex = entry.responses[0].nextDialogueIndex;
            optionOneButton.onClick.AddListener(() => OnResponse(nextIndex, entry.timer));
        }

        // Show response 2 if available
        if (entry.responses.Count > 1 && !string.IsNullOrEmpty(entry.responses[1].responseText))
        {
            optionTwoButton.gameObject.SetActive(true);
            optionTwoText.text = entry.responses[1].responseText;

            int nextIndex = entry.responses[1].nextDialogueIndex;
            optionTwoButton.onClick.AddListener(() => OnResponse(nextIndex, entry.timer));
        }
    }
    private void OnResponse(int nextIndex, float timer)
    {
        // Hide buttons immediately
        optionOneButton.gameObject.SetActive(false);
        optionTwoButton.gameObject.SetActive(false);

        // If timer > 0, show countdown and start timer
        if (timer > 0)
        {
            parentCountDown.SetActive(true);
            StartCoroutine(StartCountdown(nextIndex, timer));
        }
        else
        {
            // Otherwise go directly to next dialogue
            GoToNextDialogue(nextIndex);
        }
    }

    private IEnumerator StartCountdown(int nextIndex, float duration)
    {
        float remainingTime = duration;
        while (remainingTime > 0)
        {
            countDownText.text = Mathf.CeilToInt(remainingTime).ToString();
            yield return new WaitForSeconds(1f);
            remainingTime -= 1f;
        }

        parentCountDown.SetActive(false);
        GoToNextDialogue(nextIndex);
    }

    private void GoToNextDialogue(int nextIndex)
    {
        if (nextIndex == 0)
        {
            EventManager.ConversationEnded();
            EndConversation();
            return;
        }
        else if (nextIndex == -1)
        {       
            EndConversation();
            return;
        }

        currentIndex = nextIndex;
        DisplayDialogue();
    }


    private void EndConversation()
    {
        Debug.Log("Conversation Ended.");
        conversationText.text = "";
        optionOneButton.gameObject.SetActive(false);
        optionTwoButton.gameObject.SetActive(false);

        // stop audio too
        audioSource.Stop();

        this.gameObject.SetActive(false);
    }
}
