using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Hardcoded test/debug class, expects conversation with 
/// options list with either no options or exactly 2 options.
/// </summary>
public class ConversationTest : MonoBehaviour
{
    [SerializeField] private Button _chat;

    [SerializeField] private GameObject _dialog;
    [SerializeField] private Button _buttonBody;
    
    [SerializeField] private Button _option1;
    [SerializeField] private TMPro.TextMeshProUGUI _option1Text;
    [SerializeField] private Button _option2;
    [SerializeField] private TMPro.TextMeshProUGUI _option2Text;

    [SerializeField] private TMPro.TextMeshProUGUI _speaker;
    [SerializeField] private TMPro.TextMeshProUGUI _body;

    [SerializeField] private TextAsset _convoJson;
    [SerializeField] private ConversationSystem _system;

    public void Awake()
    {
        HideDialog();
    }

    public void Start()
    {
        _chat.onClick.AddListener(ShowDialog);
        _buttonBody.onClick.AddListener(Next);
        _option1.onClick.AddListener(Next0);
        _option2.onClick.AddListener(Next1);

        _system = new ConversationSystem();
        _system.OnLineUpdate += LineUpdated;
        _system.OnMessage += Message;
        _system.OnEnd += HideDialog;

    }

    private void ShowDialog()
    {
        _chat.gameObject.SetActive(false);
        _dialog.SetActive(true);

        _system.Load(_convoJson);
    }

    private void HideDialog()
    {
        _chat.gameObject.SetActive(true);
        _dialog.SetActive(false);
    }

    private void LineUpdated()
    {
        if(_system.TryGetCurrentSpeaker(out string speaker))
        {
            _speaker.text = speaker;
        }

        if(_system.TryGetCurrentLine(out ConversationSystem.Line main, out IReadOnlyList<ConversationSystem.Line> options))
        {
            if(options != null && options.Count > 0)
            {
                _buttonBody.gameObject.SetActive(false);
                _option1.gameObject.SetActive(true);
                _option2.gameObject.SetActive(true);

                _body.text = main.text;
                _option1Text.text = options[0].text;
                _option2Text.text = options[1].text;
            }
            else
            {
                _buttonBody.gameObject.SetActive(true);
                _option1.gameObject.SetActive(false);
                _option2.gameObject.SetActive(false);

                _body.text = main.text;
            }
        }
    }

    private void Message(string message)
    {
        Debug.Log($"Message: {message}");
    }

    private void Next()
    {
        SelectOption();
    }

    private void Next0()
    {
        SelectOption(0);
    }

    private void Next1()
    {
        SelectOption(1);
    }

    private void SelectOption(int index = -1)
    {
        if (_system.TryGetCurrentLine(out ConversationSystem.Line main, out IReadOnlyList<ConversationSystem.Line> options))
        {
            if (index < 0)
            {
                main.Next();
            }
            else
            {
                options[index].Next();
            }
        }
    }
}
