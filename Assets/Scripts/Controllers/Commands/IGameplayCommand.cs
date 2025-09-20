using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGameplayCommand
{
    IEnumerable<Cell> Variants { get; }
    
    IEnumerable<Unit> Requires { get; }
    
    IDictionary<Cell, Unit> TargetUnits { get; }

    void Interact(Cell cell);

    bool CheckRequires(Cell cell = null);
}
