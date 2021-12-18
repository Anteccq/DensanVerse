using DensanVerse.Server.Models.Cards.Skills.Conditions;
using DensanVerse.Server.Models.Cards.Skills.EffectConditions;

namespace DensanVerse.Server.Models.Cards.Skills;

public class SkillEffect
{
    public SkillEffectKind Kind { get; set; }
    public TargetAmount Amount { get; set; }
}