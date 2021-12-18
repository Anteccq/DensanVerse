using DensanVerse.Server.Models.Cards.Skills;

namespace DensanVerse.Server.Models.Cards;

public abstract class Card
{
    public int Id { get; init; }
    public string FieldId { get; set; }
    public string Name { get; init; }
    public CardClass Class { get; set; }

    public CardType Type { get; set; }

    public bool IsAuthority { get; set; }
    public bool IsForbidden { get; set; }

    public int Cost { get; set; }
    public int ClubBudget { get; set; }
    public string FlavorText { get; set; }
    public string DescriptionText { get; set; }

    public List<Skill> Skills { get; } = new();

    public virtual void Play()
    {

    }
}

public enum CardClass
{
    Programming,
    DesktopMusic,
    ComputerGraphic,
    Central,

    //The Bloody GOD of Tyranny Cards
    Absolute
}

public enum CardType
{
    ClubMember,
    Environment,
    Bible
}