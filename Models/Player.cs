namespace ChessGame.Models
{
    public enum Player
    {
        Black,
        White,
        None,
    }

    public static class PlayerUtils
    {
        public static Player Opponent(this Player player)
        {
            switch (player)
            {
                case Player.Black:
                    return Player.White;
                case Player.White:
                    return Player.Black;
                default:
                    return Player.None;
            }
        }
    }
}
