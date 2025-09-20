public enum Team
{
    None = 0,
    White,
    Black
}

public enum NeighbourType
{
    ForwardLeft,
    ForwardRight,
    BackwardLeft,
    BackwardRight,
}

public enum UnitType
{
    Common,
    Queen,
}

public enum GameEvent
{
    Select,
    Cancel,
    Confirm,
    EndTurn,
    StartTurn,
    NextAttack
}

public enum GameStatus
{
    Select,
    Attack,
    Confirm,
}

[System.Flags]
public enum ShowSelected
{
    None = 0,
    Unit = 1,
    Cell = 2,
    Variants = 4,
    Required = 8
}
