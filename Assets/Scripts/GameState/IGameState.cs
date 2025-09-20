namespace GameState
{
    public interface IGameState
    {
        Cell Cell { get; set; }
        bool Lock { get; set; }
        GameStatus Status { get; set; }
        Unit Unit { get; set; }
        Team CurrentTeam { get; set; }
    }
}