using InvalidOperationException = System.InvalidOperationException;

namespace Infrastructure;

public enum RoundCardsState
{
    Playing,
    PlayerStopped,
    PlayerBust,
    PlayerHalvTolv
}

public class RoundCards
{
    public decimal CurrentScore { get; private set; } = 0m;
    public List<Card> Cards { get; } = new();
    public RoundCardsState State { get; private set;} = RoundCardsState.Playing;
    

    public decimal CalculateScore()
    {
        if (Cards is {Count: 0})
        {
            return 0;
        }

        if (Cards is [{SuitCard: SuitCard.Ace}])
        {
            return 11;
        }

        if (Cards is [{} firstCard, {} secondCard])
        {
            if (firstCard.SuitCard is SuitCard.Ace || secondCard.SuitCard is SuitCard.Ace)
            {
                if (firstCard.IsHalf() || secondCard.IsHalf())
                {
                    return 11.5m;
                }
            }
        }

        var sum = Cards.Sum(SumCard);
        if (Cards.Count == 5 && sum <= 11.5m)
        {
            return 11.5m;
        }

        return sum;
    }

    private static decimal SumCard(Card x)
    {
        if (x.SuitCard is SuitCard.Ace)
        {
            return 1m;
        }
        else if (x.IsHalf())
        {
            return 0.5m;
        }
        else
        {
            return (decimal) x.SuitCard;
        }
    }

    public decimal KnownCardsSum()
    {
        return Cards.Skip(1).Sum(SumCard);
    }
  

    public RoundCards()
    {
        
    }

    public void AddDrawnCard(Card card)
    {
        if (State != RoundCardsState.Playing)
        {
            throw new InvalidOperationException($"Can only draw new card when state is Playing but was {State}");
        }
        
        Cards.Add(card);
        CurrentScore = CalculateScore();
        if (CurrentScore > 11.5m)
        {
            State = RoundCardsState.PlayerBust;
        }
        else if (CurrentScore == 11.5m)
        {
            State = RoundCardsState.PlayerHalvTolv;
        }
    }

    public void Stop()
    {
        if (State != RoundCardsState.Playing)
        {
            throw new InvalidOperationException(
                $"Can only stop when playing and user is not bust or reached HalvTolv. State was {State}");
        }

        State = RoundCardsState.PlayerStopped;
    }
}