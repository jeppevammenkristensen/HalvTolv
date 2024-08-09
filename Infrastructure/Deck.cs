namespace Infrastructure;

public class Deck
{
    public bool AllCardsDealt { get; private set; }
    
    private Stack<Card> _drawnCardsStack = new();
    private List<Card> _usedCards = new();
    
    private List<Card> _cards = new();
    private Random _rand = new();

    public int CardCount => _cards.Count;
    
    public Deck()
    {
        foreach (var suit in Enum.GetValues<Suit>())
        {
            foreach (var suitCard in Enum.GetValues<SuitCard>())
            {
                _cards.Add(new Card(suit,suitCard));
            }
        }

        Shuffle();
    }

    private void Shuffle() 
    {
        if (_drawnCardsStack.Count > 0)
        {
            while (_drawnCardsStack.TryPeek(out var currentCard))
            {
                _cards.Add(currentCard);
            }
        }
        
        int n = _cards.Count;
        for (int i = n - 1; i > 0; i--)
        {
            int j = _rand.Next(i + 1);
            // Swap the two positions
            (_cards[i], _cards[j]) = (_cards[j], _cards[i]);
        }
    }

    public bool CanDraw => _cards is {Count: > 0};

    /// <summary>
    /// Draws a card. Remember to call the <see cref="AddUsedCards"/>
    /// to add it the drawnCardsStack. This will be used if we need to reshuffle to get
    /// cards
    /// </summary>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public Card Draw()
    {
        var firstItem = _cards[0];
        _cards.RemoveAt(0);
        _drawnCardsStack.Push(firstItem);

        if (_cards.Count == 0)
        {
            AllCardsDealt = true;
            _cards.AddRange(_usedCards);
            _usedCards.Clear();
        }
        
        return firstItem;
    }

    public void AddDrawnCardsAsUsedCards()
    {
        while (_drawnCardsStack.Count > 0)
        {
            _usedCards.Add(_drawnCardsStack.Pop());
        }
    }
}