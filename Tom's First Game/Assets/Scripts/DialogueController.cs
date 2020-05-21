﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;




public class DialogueController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _dialogueText = null;
    public Image _yesArrow;
    public Image _noArrow;
    public GameObject _dialogueCanvas;
    public GameObject _choiceCanvas;
    public int _characterDelay = 1;
    
    private PlayerController _playerScript;
    private Interactor _interactor;
    private Queue<string> _lines = new Queue<string>();
    private int _delay = 0;
    private bool _isSpeaking = false;
    private int _remainingCharacters;
    private bool _isActive = false;
    private string _choiceText;
    private bool _choiceLoaded = false;
    private bool _isChoosing = false;
    private bool _yesSelected = true;
    private DialogueNode _currentNode;
    
    public string ChoiceText{get {return _choiceText;} set {_choiceText=value;}}
    public bool ChoiceLoaded{get {return _choiceLoaded;} set{_choiceLoaded=value;}}


    void Start()
    {
        _dialogueCanvas.SetActive(false);
        _playerScript = FindObjectOfType<PlayerController>();
        _interactor = FindObjectOfType<Interactor>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (_isActive && !_isChoosing) 
            {
                advanceDialogue();
            }
            if (_isChoosing && _remainingCharacters < 1) 
            {
                GlobalPlayerController.AddDecision(_choiceText, _yesSelected);
                EndDialogue();
            }
        }
        
        if ((_isSpeaking || _isChoosing) && _delay == 0){
            AddChar();
        }
        if (_delay > 0) _delay--;
        if (_isChoosing)
        {
            if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.S)) {
                _yesSelected = !_yesSelected;
            }
            _yesArrow.enabled = _yesSelected;
            _noArrow.enabled = !_yesSelected;
        }
    }
 
    public void StartDialogue(DialogueNode node)
    {
        _currentNode = node;
        _isActive = true;
        _dialogueCanvas.SetActive(true);
        _choiceCanvas.SetActive(false);
        _playerScript.CanMove = false;
        _interactor.StartConversation();
        _lines.Clear();
        foreach(string sentence in _currentNode.Dialogue)
        {
            _lines.Enqueue(sentence);
        }
        if (_lines.Count == 0) DialogueEmpty();
        DisplayNextSentence();
    }
    
    private bool DisplayNextSentence()
    {
        if (_lines.Count == 0) return false;
        string sentence = _lines.Dequeue();
        _dialogueText.text = sentence;
        _dialogueText.maxVisibleCharacters = 0;
        _remainingCharacters = sentence.Length;
        _isSpeaking = true;
        _delay = _characterDelay;
        return true;
    }

    private void advanceDialogue()
    {
        if (!_isSpeaking){
            if (!DisplayNextSentence()) DialogueEmpty();
        }
        else
        {
            _dialogueText.maxVisibleCharacters += _remainingCharacters;
            _remainingCharacters = 0;
            _isSpeaking = false;
        }
    }

    private void StartChoice()
    {
        _isChoosing = true;
        _choiceCanvas.SetActive(true);
        string sentence = _choiceText;
        _dialogueText.text = sentence;
        _dialogueText.maxVisibleCharacters = 0;
        _remainingCharacters = sentence.Length;
        _isSpeaking = false;
        _delay = _characterDelay;
    }

    private void AddChar()
    {
        _dialogueText.maxVisibleCharacters++;
        _remainingCharacters--;
        if (_remainingCharacters == 0) _isSpeaking = false;
        else _delay = _characterDelay;
    }

    private void EndDialogue()
    {
        _dialogueCanvas.SetActive(false);
        _playerScript.CanMove = true;
        _interactor.EndConversation();
        _isActive = false;
        _choiceLoaded = false;
        _isChoosing = false;
        _currentNode.AdvanceNode();
    }

    private void DialogueEmpty()
    {
        if (!_choiceLoaded) EndDialogue();
        else StartChoice();
    }
}
