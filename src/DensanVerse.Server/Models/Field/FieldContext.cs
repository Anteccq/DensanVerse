using DensanVerse.Server.Models.Cards;
using DensanVerse.Server.Models.Players;

namespace DensanVerse.Server.Models.Field;

public class FieldContext
{
    public Player Player { get; set; } 
    public int PlayerHealthPoint { get; set; }
    public int ClubBudget { get; set; }
    public PlayerClass PlayerClass { get; set; }
    public List<Card> CardDeck { get; set; }
    public List<Card> HandCards { get; set; }
    public int EnergyDrinkAmount { get; set; }
    public ClubMemberCard?[] JoinedClubMemberCards { get; set; }
    public EnvironmentCard?[] EnvironmentCards { get; set; }
    public List<Card> ObOgCards { get; set; }

    public List<ClubMemberCard> GetLiveJoinedClubMemberCards()
        => JoinedClubMemberCards.Where(x => x is not null).ToList()!;

    public List<EnvironmentCard> GetLiveEnvironmentCards()
        => EnvironmentCards.Where(x => x is not null).ToList()!;
}