using DensanVerse.Server.Models.Field;

namespace DensanVerse.Server.Models.Battle;

public class BattleContext
{
    public string BattleId { get; set; }
    public FieldContext TeamRebelFieldContext { get; set; }
    public FieldContext TeamOwnerFieldContext { get; set; }
    public int TurnCount { get; set; }
}