using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Pauser : MonoBehaviour
{
    private PlayerInput _playerInput;
    public bool isGameOver = false;
    public bool isPaused = false;

    private void Start()
    {
        _playerInput = FindObjectOfType<PlayerInput>();
    }

    public void Pause()
    {
        if(!isPaused)
        {
            _playerInput.SwitchCurrentActionMap("Menu");
            isPaused = true;
        }
        if(isPaused)
        {
            _playerInput.SwitchCurrentActionMap("Gameplay");
            isPaused = false;
        }
    }
}
