using Astrum.Http;
using Astrum.Json.Card;
using Astrum.Json.Gacha;
using Astrum.Json.GuildBattle;
using Astrum.Json.Item;
using Astrum.Json.Mypage;
using Astrum.Json.Raid;
using Astrum.Json.Stage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Astrum.Handler
{
    public class InfoUpdater
    {
        
        public static void UpdateMypageView(MypageInfo mypage, ViewModel viewModel)
        {
            viewModel.Name = mypage.status.name;
            viewModel.Level = mypage.status.level;

            viewModel.StaminaValue = mypage.status.stamina_value;
            viewModel.StaminaMax = mypage.status.stamina_max;

            viewModel.ExpValue = mypage.status.exp_value;
            viewModel.ExpMin = mypage.status.exp_min;
            viewModel.ExpMax = mypage.status.exp_max;

            viewModel.BpValue = mypage.status.bp_value;
            viewModel.BpMax = mypage.status.bp_max;

            viewModel.TpValue = mypage.status.tp_value;
            viewModel.TpMax = mypage.status.tp_max;

            viewModel.CardQuantity = mypage.status.card_quantity;
            viewModel.CardMax = mypage.status.card_max;
        }

        public static void UpdateStageView(StageInfo stage, ViewModel viewModel)
        {
            if (stage.status != null)
            {
                viewModel.Level = stage.status.level;

                viewModel.StaminaValue = stage.status.stamina.value;
                viewModel.StaminaMax = stage.status.stamina.max;

                viewModel.ExpValue = stage.status.exp.value;
                viewModel.ExpMin = stage.status.exp.min;
                viewModel.ExpMax = stage.status.exp.max;

                viewModel.BpValue = stage.status.bp.value;
                viewModel.BpMax = stage.status.bp.max;

                viewModel.TpValue = stage.status.tp.value;
                viewModel.TpMax = stage.status.tp.max;
                
                if (viewModel.IsFuryRaidEnable)
                {
                    viewModel.Fever = stage.status.furyraid != null && stage.status.furyraid.fever != null;
                }

                if (viewModel.IsLimitedRaidEnable)
                {
                    viewModel.Fever = stage.status.limitedraid != null && stage.status.limitedraid.fever != null;
                }

                if (viewModel.IsBreedingEnable)
                {
                    viewModel.Fever = stage.status.breeding != null && stage.status.breeding.fever != null && stage.status.breeding.fever.breedingPoint != null;
                }
            }
        }

        public static void UpdateItemStock(ItemInfo item, ViewModel viewModel)
        {
            if (AstrumClient.INSTANT_HALF_STAMINA.Equals(item._id))
            {
                viewModel.StaminaHalfStock = item.stock;
            }
            else if (AstrumClient.INSTANT_STAMINA.Equals(item._id))
            {
                viewModel.StaminaStock = item.stock;
            }
            else if (AstrumClient.INSTANT_MINI_BP.Equals(item._id))
            {
                viewModel.BpMiniStock = item.stock;
            }
            else if (AstrumClient.INSTANT_BP.Equals(item._id))
            {
                viewModel.BpStock = item.stock;
            }
        }


        public static void UpdateItemStock(UseItemResult item, ViewModel viewModel)
        {
            if (AstrumClient.INSTANT_HALF_STAMINA.Equals(item._id))
            {
                viewModel.StaminaHalfStock = item.stock.after;
                viewModel.StaminaValue = item.value.after;
            }
            else if (AstrumClient.INSTANT_STAMINA.Equals(item._id))
            {
                viewModel.StaminaStock = item.stock.after;
                viewModel.StaminaValue = item.value.after;
            }
            else if (AstrumClient.INSTANT_MINI_BP.Equals(item._id))
            {
                viewModel.BpMiniStock = item.stock.after;
                viewModel.BpValue = item.value.after;
            }
            else if (AstrumClient.INSTANT_BP.Equals(item._id))
            {
                viewModel.BpStock = item.stock.after;
                viewModel.BpValue = item.value.after;
            }
        }

        public static void UpdateBpAfterRaidBattle(RaidBattleInfo battleInfo, ViewModel viewModel)
        {
            if (battleInfo.isPlaying || battleInfo.isWin || battleInfo.isLose)
            {
                viewModel.BpValue = battleInfo.bpValue;
                viewModel.BpMax = battleInfo.bpMax;
            }
        }

        public static void UpdateBattleDamage(BossBattleResultInfo resultInfo, ViewModel viewModel)
        {
            if (!resultInfo.isEnd)
            {
                var atkSum = resultInfo.result.afterDeck.Sum(deck => deck.Value.atk);
                if (AstrumClient.FULL.Equals(resultInfo.init.bpType))
                {
                    viewModel.BaseDamage = atkSum;
                }
                else
                {
                    viewModel.BaseDamage = atkSum * 5;
                }
            }
        }



        public static void UpdateGuildBattleStatus(GuildBattleStatus status, ViewModel viewModel)
        {

            viewModel.TpValue = status.tp.value;
            viewModel.TpMax = status.tp.max;

        }


        public static void UpdataGachaResult(GachaResult result, ViewModel viewModel)
        {
            viewModel.CardQuantity = result.card.value;
            viewModel.CardMax = result.card.max;
        }

        public static void UpdateRaiseInfo(RaiseInfo raiseInfo,ViewModel viewModel)
        {
            viewModel.CardQuantity = raiseInfo.card.value;
            viewModel.CardMax = raiseInfo.card.max;

            viewModel.TrainingBase = raiseInfo.@base;
            viewModel.TrainingBaseId = raiseInfo.@base._id;
            viewModel.TrainingBaseRare = raiseInfo.@base.rare;
            viewModel.TrainingBaseName = raiseInfo.@base.name;
            viewModel.TrainingBaseLevel = raiseInfo.@base.level;
            viewModel.TrainingBaseMaxLevel = raiseInfo.@base.maxLevel;
            viewModel.TrainingBaseAbilityLevel = raiseInfo.@base.abilityLevel;
            viewModel.TrainingBaseMaxAbilityLevel = raiseInfo.@base.maxAbilityLevel;
            viewModel.TrainingBaseExpGrowth = raiseInfo.@base.growth.exp;
            viewModel.TrainingBaseAbilityGrowth = raiseInfo.@base.growth.ability;
        }

        public static void UpdateRaiseItemInfo(RaiseInfo raiseItemInfo, ViewModel viewModel)
        {
            viewModel.AbilityBookGoldStock = 0;
            viewModel.AbilityBookGoldAvailable = 0;
            viewModel.AbilityBookSilverStock = 0;
            viewModel.AbilityBookSilverAvailable = 0;
            viewModel.AbilityBookBronzeStock = 0;
            viewModel.AbilityBookBronzeAvailable = 0;
            viewModel.StrengthStatueGoldStock = 0;
            viewModel.StrengthStatueGoldAvailable = 0;
            viewModel.StrengthStatueSilverStock = 0;
            viewModel.StrengthStatueSilverAvailable = 0;
            viewModel.StrengthStatueBronzeStock = 0;
            viewModel.StrengthStatueBronzeAvailable = 0;

            if (raiseItemInfo.items != null)
            {
                if (raiseItemInfo.items.ability != null)
                {
                    foreach (var item in raiseItemInfo.items.ability)
                    {
                        switch (item._id)
                        {
                            case AstrumClient.INSTANT_ABILITY_BOOK_GOLD:
                                viewModel.AbilityBookGoldStock = item.stock;
                                viewModel.AbilityBookGoldAvailable = item.available;
                                break;
                            case AstrumClient.INSTANT_ABILITY_BOOK_SILVER:
                                viewModel.AbilityBookSilverStock = item.stock;
                                viewModel.AbilityBookSilverAvailable = item.available;
                                break;
                            case AstrumClient.INSTANT_ABILITY_BOOK_BRONZE:
                                viewModel.AbilityBookBronzeStock = item.stock;
                                viewModel.AbilityBookBronzeAvailable = item.available;
                                break;
                        }
                    }
                }

                if (raiseItemInfo.items.exp != null)
                {
                    foreach (var item in raiseItemInfo.items.exp)
                    {
                        switch (item._id)
                        {
                            case AstrumClient.INSTANT_STRENGTH_STATUE_GOLD:
                                viewModel.StrengthStatueGoldStock = item.stock;
                                viewModel.StrengthStatueGoldAvailable = item.available;
                                break;
                            case AstrumClient.INSTANT_STRENGTH_STATUE_SILVER:
                                viewModel.StrengthStatueSilverStock = item.stock;
                                viewModel.StrengthStatueSilverAvailable = item.available;
                                break;
                            case AstrumClient.INSTANT_STRENGTH_STATUE_BRONZE:
                                viewModel.StrengthStatueBronzeStock = item.stock;
                                viewModel.StrengthStatueBronzeAvailable = item.available;
                                break;
                        }
                    }
                }
            }
        }

    }
}
