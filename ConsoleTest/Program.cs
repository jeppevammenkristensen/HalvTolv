// See https://aka.ms/new-console-template for more information

using ConsoleTest;
using Infrastructure;

ConsoleGame game = new ConsoleGame();
game.Start();

//
// while (deck.CanDraw)
// {
//     var draw = deck.Draw();
//     Console.WriteLine($"{draw.Suit} {draw.SuitCard}");
// }