namespace Infrastructure;

public class Player
{
    public int Number { get; }

    public Player(int number)
    {
        Number = number;
    }
    
    public bool IsDealer { get; set; }
}