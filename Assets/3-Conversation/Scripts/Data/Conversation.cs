using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Conversation
{
    public ConversationMetadata metadata = default;
    public List<ConversationLine> lines = default;
    public ConversationEnd end = default;
}
