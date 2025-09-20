using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GameState;
using ModestTree;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

public class TurnIndicator : MonoBehaviour
{
    private int _index;
    private IReadOnlyList<Team> _teams;
    
    [Inject]
    private SignalBus _signalBus;
    [Inject]
    private IGameState _gameState;
    
    [SerializeField]
    private TextMeshProUGUI turnIndicatorText;
    
    [SerializeField]
    private TextMeshProUGUI hintText;

    void Start()
    {
        _teams = FindObjectsByType<Unit>(FindObjectsInactive.Exclude, FindObjectsSortMode.None)
            .Select(x => x.Team).Distinct().OrderBy(x => x).ToList();
        _gameState.CurrentTeam = _teams[_index];
        turnIndicatorText.text = $"Turn {_gameState.CurrentTeam}";
        hintText.enabled = false;
        hintText.text = "Space for confirm, esc for cancel";
        
        _signalBus.Subscribe<GameEvent>(NextTurn);
    }

    private void NextTurn(GameEvent gameEvent)
    {
        if (_gameState.Lock) return;
        if (gameEvent == GameEvent.EndTurn)
        {
            _index = (_index + 1) % _teams.Count;
            Debug.Log($"Turn finishing {_index} {_gameState.CurrentTeam} {_teams[_index]}");
            _gameState.CurrentTeam = _teams[_index];
            turnIndicatorText.text = $"Turn {_gameState.CurrentTeam}";
            _signalBus.Fire(GameEvent.StartTurn);
        }

        hintText.enabled = _gameState.Cell && _gameState.Status is GameStatus.Confirm or GameStatus.Attack;
    }

    private void OnDestroy()
    {
        _signalBus.Unsubscribe<GameEvent>(NextTurn);
    }
}
