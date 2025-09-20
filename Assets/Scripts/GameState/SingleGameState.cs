namespace GameState
{
    public class SingleGameState: IGameState
    {
        public Cell Cell { get; set; }
        public bool Lock { get; set; }
        public GameStatus Status { get; set; }
        public Unit Unit { get; set; }
        public Team CurrentTeam { get; set; }
    }
}