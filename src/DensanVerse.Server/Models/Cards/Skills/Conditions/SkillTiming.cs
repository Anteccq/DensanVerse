namespace DensanVerse.Server.Models.Cards.Skills.Conditions;

public enum SkillTiming
{
    OnPlayerTurn,
    AfterPlayerTurn,
    OnEnemyTurn,
    AfterEnemyTurn,

    OnTargetJoinClub,
    AfterTargetJoinClub,

    OnTargetRetireClub,
    AfterTargetRetireClub,

    OnPlayerAttack,
    AfterPlayerAttack,

    OnEnemyAttack,
    AfterEnemyAttack,

    OnSelfJoinClub,
    OnSelfRetireClub,
}