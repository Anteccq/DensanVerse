namespace DensanVerse.Server.Models.Cards;

public class ClubMemberCard : Card
{
    public int Qpa { get; set; }
    public int Energy { get; set; }

    public ClubMemberCard()
    {
        Type = CardType.ClubMember;
    }
    public override void Play()
    {
        base.Play();
    }
}