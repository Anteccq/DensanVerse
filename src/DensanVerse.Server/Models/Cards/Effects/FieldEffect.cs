using DensanVerse.Server.Models.Cards.Skills;
using DensanVerse.Server.Models.Cards.Skills.Conditions;

namespace DensanVerse.Server.Models.Cards.Effects
{
    public class FieldEffect
    {
        public SkillTarget[] EffectTargets { get; set; }

        public SkillEffect[] SkillEffects { get; set; }
    }
}
