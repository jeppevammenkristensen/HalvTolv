namespace Infrastructure;

public enum RoundState
{
    Initial,
    InitialCardsAdded,
    PlayerTurnsFinished,
    RoundFinished
    
}

public class Round
{
    private RoundState _state = RoundState.Initial;
    
    private readonly Deck _deck;
    public int RoundNumber { get; private set; }
    
    public List<(Player, RoundCards)> PlayerCards = new();
    public List<(Player,RoundCards)> DealerMatches = new();

    public List<RoundMatch> Matches = new();
    
    public Round(Deck deck, List<Player> players, int number)
    {
        _deck = deck;
        RoundNumber = number;
        PlayerCards.AddRange(players.Select(x => (x,new RoundCards())));
    }

    public void AssignCardsToPlayers()
    {
        if (_state != RoundState.Initial)
        {
            throw new InvalidOperationException(
                $"Round must be in state Initial to assign card to players but was {_state}");
        }
        
        foreach (var (player, roundCards) in PlayerCards)
        {
            var card = _deck.Draw();
            roundCards.AddDrawnCard(card);
            Matches.Add(new RoundMatch(roundCards, new RoundCards(), _deck));
        }

        _state = RoundState.InitialCardsAdded;
    }

    public RoundMatch StartRound(int playerIndex)
    {
        var match =  Matches[playerIndex];
        if (match.State == RoundMatchState.Initial)
        {
            return match;
        }
        else
        {
            // throw error as match must be initial
            throw new InvalidOperationException(
                $"Match must be in the Initial state to start the round. Current state: {match.State}");
        }
    }

    public void RoundFinished()
    {
        if (_state != RoundState.PlayerTurnsFinished)
        {
            throw new InvalidOperationException($"Cannot close this round as it is incorrect state {_state}");
        }
        
        // Do some validation and throw an exception if 
        // some criterias are not met
        foreach (var (player, roundCards) in PlayerCards)
        {
            if (!true)
            {
                throw new InvalidOperationException($"Player {player.Number} has exceeded the score limit of 21.");
            }
        }
    }
}

public delegate Card GetCardDelegate();

public enum RoundMatchState
{
    Initial,
    PlayerFinished,
    DealerFinished
}

public delegate int RandomIntDelegate(int max);

public class RoundMatch
{
    private readonly Deck _deck;
    internal RandomIntDelegate RandomIntIntGenerator = i => Rand.Next(i);
    
    private static Random Rand = new();
    
    public bool? PlayerWon { get; private set; } = null;
    
    
    public RoundMatchState State { get; private set; } = RoundMatchState.Initial;
    public RoundCards Player { get; }
    public RoundCards Dealer { get; }

    public RoundMatch(RoundCards player, RoundCards dealer, Deck deck)
    {
        _deck = deck;
        Player = player;
        Dealer = dealer;
    }

    public Card AddCardPlayer()
    {
        if (Player.State != RoundCardsState.Playing)
        {
            // throw exception af state must be Playing
            throw new InvalidOperationException(
                $"Player must be in the Playing state to add a card. Current state: {State}");
        }
        
        var card = _deck.Draw();
        DealerKnowledge.Instance.AddKnownCard(card);
        Player.AddDrawnCard(card);

        if (Player.State == RoundCardsState.PlayerBust)
        {
            State = RoundMatchState.DealerFinished;
            Dealer.Stop();
            PlayerWon = false;
        }

        if (Player.State == RoundCardsState.PlayerHalvTolv)
        {
            State = RoundMatchState.PlayerFinished;
        }

        return card;
    }

    public void StopPLayer()
    {
        Player.Stop();
        State = RoundMatchState.PlayerFinished;
        if (Player.State == RoundCardsState.PlayerBust)
        {
            PlayerWon = false;
        }
    }

    public void DealerMakeMove()
    {
        if (State != RoundMatchState.PlayerFinished)
        {
            // throw saying InvalidOperationException
            throw new InvalidOperationException($"Player must finish their turn before the dealer can make a move. {State}");
        }

        if (Player.State == RoundCardsState.PlayerBust)
        {
            PlayerWon = false;
            return;
        }

        if (Player.State == RoundCardsState.PlayerHalvTolv)
        {
            TryReach(11.5m);
        }
        else switch (Player.Cards.Count)
        {
            case 1:
                TryReach(8);
                break;
            case > 1:
            {
                var knownCardsSum = Player.KnownCardsSum();
                if (knownCardsSum < 6)
                {
                    if (RandomIntIntGenerator(10) == 1)
                    {
                        TryReach(knownCardsSum + 0.5m);        
                    }
                
                    TryReach(8 + RandomIntIntGenerator(3));
                }

                decimal max = 11 - 0.5m - knownCardsSum;
                // generate a random that will round to nearest half. So 0.3 would be 0.5 0.2 would be 0 0.8 would be 1.0
                var randomDecimal = Rand.NextDouble();
                var randomHalf = Math.Round(randomDecimal * 2) / 2;
            
                TryReach(knownCardsSum + (max * Convert.ToDecimal(randomHalf)));
                break;
            }
        }

        State = RoundMatchState.DealerFinished;
    }

    public void EvaluateAndClose()
    {
        _deck.AddDrawnCardsAsUsedCards();
        
        // ensure that state is DealerFinished
        if (State != RoundMatchState.DealerFinished)
        {
            // throw saying InvalidOperationException
            throw new InvalidOperationException(
                $"Round match must be in the DealerFinished state to evaluate and close. Current state: {State}");
        }

        if (PlayerWon is { })
        {
            return;
        }

        if (Dealer.State == RoundCardsState.PlayerBust)
        {
            PlayerWon = true;
        }
        else if (Dealer.State == RoundCardsState.PlayerHalvTolv)
        {
            if (Player.State == RoundCardsState.PlayerHalvTolv)
            {
                PlayerWon = false;
            }
            else
            {
                PlayerWon = true;
            }
        }
        else if (Dealer.State == RoundCardsState.PlayerStopped)
        {
            if (Player.State == RoundCardsState.PlayerStopped)
            {
                if (Player.CurrentScore > Dealer.CurrentScore)
                {
                    PlayerWon = true;
                }
                else
                {
                    PlayerWon = false;
                }

                DealerKnowledge.Instance.AddKnownCard(Player.Cards[0]);
            }
            else
            {
                PlayerWon = false;
            }
        }
        else
        {
            // DealerState was unexpected throw error
            throw new InvalidOperationException($"Unexpected Dealer state {Dealer.State}");
        }
    }



    private void TryReach(decimal sum)
    {
        while (Dealer.State is RoundCardsState.Playing && Dealer.CurrentScore < sum)
        {
            var card = _deck.Draw();
            DealerKnowledge.Instance.AddKnownCard(card);
            Dealer.AddDrawnCard(card);
        }

        if (Dealer.State is RoundCardsState.Playing)
        {
            Dealer.Stop();
        }
    }
}