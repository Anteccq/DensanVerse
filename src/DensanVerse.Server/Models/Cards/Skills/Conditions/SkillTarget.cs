namespace DensanVerse.Server.Models.Cards.Skills.Conditions;

public class SkillTarget
{
    public TargetKind Kind { get; set; }

    public TargetSource Source { get; set; }

    public TargetComparison Comparison { get; set; }

    public TargetAmount Amount { get; set; }

    public bool IsSingleTarget { get; set; }
}