using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GameState;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

public class PlayerController: IDisposable
{
    private readonly SignalBus _signalBus;
    private readonly IGameState _gameState;
    private readonly IGameplayCommand _command;
    
    public PlayerController(SignalBus signalBus, IGameState gameState, IGameplayCommand command)
    {
        _signalBus = signalBus;
        _gameState = gameState;
        _command = command;
        _signalBus.Subscribe<GameEvent>(StartPlay);
    }

    private void StartPlay(GameEvent gameEvent)
    {
        if (_gameState.Lock) return;
        Debug.Log($"StartPlay {gameEvent} {_gameState.Status}");
        if (gameEvent != GameEvent.Confirm) return;
        if (_gameState.Status != GameStatus.Confirm && _gameState.Status != GameStatus.Attack) return;
        if (!_gameState.Cell) return;
        
        _gameState.Lock = true;

        var destination = _gameState.Cell;
        var unit = _gameState.Unit;

        if (destination.Unit is null)
        {
            unit.Cell.Unit = null;
            unit.OnMovementCompleted += StopPlay;
            unit.MoveTo(destination);
        }
    }

    private void StopPlay(Unit unit)
    {
        var destination = _gameState.Cell;
        if (destination.HomeTeam != Team.None &&
            destination.HomeTeam != _gameState.CurrentTeam)
        {
            _gameState.Unit.Type = UnitType.Queen;
        }
        unit.Cell = destination;
        _gameState.Cell = null;
        
        if (_command.TargetUnits.TryGetValue(unit.Cell, out var targetUnit))
        {
            _gameState.Status = GameStatus.Attack;
            targetUnit.Cell.Unit = null;
            targetUnit.gameObject.SetActive(false);
            UnityEngine.Object.Destroy(targetUnit.gameObject);
            _command.Interact(unit.Cell);
            if (_command.Requires.Any())
            {
                _gameState.Lock = false;
                unit.OnMovementCompleted -= StopPlay;
                _signalBus.Fire(GameEvent.NextAttack);
                return;
            }
        }
        
        _gameState.Unit = null;
        _gameState.Lock = false;
        _signalBus.Fire(GameEvent.EndTurn);
        unit.OnMovementCompleted -= StopPlay;
    }

    public void Dispose()
    {
        _signalBus.Unsubscribe<GameEvent>(StartPlay);
    }
}
