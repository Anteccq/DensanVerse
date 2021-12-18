using DensanVerse.Server.Models.Cards.Effects;
using DensanVerse.Server.Models.Cards.Skills.Conditions;

namespace DensanVerse.Server.Models.Cards.Skills;

public class Skill
{
    public int Id { get; set; }
    public SkillTiming Condition { get; set; }

    public SkillTarget[] ConditionTargets { get; set; }

    public FieldEffect[] Effects { get; set; }
}