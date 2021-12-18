using System.Security.Cryptography;
using DensanVerse.Server.Models.Cards;
using DensanVerse.Server.Models.Cards.Effects;
using DensanVerse.Server.Models.Cards.Skills;
using DensanVerse.Server.Models.Cards.Skills.Conditions;
using DensanVerse.Server.Models.Cards.Skills.EffectConditions;
using DensanVerse.Server.Models.Field;

namespace DensanVerse.Server.Services
{
    public class FieldEffectService
    {
        private readonly Random _rand;
        public FieldEffectService()
        {
            _rand = new Random();
        }

        private void ExecuteEffect(FieldEffect effect, FieldContext playerContext, FieldContext enemyContext, string selfCardFieldId, string selectedCardFieldId)
        {
            var context = new TargetContext(playerContext, enemyContext, selfCardFieldId, selectedCardFieldId);

            //Leader に対するものはいずれかの EffectTarget で効果を発揮した時点で終了。
            var effectLeaders = 
                effect.EffectTargets
                    .Where(x => IsKindLeader(x.Kind))
                    .GroupBy(x => x.Kind)
                    .ToDictionary(x => x.Key, x =>x.ToArray());

            //Card に対するものは全ての EffectTarget を探索するが、効果を発揮した number をチェックし、二重に効果が発揮されないようにする。
            var effectCards = effect.EffectTargets
                .Where(x => !IsKindLeader(x.Kind))
                .ToList();

            foreach (var leaderEffect in effectLeaders)
            {
                _ = leaderEffect.Value
                    .First(x =>
                        TryEffectsLeader(x, context, leaderEffect.Key is TargetKind.PlayerLeader, effect.SkillEffects));
            }

            var exceptIds = new HashSet<string>();
            foreach (var effectCard in effectCards)
            {
                if (!TryEffectTargetCards(effectCard, effect.SkillEffects, context,
                        exceptIds, out var effectedCardFieldIds))
                    continue;

                exceptIds.UnionWith(effectedCardFieldIds);
            }
        }

        private static bool IsKindLeader(TargetKind kind)
            => kind is TargetKind.PlayerLeader or TargetKind.EnemyLeader;

        // 誰にエフェクトするのか
        private bool TryEffectTargetCards(SkillTarget target, SkillEffect[] effects, TargetContext context,
            IReadOnlySet<string> exceptFieldCardIds, out string[] effectedCardFieldId)
        {
            var cards = ExtractTargetCards(target, context,
                x =>
                    CreateCardFilter(context, target.Kind)
                        .Where(cmc => !exceptFieldCardIds.Contains(cmc.FieldId)));
            if (cards.Count == 0)
            {
                effectedCardFieldId = Array.Empty<string>();
                return false;
            }
            EffectCards(target, cards, context, effects);
            effectedCardFieldId = cards.Select(x => x.FieldId).ToArray();
            return true;
        }

        private IEnumerable<ClubMemberCard> CreateCardFilter(TargetContext context, TargetKind kind)
            => kind switch
            {
                TargetKind.PlayerCards => context.PlayerContext.GetLiveJoinedClubMemberCards(),
                TargetKind.EnemyCards => context.EnemyContext.GetLiveJoinedClubMemberCards(),

                TargetKind.RandomPlayerCard => new List<ClubMemberCard>{GetRandomTargetCard(context.PlayerContext)},
                TargetKind.RandomEnemyCard => new List<ClubMemberCard>{GetRandomTargetCard(context.EnemyContext)},

                TargetKind.PgPlayerCards => context.PlayerContext.GetLiveJoinedClubMemberCards()
                        .Where(cmc => cmc.Class == CardClass.Programming),
                TargetKind.PgEnemyCards => context.EnemyContext.GetLiveJoinedClubMemberCards()
                        .Where(cmc => cmc.Class == CardClass.Programming),

                TargetKind.DtmPlayerCards => context.PlayerContext.GetLiveJoinedClubMemberCards()
                        .Where(cmc => cmc.Class == CardClass.DesktopMusic),
                TargetKind.DtmEnemyCards => context.EnemyContext.GetLiveJoinedClubMemberCards()
                        .Where(cmc => cmc.Class == CardClass.DesktopMusic),

                TargetKind.CgPlayerCards => context.PlayerContext.GetLiveJoinedClubMemberCards()
                        .Where(cmc => cmc.Class == CardClass.ComputerGraphic),
                TargetKind.CgEnemyCards => context.EnemyContext.GetLiveJoinedClubMemberCards()
                        .Where(cmc => cmc.Class == CardClass.ComputerGraphic),

                TargetKind.CentralPlayerCards => context.PlayerContext.GetLiveJoinedClubMemberCards()
                        .Where(cmc => cmc.Class == CardClass.Central),
                TargetKind.CentralEnemyCards => context.EnemyContext.GetLiveJoinedClubMemberCards()
                        .Where(cmc => cmc.Class == CardClass.Central),

                TargetKind.PlayerObOg => context.PlayerContext.ObOgCards.OfType<ClubMemberCard>(),
                TargetKind.EnemyObOg => context.EnemyContext.ObOgCards.OfType<ClubMemberCard>(),

                TargetKind.PlayerSelectCard => 
                    context.PlayerContext.GetLiveJoinedClubMemberCards()
                        .Concat(context.EnemyContext.GetLiveJoinedClubMemberCards())
                        .Where(cmc => cmc.FieldId == context.SelectedCardFieldId),

                _ => throw new ArgumentOutOfRangeException(nameof(kind), kind, null)
            };

        private ClubMemberCard GetRandomTargetCard(FieldContext context)
        {
            var fieldMember = context.GetLiveJoinedClubMemberCards();
            return fieldMember[_rand.Next(fieldMember.Count)];
        }

        private IList<ClubMemberCard> ExtractTargetCards(SkillTarget target,
            TargetContext context, Func<TargetContext, IEnumerable<ClubMemberCard>> filterFunc)
        {
            var filteredCards = filterFunc(context).ToArray();
            var result = new List<ClubMemberCard>();
            foreach (var card in filteredCards)
            {
                var leftValue = GetCardAmount(target.Source, card);
                var rightValue = GetAmount(target.Amount, context);
                if(IsTarget(target.Comparison, leftValue, rightValue))
                    result.Add(card);
            }

            return result;
        }

        private void EffectCards(SkillTarget target, IEnumerable<ClubMemberCard> targetCards, TargetContext context, SkillEffect[] effects)
        {
            foreach (var targetCard in targetCards)
                EffectCard(target, targetCard, context, effects);
        }

        private void EffectCard(SkillTarget target, ClubMemberCard targetCard, TargetContext context, SkillEffect[] effects)
        {
            foreach (var effect in effects)
            {
                var amount = GetAmount(effect.Amount, context);
                ChangeCardAmount(effect.Amount.Value, targetCard, amount);
            }
        }


        private bool TryEffectsLeader(SkillTarget target, TargetContext context, bool isTargetPlayerLeader, SkillEffect[] effects)
        {
            if (!IsLeaderTarget(target, context, true)) return false;
            foreach (var effect in effects)
            {
                var amount = GetAmount(effect.Amount, context);
                ChangeLeaderAmount(
                    effect.Amount.Value,
                    isTargetPlayerLeader ? context.PlayerContext : context.EnemyContext,
                    amount);
            }
            return true;
        }

        private bool IsLeaderTarget(SkillTarget target, TargetContext context, bool isPlayerLeaderTarget)
        {
            var leftTarget = GetLeaderTargetSource(target.Source,
                    isPlayerLeaderTarget ? context.PlayerContext : context.EnemyContext);
            var rightTarget = GetAmount(target.Amount, context);
            return IsTarget(target.Comparison, rightTarget, leftTarget);
        }

        private bool IsTarget(TargetComparison comparison, int left, int right)
            => comparison switch
            {
                TargetComparison.Equal => left == right,
                TargetComparison.GreaterThan => left > right,
                TargetComparison.LessThan => left < right,
                TargetComparison.GreaterThanOrEqual => left >= right,
                TargetComparison.LessThanOrEqual => left <= right,
                TargetComparison.NotEqual => left != right,
                _ => throw new ArgumentOutOfRangeException(nameof(comparison), comparison, null)
            };

        private int GetLeaderTargetSource(TargetSource source, FieldContext context)
            => source switch
            {
                TargetSource.Energy => context.PlayerHealthPoint,
                TargetSource.ClubBudget => context.ClubBudget,
                _ => throw new ArgumentOutOfRangeException(nameof(source), source, null)
            };


        #region AmountServices
        private int GetAmount(TargetAmount targetAmount, TargetContext context)
            => targetAmount.Source switch
            {
                AmountSource.Self => GetCardAmount(targetAmount.Value,
                    context.PlayerContext.GetLiveJoinedClubMemberCards().First(x => x.FieldId == context.SelfCardFieldId))
                * GetMultipleValue(targetAmount),

                AmountSource.PlayerLeader => GetLeaderAmount(targetAmount.Value, context.PlayerContext) * GetMultipleValue(targetAmount),

                AmountSource.EnemyLeader => GetLeaderAmount(targetAmount.Value, context.EnemyContext) * GetMultipleValue(targetAmount),
                AmountSource.TargetCard => GetCardAmount(targetAmount.Value,
                    context.PlayerContext.GetLiveJoinedClubMemberCards()
                        .Concat(context.EnemyContext.GetLiveJoinedClubMemberCards())
                        .First(x => x.FieldId == context.SelectedCardFieldId)) * GetMultipleValue(targetAmount),

                AmountSource.Fixed => targetAmount.FixedValue,

                _ => throw new ArgumentOutOfRangeException()
            };

        private int GetLeaderAmount(TargetSource targetSource, FieldContext context)
            => targetSource switch
            {
                TargetSource.Energy => context.PlayerHealthPoint,
                TargetSource.ClubBudget => context.ClubBudget,
                _ => throw new ArgumentOutOfRangeException()
            };

        private void ChangeLeaderAmount(TargetSource targetSource, FieldContext context, int effectAmount)
        {
            switch (targetSource)
            {
                case TargetSource.Energy:
                    context.PlayerHealthPoint += effectAmount;
                    break;
                case TargetSource.ClubBudget:
                    context.ClubBudget += effectAmount;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private int GetCardAmount(TargetSource targetSource, ClubMemberCard card)
            => targetSource switch
            {
                TargetSource.Qpa => card.Qpa,
                TargetSource.Energy => card.Energy,
                TargetSource.QpaAndEnergy => card.Energy,
                TargetSource.ClubBudget => card.ClubBudget,
                _ => throw new ArgumentOutOfRangeException()
            };

        private void ChangeCardAmount(TargetSource targetSource, ClubMemberCard card, int effectAmount)
        {
            switch (targetSource)
            {
                case TargetSource.Qpa:
                    card.Qpa += effectAmount;
                    break;
                case TargetSource.Energy:
                    card.Energy += effectAmount;
                    break;
                case TargetSource.QpaAndEnergy:
                    card.Energy += effectAmount;
                    card.Qpa += effectAmount;
                    break;
                case TargetSource.ClubBudget:
                    card.ClubBudget += effectAmount;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        //To Target-Amount Inside
        private int GetMultipleValue(TargetAmount targetAmount)
            => targetAmount.Multiple is not (0 or 1)
                ? targetAmount.Multiple
                : 1;
        #endregion
    }
}
