using AutoSpectre;
using Infrastructure;
using Spectre.Console;

namespace ConsoleTest;

[AutoSpectreForm]
public class PlayerDeal
{
    private readonly RoundMatch _round;
    public readonly bool CanDraw;
    
    
    public PlayerDeal(RoundMatch round)
    {
        _round = round;
        CanDraw = _round.State == RoundMatchState.Initial; }
    
    
    [TextPrompt(Title = "Draw card?", Condition = nameof(CanDraw) )]
    public bool CanDrawCard { get; set; }

    [TaskStep(Condition = nameof(CanDrawCard))]
    public void DrawCard(IAnsiConsole console)
    {
        var card = _round.AddCardPlayer();
        console.MarkupLineInterpolated($"Drew [white]{card.Suit} {card.SuitCard}[/]");
    }
    
    [TaskStep(Condition = nameof(CanDrawCard), NegateCondition = true)]
    public void Stop(IAnsiConsole console)
    {
        _round.StopPLayer();
        console.MarkupLineInterpolated($"Stopped");
    }

    [TaskStep]
    public void Status(IAnsiConsole console)
    {
        if (_round.Player.State is RoundCardsState.Playing or RoundCardsState.PlayerStopped)
        {
            console.MarkupLineInterpolated($"Current score: {_round.Player.CurrentScore}");
        }
        else if (_round.Player.State is RoundCardsState.PlayerBust)
        {
            console.MarkupLineInterpolated($"Player bust. Score: {_round.Player.CurrentScore}");
        }
        else if (_round.Player.State == RoundCardsState.PlayerHalvTolv)
        {
            console.MarkupLineInterpolated($"Player has Halv tolv");
        }
    }
}