namespace DensanVerse.Server.Models.Cards.Skills.Conditions;

public class TargetAmount
{
    public int SimpleAmount { get; set; }
    public AmountSource Source { get; set; }
    public TargetSource Value { get; set; }

    public int FixedValue { get; set; }
    public int Multiple { get; set; }
}