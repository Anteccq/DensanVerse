using DensanVerse.Server.Models.Cards.Skills.Conditions;
using DensanVerse.Server.Models.Field;

namespace DensanVerse.Server.Models.Cards.Effects;

public record TargetContext(FieldContext PlayerContext, FieldContext EnemyContext,
    string SelfCardFieldId, string SelectedCardFieldId);