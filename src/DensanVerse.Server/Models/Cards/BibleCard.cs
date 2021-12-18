namespace DensanVerse.Server.Models.Cards;

public class BibleCard : Card
{
    public bool HasDeadLine { get; set; }
    public int DeadLine { get; set; }

    public BibleCard()
    {
        Type = CardType.Bible;
    }

    public override void Play()
    {
        base.Play();
    }
}