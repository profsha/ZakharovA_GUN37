using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Controllers.Commands;
using GameState;
using UnityEngine;
using Zenject;

public class Battlefield: System.IDisposable
{
    private readonly IGameState _gameState;
    private readonly SignalBus _signalBus;
    private readonly CellPalleteSettings _settings;
    private readonly IGameplayCommand _command;
    private readonly IEnumerable<Unit> _units;

    private readonly Dictionary<(float x, float z), Cell> _cells;
    
    public Battlefield(SignalBus signalBus,
        IGameState gameState,
        CellPalleteSettings settings,
        IGameplayCommand command,
        List<Unit> units)
    {   
        _gameState = gameState;
        _signalBus = signalBus;
        _settings = settings;
        _command = command;
        _units = units;
        _cells = Object.FindObjectsByType<Cell>(FindObjectsInactive.Include, FindObjectsSortMode.None)
            .ToDictionary(x => (x.transform.position.x, x.transform.position.z));

        var dictUnits = _units.ToDictionary(x => (x.transform.position.x, x.transform.position.z));

        foreach (var (position, cell) in _cells)
        {
            cell.Neighbours = new Dictionary<NeighbourType, Cell>();

            foreach (NeighbourType neighbourType in System.Enum.GetValues(typeof(NeighbourType)))
            {
                Cell neighbourCell = null; 
                switch (neighbourType)
                {
                    case NeighbourType.ForwardLeft:
                        _cells.TryGetValue((position.x + 1, position.z - 1), out neighbourCell);
                        break;
                    case NeighbourType.ForwardRight:
                        _cells.TryGetValue((position.x - 1, position.z - 1), out neighbourCell);
                        break;
                    case NeighbourType.BackwardLeft:
                        _cells.TryGetValue((position.x + 1, position.z + 1), out neighbourCell);
                        break;
                    case NeighbourType.BackwardRight:
                        _cells.TryGetValue((position.x - 1, position.z + 1), out neighbourCell);
                        break;
                }

                if (neighbourCell != null)
                {
                    cell.Neighbours.Add(neighbourType, neighbourCell);
                }
            }
            
            cell.OnCellClicked += HandleClick;

            if (dictUnits.TryGetValue(position, out var unit))
            {
                cell.Unit = unit;
                if (unit != null)
                {
                    unit.Cell = cell;
                    unit.OnMovementStarted += (x) => Debug.Log($"MovementStarted {x.name}");
                    unit.OnMovementCompleted += (x) => Debug.Log($"MovementCompleted {x.name}");
                }
            }
        }
        
        _signalBus.Subscribe<GameEvent>(Callback);
    }

    private void ClearSelection()
    {
        foreach (var (_, cell) in _cells)
        {
            cell.SetSelected(false);
        }
    }

    private void ShowVariants()
    {
        foreach (var cell in _command.Variants)
        {
            var attack = _command.TargetUnits.ContainsKey(cell);
            cell.SetSelected(true, attack ? _settings.Attack : _settings.Move);
        }
    }

    private void ShowRequireUnits()
    {
        foreach (var unit in _command.Requires) 
        {
            unit.Cell.SetSelected(true, _settings.MoveAndAttack);
        }
    }
    
    private void Callback(GameEvent gameEvent)
    {
        if (_gameState.Lock) return;
        ClearSelection();
        
        ShowSelected showSelected = ShowSelected.Unit | ShowSelected.Cell;
        switch (_gameState, gameEvent)
        {
            case ({Status: GameStatus.Select or GameStatus.Attack, Cell: not null, Unit: not null}, GameEvent.Select):
                _gameState.Status = _gameState.Status == GameStatus.Attack ? GameStatus.Attack : GameStatus.Confirm;
                showSelected = ShowSelected.Unit | ShowSelected.Cell;
                break;
            case ({Status: GameStatus.Select, Unit: not null }, GameEvent.Select):
                showSelected = ShowSelected.Unit | ShowSelected.Variants | ShowSelected.Required;
                break;
            case ({Status: GameStatus.Select}, GameEvent.Select):
                _command.CheckRequires();
                showSelected = ShowSelected.Required;
                break;
            case ({Status: GameStatus.Attack, Unit: not null}, GameEvent.Select):
                showSelected = ShowSelected.Unit | ShowSelected.Variants;
                break;
            case ({Status: GameStatus.Select}, GameEvent.Cancel):
                _command.CheckRequires();
                _gameState.Unit = null;
                showSelected = ShowSelected.Required;
                break;
            case ({Status: GameStatus.Confirm or GameStatus.Attack}, GameEvent.Cancel):
                _gameState.Cell = null;
                _command.Interact(_gameState.Unit?.Cell);
                _gameState.Status =  _gameState.Status == GameStatus.Attack ? GameStatus.Attack : GameStatus.Select;
                showSelected = ShowSelected.Variants;
                break;
            case (not null, GameEvent.StartTurn):
                _command.CheckRequires();
                _gameState.Status = GameStatus.Select;
                showSelected = ShowSelected.Required;
                break;
            case (not null, GameEvent.NextAttack):
                showSelected = ShowSelected.Unit | ShowSelected.Variants;
                break;
        }

        if (showSelected.HasFlag(ShowSelected.Variants))
        {
            ShowVariants();
        }

        if (showSelected.HasFlag(ShowSelected.Required))
        {
            ShowRequireUnits();
        }

        if (showSelected.HasFlag(ShowSelected.Cell))
        {
            _gameState.Cell?.SetSelected(true, _settings.Move);
        }

        if (showSelected.HasFlag(ShowSelected.Unit))
        {
            _gameState.Unit?.Cell.SetSelected(true, _settings.Selected);
        }
    }

    private void HandleClick(Cell cell)
    {
        if (_gameState.Lock) return;
        _command.Interact(cell);
        _signalBus.Fire(GameEvent.Select);
    }

    public void Dispose()
    {
        foreach (var (_, cell) in _cells)
        {
            cell.OnCellClicked -= HandleClick;
        }
    }
}
