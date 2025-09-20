using UnityEngine;

[CreateAssetMenu(fileName = "New CellPallete", menuName = "CellPallete", order = 51)]
public class CellPalleteSettings : ScriptableObject
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
