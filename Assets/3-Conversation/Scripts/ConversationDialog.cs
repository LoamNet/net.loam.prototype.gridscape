using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Hardcoded test/debug class, expects conversation with 
/// options list with either no options or exactly 2 options.
/// </summary>
[RequireComponent(typeof(CanvasGroup))]
public class ConversationDialog : MonoBehaviour
{
    [Header("Internal dialog config")]
    [SerializeField] private string _playerSpeakerID = "Player";

    [Header("Header")]
    [SerializeField] private CanvasGroup _groupSpeakerL;
    [SerializeField] private TextMeshProUGUI _labelSpeakerL;
    [SerializeField] private CanvasGroup _groupSpeakerR;
    [SerializeField] private TextMeshProUGUI _labelSpeakerR;

    [Header("Body")]
    [SerializeField] private Button _nextButton;
    [SerializeField] private TextMeshProUGUI _body;

    [Header("Options")]
    [SerializeField] private Transform _optionsParent;
    [SerializeField] private ConversationDialogOption _optionTemplate;

    // Internal
    private Stack<ConversationDialogOption> _optionPool;
    private List<ConversationDialogOption> _optionActive;

    /// <summary>
    /// A non-swappable conversation system reference
    /// </summary>
    public ConversationSystem ConversationSystem { get; private set; }

    /// <summary>
    /// Run after initialization 
    /// </summary>
    public void Awake()
    {
        _optionTemplate.gameObject.SetActive(false);

        _optionPool = new Stack<ConversationDialogOption>();
        _optionActive = new List<ConversationDialogOption>();

        _nextButton.onClick.AddListener(DefaultOption);

        ConversationSystem = new ConversationSystem();
        ConversationSystem.OnLineUpdate += LineUpdated;
    }

    /// <summary>
    /// Internal shorthand callback for choosing the default
    /// 'baked in' option (used when player-facing options aren't present)
    /// </summary>
    private void DefaultOption()
    {
        ChooseOption(-1);
    }

    /// <summary>
    /// Configure relevant values for showing specified canvas group.
    /// </summary>
    /// <param name="target"></param>
    private void Show(CanvasGroup target)
    {
        target.alpha = 1;
        target.blocksRaycasts = true;
        target.interactable = true;
    }

    /// <summary>
    /// Configure relevant values for hiding specified canvas group.
    /// Still take up space.
    /// </summary>
    /// <param name="target"></param>
    private void Hide(CanvasGroup target)
    {
        target.alpha = 0;
        target.blocksRaycasts = false;
        target.interactable = false;
    }

    /// <summary>
    /// Respond to a callback from the conversation system by displaying new visuals related to the contents of the line.
    /// </summary>
    private void LineUpdated()
    {
        // Visualize current speaker
        if(ConversationSystem.TryGetCurrentSpeaker(out string speaker))
        {
            if(speaker.Equals(_playerSpeakerID))
            {
                Hide(_groupSpeakerL);

                Show(_groupSpeakerR);
                _labelSpeakerR.text = speaker;
            }
            else
            {
                Hide(_groupSpeakerR);

                Show(_groupSpeakerL);
                _labelSpeakerL.text = speaker;
            }
        }

        // Disaply current line and any options if present
        if(ConversationSystem.TryGetCurrentLine(out ConversationSystem.Line main, out IReadOnlyList<ConversationSystem.Line> options))
        {
            if(options != null && options.Count > 0)
            {
                _nextButton.gameObject.SetActive(false);

                for (int i = 0; i < options.Count; ++i)
                {
                    ConversationDialogOption option = GetOption();
                    option.entryLabel.text = options[i].text;
                    option.entryIndex = i;
                    option.entryButton.onClick.AddListener(() => { ChooseOption(option.entryIndex); });
                }
            }
            else
            {
                _nextButton.gameObject.SetActive(true);
            }

            _body.text = main.text;
        }
    }

    /// <summary>
    /// Load up the selected option, or if no other options are present, request the default 'next' for the line/entry
    /// </summary>
    /// <param name="optionIndex"></param>
    private void ChooseOption(int optionIndex)
    {
        // Pool options
        for (int i = _optionActive.Count - 1; i >= 0; --i)
        {
            _optionActive[i].entryButton.onClick.RemoveAllListeners();
            PoolOption(_optionActive[i], i);
        }

        if (ConversationSystem.TryGetCurrentLine(out ConversationSystem.Line main, out IReadOnlyList<ConversationSystem.Line> options))
        {
            if (optionIndex < 0)
            {
                main.Next();
            }
            else
            {
                options[optionIndex].Next();
            }
        }
    }

    /// <summary>
    /// Utility function. Attempt to get existing pooled option to use, or create new one if not created.
    /// Nothing about the return state is guaranteed.
    /// </summary>
    /// <returns></returns>
    private ConversationDialogOption GetOption()
    {
        ConversationDialogOption option = null;
        if (_optionPool.Count > 0)
        {
            option = _optionPool.Pop();
        }
        else
        {
            option = Instantiate(_optionTemplate, _optionsParent);
            option.name = "Pooled Option";
        }

        option.gameObject.SetActive(true);
        _optionActive.Add(option);
        return option;
    }

    /// <summary>
    /// Return a specific option back to the pool. If you happen to know its 
    /// index, that can be specified for faster cleanup.
    /// </summary>
    /// <param name="toPool"></param>
    /// <param name="index"></param>
    private void PoolOption(ConversationDialogOption toPool, int index = -1)
    {
        if (index < 0)
        {
            if (!_optionActive.Remove(toPool))
            {
                throw new System.Exception("Could not remove object froma active list");
            }
        }
        else
        {
            _optionActive.RemoveAt(index);
        }

        toPool.entryButton.onClick.RemoveAllListeners();
        toPool.entryLabel.text = "";
        toPool.gameObject.SetActive(false);

        _optionPool.Push(toPool);
    }
}
