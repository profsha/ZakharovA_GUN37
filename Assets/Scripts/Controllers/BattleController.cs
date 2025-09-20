using System;
using Controllers.Commands;
using GameState;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using Zenject;

public class BattleController
{
    private readonly Controls.GameActions _game;
    private readonly SignalBus _signalBus;
    private readonly IGameState _gameState;
    
    public BattleController(SignalBus signalBus,
        IGameState gameState,
        Controls.GameActions game)
    {
        _signalBus = signalBus;
        _game = game;
        _gameState = gameState;
        _game.Confirm.canceled += OnConfirm;
        _game.Cancel.canceled += OnCancel;
    }
    
    private void OnCancel(InputAction.CallbackContext obj)
    {
        _signalBus.Fire(GameEvent.Cancel);
    }

    private void OnConfirm(InputAction.CallbackContext obj)
    {
        _signalBus.Fire(GameEvent.Confirm);
    }
}
