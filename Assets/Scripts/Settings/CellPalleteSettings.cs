using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellPalleteSetings : ScriptableObject
{
    [field: SerializeField]
    public Material Selected { get; private set; }
    [field: SerializeField]
    public Material Move { get; private set; }
    [field: SerializeField]
    public Material Attack { get; private set; }
    [field: SerializeField]
    public Material MoveAndAttack { get; private set; }
}
