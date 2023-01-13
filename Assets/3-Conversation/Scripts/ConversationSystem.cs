using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConversationSystem : MonoBehaviour
{
    public TextAsset json;
    public Conversation conversation;

    private void Start()
    {
        string text = json.text;
        conversation = JsonUtility.FromJson<Conversation>(text);
    }
}
