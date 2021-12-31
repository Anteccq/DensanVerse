using System.Collections.Concurrent;
using DensanVerse.Server.Models.Battle;
using DensanVerse.Server.Models.Cards;
using DensanVerse.Server.Models.Cards.Effects;
using DensanVerse.Server.Models.Cards.Skills;
using DensanVerse.Server.Models.Cards.Skills.Conditions;
using DensanVerse.Server.Models.Field;
using DensanVerse.Server.Models.Players;

namespace DensanVerse.Server.Services
{
    public class BattleService
    {
        private ConcurrentDictionary<string, BattleContext> CachedBattles = new();
        public void CreateBattleRoom()
        {
            var battle = new BattleContext
            {
                BattleId = Guid.NewGuid().ToString("N"),
                TeamOwnerFieldContext = CreateField(0, 10, PlayerClass.Cg),
                TeamRebelFieldContext = CreateField(1, 100, PlayerClass.Pg),
                TurnCount = 0
            };

            CachedBattles.AddOrUpdate(battle.BattleId, _ => battle, (_,_) => battle);
        }

        public BattleContext GetBattle(string battleId)
        {
            return CachedBattles.TryGetValue(battleId, out var battle)
                ? battle
                : throw new InvalidOperationException("GG");
        }

        public void FieldCardAction(string battleId, int actionPlayerId, string selfCardFieldId, FieldEffect effect, string targetCardFieldId)
        {
            var battle = GetBattle(battleId);
            var (playerContext, enemyContext) =
                battle.TeamRebelFieldContext.Player.Id == actionPlayerId
                ? (battle.TeamRebelFieldContext, battle.TeamOwnerFieldContext)
                : (battle.TeamOwnerFieldContext, battle.TeamRebelFieldContext);

            var service = new FieldEffectService();
            service.ExecuteEffect(effect, playerContext, enemyContext, selfCardFieldId, targetCardFieldId);
        }

        //Normal Field
        public FieldContext CreateField(int playerId, int deckId, PlayerClass playerClass)
        {
            return new FieldContext
            {
                CardDeck = new List<Card>(),
                EnergyDrinkAmount = 0,
                EnvironmentCards = new EnvironmentCard[2],
                HandCards = new List<Card>(),
                JoinedClubMemberCards = new ClubMemberCard[5],
                ObOgCards = new List<Card>(),
                Player = new Player
                {
                    Id = playerId,
                    Name = $"電算太郎 {playerId}号機",
                    Rank = new Rank
                    {
                        Id = playerId,
                        Point = 999,
                        Title = "豪雨"
                    }
                },
                PlayerClass = playerClass
            };
        }

        public void CloseBattleRoom(string battleId, int winnerId)
        {
            CachedBattles.TryRemove(battleId, out var battle);
            //BattleDb.Add(battle, winnerId);
        }
    }
}
