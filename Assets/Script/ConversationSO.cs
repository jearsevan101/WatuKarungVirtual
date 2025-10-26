using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Conversation", menuName = "Dialogue/Conversation")]
public class ConversationSO : ScriptableObject
{
    [Header("NPC Info")]
    public Sprite npcImage;
    public string npcName;

    [Header("Dialogue List")]
    public List<DialogueEntry> dialogueEntries;
}

[System.Serializable]
public class DialogueEntry
{
    [TextArea(3, 10)]
    public string dialogueLine;
    public AudioClip audioLine;
    public float timer;
    public List<ResponseOption> responses;
}

[System.Serializable]
public class ResponseOption
{
    [TextArea(2, 5)]
    public string responseText;

    // instead of linking to another ScriptableObject,
    // we point to the index in the same list
    public int nextDialogueIndex = -1;
}