using System;
using FluentAssertions;
using Infrastructure;
using JetBrains.Annotations;
using Xunit;

namespace Infrastructure.Tests;

[TestSubject(typeof(RoundCards))]
public class RoundCardsTest
{
    private Random _random = new Random();

    [Theory]
    [InlineData(SuitCard.Ace, Suit.Spades, 11)]
    [InlineData(SuitCard.Ace, Suit.Hearts, 11)]
    [InlineData(SuitCard.Jack, Suit.Hearts, 0.5)]
    [InlineData(SuitCard.Queen, Suit.Clubs, 0.5)]
    [InlineData(SuitCard.King, Suit.Diamonds, 0.5)]
    [InlineData(SuitCard.Two, Suit.Clubs, 2)]
    [InlineData(SuitCard.Three, Suit.Diamonds, 3)]
    [InlineData(SuitCard.Four, Suit.Hearts, 4)]
    [InlineData(SuitCard.Five, Suit.Hearts, 5)]
    [InlineData(SuitCard.Six, Suit.Clubs, 6)]
    [InlineData(SuitCard.Seven, Suit.Diamonds, 7)]
    [InlineData(SuitCard.Eight, Suit.Diamonds, 8)]
    [InlineData(SuitCard.Nine, Suit.Diamonds, 9)]
    [InlineData(SuitCard.Ten, Suit.Spades, 10)]
    public void CalculateScore_SingleCard(SuitCard suitCard,Suit suit, Decimal expectedValue)
    {
        var roundOfCards = new RoundCards();
        roundOfCards.AddDrawnCard(new Card(suit,suitCard));
        roundOfCards.CalculateScore().Should().Be(expectedValue);
    }

    [Theory]
    [InlineData(SuitCard.Ace, SuitCard.Jack, 11.5)]
    [InlineData(SuitCard.Ace, SuitCard.Queen, 11.5)]
    [InlineData(SuitCard.Ace, SuitCard.King, 11.5)]
    [InlineData(SuitCard.Jack, SuitCard.Ace, 11.5)]
    [InlineData(SuitCard.Queen, SuitCard.Ace, 11.5)]
    [InlineData(SuitCard.King, SuitCard.Ace, 11.5)]
    [InlineData(SuitCard.Ace, SuitCard.Ace, 2)]
    [InlineData(SuitCard.Ace, SuitCard.Two, 3)]
    [InlineData(SuitCard.Three, SuitCard.Ace, 4)]
    [InlineData(SuitCard.Four, SuitCard.Ace, 5)]
    [InlineData(SuitCard.Ace, SuitCard.Five, 6)]
    [InlineData(SuitCard.Ace, SuitCard.Six, 7)]
    [InlineData(SuitCard.Ace, SuitCard.Seven, 8)]
    [InlineData(SuitCard.Eight, SuitCard.Ace, 9)]
    [InlineData(SuitCard.Nine, SuitCard.Seven, 16)]
    [InlineData(SuitCard.Ace, SuitCard.Ten, 11)]
    [InlineData(SuitCard.Seven, SuitCard.Jack, 7.5)]
    [InlineData(SuitCard.Three, SuitCard.Queen, 3.5)]
    [InlineData(SuitCard.Eight, SuitCard.King, 8.5)]

    public void CalcuateScore_TwoCards(SuitCard first, SuitCard second, Decimal expectedValue)
    {
        var roundOfCards = new RoundCards();
        roundOfCards.AddDrawnCard(new Card(Suit.Clubs,first));
        roundOfCards.AddDrawnCard(new Card(Suit.Hearts, second));

        roundOfCards.CalculateScore().Should().Be(expectedValue);
    }
}