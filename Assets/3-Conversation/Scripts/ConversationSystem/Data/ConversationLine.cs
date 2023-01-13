using System.Collections.Generic;

[System.Serializable]
public class ConversationLine
{
    public string speaker = null;
    public string label = null;
    public string message = null;
    public string text = "";
    public string jump = null;
    public List<ConversationOption> options = null;
    public string notes = null;
}
