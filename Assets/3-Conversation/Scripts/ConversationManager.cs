using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConversationManager : MonoBehaviour
{
    [Header("Debug References")]
    [SerializeField] private TextAsset _conversationJSON;
    [SerializeField] private Button _showDialogButton;

    [Header("Dialog")]
    [SerializeField] private ConversationDialog _dialogTemplate;
    [SerializeField] private Transform _dialogParent;

    // internal
    private ConversationDialog _dialogInstance;
    private CanvasGroup _dialogCanvasGroup;

    public void Start()
    {
        _dialogInstance = GameObject.Instantiate(_dialogTemplate, _dialogParent);
        _dialogCanvasGroup = _dialogInstance.GetComponent<CanvasGroup>();
        _dialogInstance.ConversationSystem.OnEnd += EndConversation;
        _dialogInstance.ConversationSystem.OnMessage += Debug.Log; // Hook up to a real messaging system instead of a generic logging thing

        EndConversation();

        // DEBUG - Lambdas in this style are discouraged.
        _dialogInstance.ConversationSystem.OnEnd += () => {
            _showDialogButton.gameObject.SetActive(true);
        };
        _showDialogButton.onClick.AddListener(() => {
            _showDialogButton.gameObject.SetActive(false);
            StartConversation(_conversationJSON.text);
        });

    }

    public void StartConversation(string conversationJSON)
    {
        _dialogInstance.ConversationSystem.Load(conversationJSON);

        _dialogCanvasGroup.alpha = 1;
        _dialogCanvasGroup.interactable = true;
        _dialogCanvasGroup.blocksRaycasts = true;
    }

    private void EndConversation()
    {
        _dialogCanvasGroup.alpha = 0;
        _dialogCanvasGroup.interactable = false;
        _dialogCanvasGroup.blocksRaycasts = false;
    }


}
