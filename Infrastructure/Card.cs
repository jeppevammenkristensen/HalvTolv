namespace Infrastructure;

public class DealerKnowledge
{
    private List<Card> _currentMatchCards = [];
    
    
    private List<Card> _knownUsedCards = [];
    private int _unknownUsedCardsCount = 0;
    
    public void DeckReused()
    {
        _knownUsedCards.Clear();  
    } 
    
    public DealerKnowledge()
    {
            
    }

    public void AddKnownCard(Card card)
    {
        _currentMatchCards.Add(card);
    }

    public void FinishMatch(Card? playerCard)
    {
        _knownUsedCards.AddRange(_currentMatchCards);
        _currentMatchCards.Clear();
        if (playerCard is { } card)
        {
            _knownUsedCards.Add(card);
        }
        else
        {
            _unknownUsedCardsCount++;
        }
    }

    public void Reset()
    {
        _knownUsedCards.Clear();
        _currentMatchCards.Clear();
        _unknownUsedCardsCount = 0;
    }
    
    // Make this Singleton
    private static DealerKnowledge? _instance;
    private static readonly object _lock = new object();

    public static DealerKnowledge Instance
    {
        get
        {
            lock (_lock)
            {
                if (_instance == null)
                {
                    _instance = new DealerKnowledge();
                }

                return _instance;
            }
        }
    }
}

public record Card(Suit Suit, SuitCard SuitCard)
{
    public bool IsHalf()
    {
        return SuitCard is SuitCard.Jack or SuitCard.Queen or SuitCard.King;
    }
}