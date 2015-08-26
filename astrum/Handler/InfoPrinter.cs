using Astrum.Http;
using Astrum.Json.Breeding;
using Astrum.Json.Gacha;
using Astrum.Json.Gift;
using Astrum.Json.GuildBattle;
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
    public class InfoPrinter
    {
        public static void PrintMypage(MypageInfo mypage, ViewModel viewModel)
        {
            string history = "";
            history += String.Format("　　角色：{0} (L{1})", mypage.status.name, mypage.status.level) + Environment.NewLine;
            history += String.Format("　总战力：{0}", mypage.total) + Environment.NewLine;
            history += String.Format("　　攻击：{0}，　　防御：{1}", mypage.status.atk, mypage.status.df) + Environment.NewLine;
            history += String.Format("必杀攻击：{0}，必杀防御：{1}", mypage.status.mat, mypage.status.mdf) + Environment.NewLine;
            history += String.Format("所属工会：{0}, 工会排行：{1}", mypage.guild.name, mypage.guild.rank) + Environment.NewLine;

            viewModel.History = history;
        }
        
        public static void PrintGiftResult(GiftResult giftResult, ViewModel viewModel)
        {
            string history = "";
            if (giftResult.item > 0)
            {
                history += String.Format("　　　获得道具：{0}", giftResult.item) + Environment.NewLine;
            }
            if (giftResult.lilu > 0)
            {
                history += String.Format("　　　获得ルピ：{0}", giftResult.lilu) + Environment.NewLine;
            }
            if (giftResult.card > 0)
            {
                history += String.Format("　　　获得少女：{0}", giftResult.card) + Environment.NewLine;
            }
            if (giftResult.practice > 0)
            {
                history += String.Format("　　获得摸擬戦：{0}", giftResult.practice) + Environment.NewLine;
            }
            if (giftResult.enhance != null)
            {
                if (giftResult.enhance.strength > 0)
                {
                    history += String.Format("　　获得强化像：{0}", giftResult.enhance.strength) + Environment.NewLine;
                }
                if (giftResult.enhance.ability > 0)
                {
                    history += String.Format("　　获得技能書：{0}", giftResult.enhance.ability) + Environment.NewLine;
                }
                if (giftResult.enhance.limitbreak > 0)
                {
                    history += String.Format("　获得开花结晶：{0}", giftResult.enhance.limitbreak) + Environment.NewLine;
                }
            }

            if (giftResult.gacha != null)
            {
                if (giftResult.gacha.ContainsKey(AstrumClient.INSTANT_PLATINUM_GACHA_POINT))
                {
                    var value = giftResult.gacha[AstrumClient.INSTANT_PLATINUM_GACHA_POINT].value;
                    history += String.Format("　　　获得星钻：{0}", value) + Environment.NewLine;
                }
                if (giftResult.gacha.ContainsKey(AstrumClient.INSTANT_PLATINUM_GACHA_TICKET))
                {
                    var value = giftResult.gacha[AstrumClient.INSTANT_PLATINUM_GACHA_TICKET].value;
                    history += String.Format("　获得非洲护照：{0}", value) + Environment.NewLine;
                }
                if (giftResult.gacha.ContainsKey(AstrumClient.INSTANT_RARE_RAID_MEDAL))
                {
                    var value = giftResult.gacha[AstrumClient.INSTANT_RARE_RAID_MEDAL].value;
                    history += String.Format("获得稀有魔星币：{0}", value) + Environment.NewLine;
                }
                if (giftResult.gacha.ContainsKey(AstrumClient.INSTANT_RAID_MEDAL))
                {
                    var value = giftResult.gacha[AstrumClient.INSTANT_RAID_MEDAL].value;
                    history += String.Format("　　获得魔星币：{0}", value) + Environment.NewLine;
                }
            }

            viewModel.History = history;
        }

        public static void PrintStageInfo(StageInfo stage, ViewModel viewModel)
        {
            string history = "";
            history += String.Format("场景：{0}[{1}] ({2})", stage.name, stage.stage, stage.isBossStage ? "BOSS" : stage.progress + "%") + Environment.NewLine;

            if (stage.status != null)
            {
                history += String.Format("体力：{0} / {1}", stage.status.stamina.value, stage.status.stamina.max) + Environment.NewLine;
                history += String.Format("经验：{0} / {1}", stage.status.exp.value, stage.status.exp.max) + Environment.NewLine;
                history += String.Format("　BP：{0} / {1}", stage.status.bp.value, stage.status.bp.max) + Environment.NewLine;
                history += String.Format("　TP：{0} / {1}", stage.status.tp.value, stage.status.tp.max) + Environment.NewLine;
            }
            viewModel.History = history;
        }

        public static void PrintAreaBossInfo(AreaBossInfo boss, ViewModel viewModel)
        {
            string history = "";
            history += String.Format("{0}的{1}出现了！", boss.areaName, boss.name) + Environment.NewLine;
            history += String.Format("血量：{0} / {1}", boss.hp - boss.totalDamage, boss.hp) + Environment.NewLine;
            viewModel.History = history;
        }

        public static void PrintRaidBattleInfo(RaidBattleInfo battleInfo, ViewModel viewModel)
        {
            if (battleInfo.isPlaying)
            {
                string history = "";

                string rare = "";
                string type = battleInfo.type == "find" ? "发现" : "救援";
                switch (battleInfo.rare)
                {
                    case 1:
                        rare = "初级魔星兽";
                        break;
                    case 2:
                        rare = "中级魔星兽";
                        break;
                    case 3:
                        rare = "上级魔星兽";
                        break;
                    case 4:
                        rare = "星兽王";
                        break;
                    default:
                        rare = "魔星兽";
                        break;
                }
                
                history += String.Format("{0}({1} L{2})出现了", rare, battleInfo.name, battleInfo.level) + Environment.NewLine;
                history += String.Format("血量: {0} / {1}", battleInfo.hp - battleInfo.totalDamage, battleInfo.hp) + Environment.NewLine;
                history += String.Format("类型:{0}", type) + Environment.NewLine;
                viewModel.History = history;
            }
        }

        public static void PrintBossBattleResult(BossBattleResultInfo resultInfo, ViewModel viewModel)
        {
            if (resultInfo.result != null)
            {
                string history = "";
                history += String.Format("战斗结束：{0}", resultInfo.result.resultType) + Environment.NewLine;
                history += String.Format("BOSS血量：{0} / {1}", resultInfo.result.afterBoss.hp, resultInfo.result.afterBoss.maxHp) + Environment.NewLine;
                viewModel.History = history;
            }
        }


        public static void PrintGuildBattleInfo(GuildBattleInfo battleInfo, ViewModel viewModel)
        {
            string history = "";

            foreach (var guild in battleInfo.guilds)
            {
                history += guild.name + Environment.NewLine;
                history += String.Format("普通攻击：{0}", guild.combo.attack.count) + Environment.NewLine;
                history += String.Format("远程攻击：{0}", guild.combo.remote.count) + Environment.NewLine;
                history += String.Format("必杀攻击：{0}", guild.combo.special.count) + Environment.NewLine;
                history += String.Format("　　应援：{0}", guild.combo.yell.count) + Environment.NewLine;
                history += Environment.NewLine;
            }
            viewModel.History = history;
        }

        public static void PrintGuildBattleCmdResult(CmdResult result, ViewModel viewModel)
        {
            var status = result.battlestate.status;

            string history = "";

            history += String.Format("{0} (Combo {1})", result.cmd.name, result.history.combo) + Environment.NewLine;

            viewModel.History = history;
        }

        public static void PrintGachaResult(GachaResult result, ViewModel viewModel)
        {
            string history = "";
            foreach (var item in result.list)
            {

                if ("item".Equals(item.type))
                {
                    history += String.Format("{0} x {1}", item.name, item.value) + Environment.NewLine;
                }
                else if ("card".Equals(item.type))
                {
                    string rare = "";
                    switch (item.rare)
                    {
                        case 4:
                            rare = "[欧皇]";
                            break;
                        case 3:
                            rare = "[脸帝]";
                            break;
                        case 2:
                            rare = "[狗粮]";
                            break;
                        case 1:
                            rare = "[渣渣]";
                            break;
                        default:
                            rare = "[????]";
                            break;
                    }

                    history += String.Format("{0}{1} x {2}", rare, item.name, item.value) + Environment.NewLine;
                }
            }
            viewModel.History = history;
        }

        public static void PrintBreedingInfo(BreedingInfo info, ViewModel viewModel)
        {
            string history = "";
            history += info.name + Environment.NewLine;
            history += String.Format("{0} : {1}", info.breedingPointName, info.breedingPoint) + Environment.NewLine;
            history += String.Format("{0} : {1}", "交换pt", info.exchangePoint) + Environment.NewLine;


            history += String.Format("个人讨伐{0}, 还差{1}次获得{2}", info.totalRewards.user.total, info.totalRewards.user.next.requirement - info.totalRewards.user.total, info.totalRewards.user.next.name) + Environment.NewLine;
            history += String.Format("工会讨伐{0}, 还差{1}次获得{2}", info.totalRewards.guild.total, info.totalRewards.guild.next.requirement - info.totalRewards.guild.total, info.totalRewards.guild.next.name) + Environment.NewLine;

            //partner
            foreach (var partner in info.partners)
            {
                history += String.Format("{0}", partner.card.DisplayName) + Environment.NewLine;
                history += String.Format("絆Lv {0}/{1}", partner.breedingLevel, partner.maxBreedingLevel) + Environment.NewLine;
                history += String.Format("絆Lv {0}:{1}", partner.nextBreedReward.title, partner.nextBreedReward.description) + Environment.NewLine;
            }

            viewModel.History = history;
        }


    }
}
