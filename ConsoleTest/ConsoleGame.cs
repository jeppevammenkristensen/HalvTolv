using Infrastructure;
using Spectre.Console;

namespace ConsoleTest;

public class ConsoleGame
{
    private readonly IAnsiConsole _console;
    private readonly Deck _deck;
    private Player _player;
    private List<Round> _rounds = new();

    public ConsoleGame() : this(AnsiConsole.Console)
    {
        
    }
    
    public ConsoleGame(IAnsiConsole console)
    {
        _console = console;
        _deck = new Deck();
    }
    
    public void Start()
    {
        _console.Write(new FigletText("Halv tolv"));
        _console.MarkupLine($"[green bold]Starting game with 1 player[/]");

        _player = new Player(1);

        while (!_deck.AllCardsDealt)
        {
            PlayRound();
        }
        
        _console.MarkupLine("[green]Game completed[/]");
        _console.Prompt<bool>(new ConfirmationPrompt("Ok?"));
    }

    private void PlayRound()
    {
        var round = new Round(_deck, [_player], _rounds.Count + 1);
        _console.MarkupLineInterpolated($"[green]Starting round {round.RoundNumber} Cards left: {_deck.CardCount}[/]");

        round.AssignCardsToPlayers();

        var playerRound = round.StartRound(0);

        _console.MarkupLineInterpolated($"[yellow]Player {_player.Number} assigned: {playerRound.Player.Cards[0].Suit} {playerRound.Player.Cards[0].SuitCard}[/]");

        PlayerDealCards(playerRound);

        if (playerRound.Player.State == RoundCardsState.PlayerBust)
        {
            _console.WriteLine("Player was bust. Winner is Dealer");
        }
        else
        {
            playerRound.DealerMakeMove();
            foreach (var card in playerRound.Dealer.Cards)
            {
                _console.MarkupLineInterpolated($"[blue]Dealer drew: {card.Suit} {card.SuitCard}[/]");
            } 
            
            playerRound.EvaluateAndClose();

            if (playerRound.Dealer.State == RoundCardsState.PlayerHalvTolv)
            {
                _console.MarkupLine("Dealer got halv tolv");
            }
            else if (playerRound.Dealer.State == RoundCardsState.PlayerStopped)
            {
                _console.WriteLine("Dealer stopped");
            }
            else if (playerRound.Dealer.State == RoundCardsState.PlayerBust)
            {
                _console.WriteLine("Dealer bust");
            }

            if (playerRound.PlayerWon == true)
            {
                _console.MarkupLineInterpolated(
                    $"[green bold]Player won. Got {playerRound.Player.CurrentScore} Dealer got {playerRound.Dealer.CurrentScore} [/]");
            }
            else
            {
                _console.MarkupLineInterpolated(
                    $"[green bold]Dealer won. Got {playerRound.Dealer.CurrentScore} Player got {playerRound.Player.CurrentScore} [/]");
            }
        }

        _rounds.Add(round);
    }

    private void PlayerDealCards(RoundMatch playerRound)
    {
        while (playerRound.Player.State == RoundCardsState.Playing)
        {
            var playerDeal = new PlayerDeal(playerRound).SpectrePrompt();
        }
    }
}