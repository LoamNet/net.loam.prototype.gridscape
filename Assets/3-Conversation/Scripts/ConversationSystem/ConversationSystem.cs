using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public partial class ConversationSystem : IDisposable
{
    // Inspector
    [Header("Internal Preview")] [SerializeField] private Conversation conversation;

    // Internals
    private int _currentLineIndex;
    private ConversationLine _currentLineRaw;
    private ConversationSystemLine _currentLinePrimary;
    private List<ConversationSystemLine> _currentLineOptions;
    private Dictionary<string, int> _jumpLookup;

    // Callbacks
    public Action OnLineUpdate { get; private set; }
    public Action<string> OnMessage { get; private set; }

    /// <summary>
    /// Barebones get-the-object-alive style. 
    /// </summary>
    public ConversationSystem()
    {
        _currentLineIndex = 0;
        _currentLineRaw = null;
        _currentLinePrimary = null;
        _currentLineOptions = new List<ConversationSystemLine>();
        _jumpLookup = new Dictionary<string, int>();
    }

    /// <summary>
    /// Prepare and begin display of the specified conversation
    /// </summary>
    /// <param name="conversationAsset"></param>
    public void Load(TextAsset conversationAsset)
    {
        ConfigureInternals();
        LoadConversation(conversationAsset);
        LoadLine(0);
    }

    /// <summary>
    /// Tells you if this conversation system is still driving a conversation.
    /// </summary>
    /// <returns></returns>
    public bool ConversationActive()
    {
        return _currentLineRaw != null;
    }

    /// <summary>
    /// Access the existing line information and any associated options in a package
    /// that allows a specific line to be interacted with post-use.
    /// </summary>
    /// <param name="line"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public bool TryGetCurrentLine(out ConversationSystemLine line, out IReadOnlyCollection<ConversationSystemLine> options)
    {
        if (ConversationActive())
        {
            line = _currentLinePrimary;
            options = _currentLineOptions;
            return true;
        }

        line = null;
        options = null;
        return false;
    }

    /// <summary>
    /// Deterministic cleanup
    /// </summary>
    public void Dispose()
    {
        if(_currentLinePrimary != null)
        {
            _currentLinePrimary.Invalidate();
        }

        if (_currentLineOptions != null)
        {
            for (int i = 0; i < _currentLineOptions.Count; ++i)
            {
                _currentLineOptions[i].Invalidate();
            }

            _currentLineOptions.Clear();
        }

        if (_jumpLookup != null)
        {
            _jumpLookup.Clear();
        }
    }

    /// <summary>
    /// Configure and clean the internal variables as needed.
    /// </summary>
    private void ConfigureInternals()
    {
        Dispose();

        _currentLineIndex = 0;
        _currentLineRaw = null;
        _jumpLookup.Add("end", int.MaxValue);
    }

    /// <summary>
    /// Determine what the next line would be when given a specific
    /// current line and an optional jump string. Can be used from options
    /// or from primary lines, and won't try and look up the jump value
    /// for the provided line index.
    /// </summary>
    /// <param name="currentLineIndex"></param>
    /// <param name="jump"></param>
    private int DetermineNext(int currentLineIndex, string jump)
    {
        // If we have a jump target, try and find it.
        if (!string.IsNullOrWhiteSpace(jump))
        {
            if (_jumpLookup.TryGetValue(jump, out int targetLine))
            {
                return targetLine;
            }
            else
            {
                throw new System.Exception($"Unspecified jump target '{jump}'");
            }
        }

        return currentLineIndex + 1;
    }

    /// <summary>
    /// Given a specific asset, load the asset into the conversation system and build a jump lookup
    /// </summary>
    /// <param name="conversationAsset"></param>
    private void LoadConversation(TextAsset conversationAsset)
    {
        // Clear old info
        _jumpLookup.Clear();
        _currentLineIndex = 0;

        // Parse out conversation data and override existing content
        string text = conversationAsset.text;
        conversation = JsonUtility.FromJson<Conversation>(text);

        // Build lookup
        for(int i = 0; i < conversation.lines.Count; ++i)
        {
            ConversationLine line = conversation.lines[i];
            if (!string.IsNullOrWhiteSpace(line.label))
            {
                _jumpLookup.Add(line.label, i);
            }
        }
    }

    /// <summary>
    /// Load the specified line from the Conversation data by index.
    /// Any index past the end of the line list is considered the 'end' of the conversation.
    /// </summary>
    /// <param name="toLoad">index into conversation.lines of the line to load.</param>
    private void LoadLine(int toLoad)
    {
        List<ConversationLine> lines = conversation.lines;

        // Handle transitioning to end case (or any excess line count as a reasonable default of 'end')
        if (toLoad >= lines.Count)
        {
            ConfigureInternals();
            OnLineUpdate?.Invoke();
            ProcessMessage(conversation.end.message);
            return;
        }

        // Configure as a clean slate, but don't dispose of list object itself.
        _currentLinePrimary.Invalidate();
        for(int i = 0; i < _currentLineOptions.Count; ++i)
        {
            _currentLineOptions[i].Invalidate();
        }
        _currentLineOptions.Clear();

        // process and load primary line
        _currentLineIndex = toLoad;
        _currentLineRaw = conversation.lines[_currentLineIndex];
        _currentLinePrimary = new ConversationSystemLine(this, _currentLineRaw.text, _currentLineRaw.jump, _currentLineIndex);

        // process any options present. If none, should be empty.
        if(_currentLineRaw.options != null)
        {
            for(int i = 0; i < _currentLineRaw.options.Count; ++i)
            {
                ConversationOption option = _currentLineRaw.options[i];
                ConversationSystemLine optionLine = new ConversationSystemLine(this, option.text, option.jump, _currentLineIndex);
                _currentLineOptions.Add(optionLine);
            }
        }

        // Dispatch relevant events
        OnLineUpdate?.Invoke();
    }

    /// <summary>
    /// Send a message outwards, argument driven by the conversation data.
    /// Useful for responding to specific events during conversations or verifying
    /// specific actions were used.
    /// 
    /// Nothing is dispatched if the message is null or empty.
    /// </summary>
    /// <param name="message"></param>
    private void ProcessMessage(string message)
    {
        if(string.IsNullOrWhiteSpace(message))
        {
            return;
        }

        OnMessage?.Invoke(message);
    }
}