namespace DensanVerse.Server.Models.Cards.Effects;

public abstract class CardEffect
{

    protected internal abstract ValueTask<Card> EffectAsync(Card card);
}