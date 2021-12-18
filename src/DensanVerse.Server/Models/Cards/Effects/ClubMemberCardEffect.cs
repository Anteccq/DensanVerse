namespace DensanVerse.Server.Models.Cards.Effects;

public abstract class ClubMemberCardEffect : CardEffect
{
    protected ClubMemberCardEffect(CardEffect innerEffect)
    {
        InnerEffect = innerEffect;
    }
    public CardEffect? InnerEffect { get; set; }
    protected internal sealed override ValueTask<Card> EffectAsync(Card card)
    {
        if (card is ClubMemberCard clubMember) return EffectAsync(clubMember);

        throw new InvalidOperationException("This card must be ClubMember type.");
    }

    protected internal abstract ValueTask<Card> EffectAsync(ClubMemberCard card);
}