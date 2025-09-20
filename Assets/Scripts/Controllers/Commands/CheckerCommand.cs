using System.Collections.Generic;
using System.Linq;
using GameState;
using UnityEngine;

namespace Controllers.Commands
{
    public class CheckerCommand: IGameplayCommand
    {
        private readonly IGameState _gameState;
        
        private readonly HashSet<Cell> _cells = new HashSet<Cell>();
        
        private readonly HashSet<Unit> _requiredUnits = new HashSet<Unit>();

        private readonly IEnumerable<Unit> _units;
        
        private readonly Dictionary<Cell, Unit> _targetUnits = new Dictionary<Cell, Unit>();

        public IEnumerable<Cell> Variants => _cells;
        public IEnumerable<Unit> Requires => _requiredUnits;

        public IDictionary<Cell, Unit> TargetUnits => _targetUnits;

        public CheckerCommand(IGameState gameState, List<Unit> units)
        {
            _gameState = gameState;
            _units = units;
        }
        
        public void Interact(Cell cell)
        {
            if (_gameState.Status != GameStatus.Select && _gameState.Status != GameStatus.Attack) return;
            if (_gameState.Unit is not null && cell.Unit is null && _cells.Contains(cell))
            {
                _gameState.Cell = cell;
                return;
            }

            if (_gameState.Status == GameStatus.Attack && cell.Unit != _gameState.Unit) return;
            if (cell.Unit?.Team != _gameState.CurrentTeam) return;
            if (_requiredUnits.Count != 0 && !_requiredUnits.Contains(cell.Unit)) return;
            _gameState.Unit = cell.Unit;

            _requiredUnits.Clear();
            _cells.Clear();
            _targetUnits.Clear();
            if (CheckAttack(cell))
            {
                _requiredUnits.Add(cell.Unit);
                return;
            }

            AvailableCells(cell);
        }

        public void AvailableCells(Cell cell)
        {
            var availableDirection = _gameState.CurrentTeam == Team.White
                ? new HashSet<NeighbourType>() { NeighbourType.ForwardLeft, NeighbourType.ForwardRight }
                : new HashSet<NeighbourType>() { NeighbourType.BackwardRight, NeighbourType.BackwardLeft };
            
            foreach (var (direction, neighbour) in cell.Neighbours)
            {
                if (cell.Unit.Type != UnitType.Queen)
                {
                    if (!neighbour.Unit &&
                        availableDirection.Contains(direction))
                    {
                        _cells.Add(neighbour);
                    }
                }
                else
                {
                    var checkCell = neighbour;
                    while (!checkCell.Unit)
                    {
                        _cells.Add(checkCell);
                        if (!checkCell.Neighbours.TryGetValue(direction, out checkCell)) break;
                    }
                }
            }
        }

        public bool CheckRequires(Cell cell = null)
        {
            _requiredUnits.Clear();
            _cells.Clear();
            _targetUnits.Clear();
            var result = false;
            if (cell == null)
            {
                foreach (var unit in _units.Where(x => x))
                {
                    if (CheckAttack(unit.Cell))
                    {
                        _requiredUnits.Add(unit);
                        result = true;
                    }
                }
            }
            else
            {
                if (CheckAttack(cell))
                {
                    _requiredUnits.Add(cell.Unit);
                    result = true;
                }
            }

            return result;
        }

        public bool CheckAttack(Cell cell)
        {
            if (cell.Unit is null) return false;
            if (cell.Unit.Team != _gameState.CurrentTeam) return false;
            var result = false;
            foreach (var (direction, neighbour) in cell.Neighbours)
            {
                if (cell.Unit.Type != UnitType.Queen)
                {
                    if (neighbour.Unit && 
                        neighbour.Unit.Team != _gameState.CurrentTeam &&
                        neighbour.Neighbours.TryGetValue(direction, out var nextCell) &&
                        !nextCell.Unit)
                    {
                        _cells.Add(nextCell);
                        _targetUnits.Add(nextCell, neighbour.Unit);
                        result = true;
                    }
                }
                else
                {
                    var checkCell = neighbour;
                    while (true)
                    {
                        if (checkCell.Unit?.Team == _gameState.CurrentTeam) break;
                        if (checkCell.Unit && checkCell.Unit.Team != _gameState.CurrentTeam &&
                            checkCell.Neighbours.TryGetValue(direction, out var nextCell) &&
                            !nextCell.Unit)
                        {
                            _cells.Add(nextCell);
                            _targetUnits.Add(nextCell, checkCell.Unit);
                            result = true;
                            break;
                        }
                        if (!checkCell.Neighbours.TryGetValue(direction, out checkCell)) break;
                    }
                }
            }

            return result;
        }
    }
}