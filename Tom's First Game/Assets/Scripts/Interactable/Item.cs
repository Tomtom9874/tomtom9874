﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour, IInteractable
{
    [SerializeField] private string _name = "null item";
    [SerializeField] private string _description = "NULL DESCRIPTION";
    private Vector2 _position;
    private OnDestroy _destroyer;
    private DialogueData _dialogueData;
    private SpriteRenderer _image;

    private void Awake()
    {
        _position = GetComponent<Transform>().position;
        _destroyer = GetComponent<OnDestroy>();
        _dialogueData = GetComponentInChildren<DialogueData>();
        _image = GetComponent<SpriteRenderer>();
    }

    public void InteractedWith() 
    {
        _dialogueData.TriggerDialogue();
        _destroyer.SelfDestruct();
        GlobalInventoryController.AddItem(_name, 1);
    }

    public string GetName()
    {
        return _name;
    }

    public string GetDescription()
    {
        return _description;
    }

    public Sprite GetSprite()
    {
        _image = GetComponent<SpriteRenderer>();
        if (_image == null){
            Debug.Log("The image is null");
        }
        return _image.sprite;
    }
}
