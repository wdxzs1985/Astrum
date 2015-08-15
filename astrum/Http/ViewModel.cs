using Astrum.Json;
using Astrum.Json.Card;
using Astrum.Json.Gacha;
using Astrum.Json.Raid;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Astrum.Http
{
    public class ViewModel : INotifyPropertyChanged
    {
        #region Interface
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion

        private List<LoginUser> _login_user_list;
        public List<LoginUser> LoginUserList
        {
            get
            {
                return _login_user_list;
            }
            set
            {
                _login_user_list = value;
                NotifyPropertyChanged("LoginUserList");
            }
        }

        private List<GachaInfo> _gacha_list;
        public List<GachaInfo> GachaList
        {
            get
            {
                return _gacha_list;
            }
            set
            {
                _gacha_list = value;
                NotifyPropertyChanged("GachaList");
            }
        }

        public bool IsLogin { get; set; }

        private bool _running;
        public bool IsRunning
        {
            get { return _running; }
            set
            {
                _running = value;
                NotifyPropertyChanged("IsRunning");
                //NotifyPropertyChanged("CanUseStaminaHalf");
                // NotifyPropertyChanged("CanUseStamina");
                //NotifyPropertyChanged("CanUseBpMini");
                //NotifyPropertyChanged("CanUseBp");

            }
        }

        private bool _ready;
        public bool IsReady
        {
            get { return _ready; }
            set
            {
                _ready = value;
                NotifyPropertyChanged("IsReady");
                NotifyPropertyChanged("CanUseStaminaHalf");
                NotifyPropertyChanged("CanUseStamina");
                NotifyPropertyChanged("CanUseBpMini");
                NotifyPropertyChanged("CanUseBp");
            }
        }

        public string WindowTitle
        {
            get
            {
                if (Name != null)
                {
                    return String.Format("プリコネ [{0} (Lv {1})]", Name, Level);
                }
                return "プリンセスコネクト";
            }
        }

        private int _bpMiniStock;
        private int _bpStock;
        private int _staminaHalfStock;
        private int _staminaStock;
        private int _minStaminaStock;
        private int _minBpStock;

        public int BpMiniStock
        {
            get
            {
                return _bpMiniStock;
            }
            set
            {
                _bpMiniStock = value;
                NotifyPropertyChanged("BpMiniStock");
                NotifyPropertyChanged("CanUseBpMini");
            }
        }

        public int BpStock
        {
            get
            {
                return _bpStock;
            }
            set
            {
                _bpStock = value;
                NotifyPropertyChanged("BpStock");
                NotifyPropertyChanged("CanUseBp");
            }
        }

        public int MinBpStock
        {
            get
            {
                return _minBpStock;
            }
            set
            {
                _minBpStock = value;
                NotifyPropertyChanged("MinBpStock");
            }
        }


        public int StaminaHalfStock
        {
            get
            {
                return _staminaHalfStock;
            }
            set
            {
                _staminaHalfStock = value;
                NotifyPropertyChanged("StaminaHalfStock");
                NotifyPropertyChanged("CanUseStaminaHalf");
            }
        }

        public int StaminaStock
        {
            get
            {
                return _staminaStock;
            }
            set
            {
                _staminaStock = value;
                NotifyPropertyChanged("StaminaStock");
                NotifyPropertyChanged("CanUseStamina");
            }
        }

        public int MinStaminaStock
        {
            get
            {
                return _minStaminaStock;
            }
            set
            {
                _minStaminaStock = value;
                NotifyPropertyChanged("MinStaminaStock");
            }
        }

        private string _history;
        public string History
        {
            get
            {
                return _history;
            }
            set
            {
                _history = value;
                NotifyPropertyChanged("History");
            }
        }

        private bool _quest_enable;
        private bool _guild_battle_enable;
        private bool _gacha_enable;
        private bool _training_enable;
        private bool _training_base_enable;

        public bool IsQuestEnable
        {
            get
            {
                return _quest_enable;
            }
            set
            {
                _quest_enable = value;
                NotifyPropertyChanged("IsQuestEnable");
            }
        }

        public bool IsGuildBattleEnable
        {
            get
            {
                return _guild_battle_enable;
            }
            set
            {
                _guild_battle_enable = value;
                NotifyPropertyChanged("IsGuildBattleEnable");
            }
        }

        public bool IsGachaEnable
        {
            get
            {
                return _gacha_enable;
            }
            set
            {
                _gacha_enable = value;
                NotifyPropertyChanged("IsGachaEnable");
            }
        }

        public bool IsTrainingEnable
        {
            get
            {
                return _training_enable;
            }
            set
            {
                _training_enable = value;
                NotifyPropertyChanged("IsTrainingEnable");
            }
        }

        public bool IsTrainingBaseEnable
        {
            get
            {
                return _training_base_enable;
            }
            set
            {
                _training_base_enable = value;
                NotifyPropertyChanged("IsTrainingBaseEnable");
            }
        }

        private string _name;
        private int _level;
        private long _exp_value;
        private long _exp_max;
        private long _exp_min;
        private int _stamina_max;
        private int _stamina_value;
        private int _bp_value;
        private int _bp_max;
        private int _tp_value;
        private int _tp_max;
        private int _card_quantity;
        private int _card_max;

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
                NotifyPropertyChanged("Name");
                NotifyPropertyChanged("WindowTitle");
            }
        }
        public int Level
        {
            get
            {
                return _level;
            }
            set
            {
                _level = value;
                NotifyPropertyChanged("Level");
                NotifyPropertyChanged("WindowTitle");
            }
        }

        public long ExpValue
        {
            get
            {
                try
                {
                    return _exp_value;
                }
                catch
                {
                    return 0;
                }

            }
            set
            {
                _exp_value = value;
                NotifyPropertyChanged("Exp");
                NotifyPropertyChanged("ExpValue");
                NotifyPropertyChanged("ExpProgress");
            }
        }

        public long ExpMax
        {
            get
            {
                return _exp_max;
            }
            set
            {
                _exp_max = value;
                NotifyPropertyChanged("Exp");
                NotifyPropertyChanged("ExpMax");
                NotifyPropertyChanged("ExpProgress");
            }
        }

        public long ExpMin
        {
            get
            {
                return _exp_min;
            }
            set
            {
                _exp_min = value;
                NotifyPropertyChanged("Exp");
                NotifyPropertyChanged("ExpMin");
                NotifyPropertyChanged("ExpProgress");
            }
        }

        public int ExpProgress
        {
            get
            {
                if (_exp_max == 0)
                {
                    return 0;
                }
                double expMax = (double)(_exp_max - _exp_min);
                double expVal = (double)(_exp_value - _exp_min);

                double rate = expVal / expMax;

                return (int)(rate * 100);
            }
        }

        public int StaminaValue
        {
            get
            {
                return _stamina_value;
            }
            set
            {
                _stamina_value = value;
                NotifyPropertyChanged("Stamina");
                NotifyPropertyChanged("StaminaValue");
                NotifyPropertyChanged("StaminaProgress");
            }
        }
        public int StaminaMax
        {
            get
            {
                return _stamina_max;
            }
            set
            {
                _stamina_max = value;
                NotifyPropertyChanged("Stamina");
                NotifyPropertyChanged("StaminaMax");
                NotifyPropertyChanged("StaminaProgress");
            }
        }


        public int StaminaProgress
        {
            get
            {
                if (_stamina_max == 0)
                {
                    return 0;
                }
                double staMax = (double)_stamina_max;
                double staVal = (double)_stamina_value;

                double rate = staVal / staMax;

                return (int)(rate * 100);
            }
        }

        public int BpValue
        {
            get
            {
                return _bp_value;
            }
            set
            {
                _bp_value = value;
                NotifyPropertyChanged("Bp");
                NotifyPropertyChanged("BpValue");
                NotifyPropertyChanged("BpProgress");
            }
        }
        public int BpMax
        {
            get
            {
                return _bp_max;
            }
            set
            {
                _bp_max = value;
                NotifyPropertyChanged("Bp");
                NotifyPropertyChanged("BpMax");
                NotifyPropertyChanged("BpProgress");
            }
        }


        public int BpProgress
        {
            get
            {
                if (_bp_max == 0)
                {
                    return 0;
                }
                double max = (double)_bp_max;
                double val = (double)_bp_value;

                double rate = val / max;

                return (int)(rate * 100);
            }
        }

        public int TpValue
        {
            get
            {
                return _tp_value;
            }
            set
            {
                _tp_value = value;
                NotifyPropertyChanged("Tp");
                NotifyPropertyChanged("TpValue");
                NotifyPropertyChanged("TpProgress");
            }
        }

        public int TpMax
        {
            get
            {
                return _tp_max;
            }
            set
            {
                _tp_max = value;
                NotifyPropertyChanged("Tp");
                NotifyPropertyChanged("TpMax");
                NotifyPropertyChanged("TpProgress");
            }
        }


        public int TpProgress
        {
            get
            {
                if (_tp_max == 0)
                {
                    return 0;
                }
                double max = (double)_tp_max;
                double val = (double)_tp_value;

                double rate = val / max;

                return (int)(rate * 100);
            }
        }

        public int CardQuantity
        {
            get
            {
                return _card_quantity;
            }
            set
            {
                _card_quantity = value;
                NotifyPropertyChanged("Card");
                NotifyPropertyChanged("CardQuantity");
                NotifyPropertyChanged("CardProgress");
            }
        }

        public int CardMax
        {
            get
            {
                return _card_max;
            }
            set
            {
                _card_max = value;
                NotifyPropertyChanged("Card");
                NotifyPropertyChanged("CardMax");
                NotifyPropertyChanged("CardProgress");
            }
        }

        public int CardProgress
        {
            get
            {
                if (_card_max == 0)
                {
                    return 0;
                }
                double max = (double)_card_max;
                double val = (double)_card_quantity;

                double rate = val / max;

                return (int)(rate * 100);
            }
        }

        public string Exp
        {
            get
            {
                return (ExpValue - ExpMin) + " / " + (ExpMax - ExpMin);
            }
        }

        public string Stamina
        {
            get
            {
                return _stamina_value + " / " + _stamina_max;
            }
        }

        public string Bp
        {
            get
            {
                return _bp_value + " / " + _bp_max;
            }
        }

        public string Tp
        {
            get
            {
                return _tp_value + " / " + _tp_max;
            }
        }

        public string Card
        {
            get
            {
                return _card_quantity + " / " + _card_max;
            }
        }



        public bool IsFuryRaidEnable { get; set; }
        public string FuryRaidEventId { get; set; }
        public bool IsFuryRaid { get; set; }
        //public List<RaidBattleInfo> FuryRaidFindList { get; set; }

        public bool IsLimitedRaidEnable { get; set; }
        public string LimitedRaidEventId { get; set; }
        public bool IsLimitedRaid { get; set; }

        public bool CanAttack
        {
            get
            {
                return (IsFuryRaidEnable == IsFuryRaid) && (IsLimitedRaidEnable == IsLimitedRaid);
            }
        }

        public bool CanFullAttack
        {
            get
            {
                bool hasBp = BpValue >= 3;
                return CanAttack && hasBp;
            }
        }

        public bool CanFullAttackForEvent
        {
            get
            {
                bool hasBp = BpValue >= 3;
                bool hasBpStock = BpValue + CanUseBpQuantity >= 3;

                return CanAttack && (hasBp || (Fever && hasBpStock));
            }
        }

        public int CanUseBpQuantity
        {
            get
            {
                int quantity = BpMiniStock - MinBpStock;
                return quantity > 0 ? quantity : 0;
            }
        }

        private bool _fever;
        public bool Fever
        {
            get
            {
                return _fever;
            }
            set
            {
                _fever = value;
                NotifyPropertyChanged("Fever");
            }
        }

        public bool IsStaminaEmpty { get; set; }

        private int _keepStamina = 100;
        public int KeepStamina
        {
            get
            {
                return _keepStamina;
            }
            set
            {
                _keepStamina = value;
                NotifyPropertyChanged("KeepStamina");
            }
        }


        public string GuildBattleId { get; set; }

        private bool _tpNormalAvailable;
        private bool _tpChatAvailable;
        private bool _tpRouletteAvailable;


        public bool IsTpNormalAvailable
        {
            get
            {
                return _tpNormalAvailable;
            }
            set
            {
                _tpNormalAvailable = value;
                NotifyPropertyChanged("IsTpNormalAvailable");
            }
        }

        public bool IsTpChatAvailable
        {
            get
            {
                return _tpChatAvailable;
            }
            set
            {
                _tpChatAvailable = value;
                NotifyPropertyChanged("IsTpChatAvailable");
            }
        }

        public bool IsTpRouletteAvailable
        {
            get
            {
                return _tpRouletteAvailable;
            }
            set
            {
                _tpRouletteAvailable = value;
                NotifyPropertyChanged("IsTpRouletteAvailable");
            }
        }

        public bool CanUseStaminaHalf
        {
            get
            {
                //return StaminaHalfStock > 0 && !IsRunning;
                return StaminaHalfStock > 0 && IsReady;
            }
        }

        public bool CanUseStamina
        {
            get
            {
                return StaminaStock > 0 && IsReady;
            }
        }

        public bool CanUseBpMini
        {
            get
            {
                return BpMiniStock > 0 && IsReady;
            }
        }

        public bool CanUseBp
        {
            get
            {
                return BpStock > 0 && IsReady;
            }
        }


        #region Training/Raise

        private string _training_base_id;
        private string _training_base_name;
        private int _training_base_level;
        private int _training_base_max_level;
        private int _training_base_ability_level;
        private int _training_base_max_ability_level;
        private int _training_base_rare;
        private int _training_base_exp_growth;
        private int _training_base_ability_growth;

        private List<CardInfo> _training_base_list;

        public string TrainingBaseId
        {
            get
            {
                return _training_base_id;
            }
            set
            {
                _training_base_id = value;
                NotifyPropertyChanged("TrainingBaseId");
            }
        }

        public string TrainingBase
        {
            get
            {
                string rare = "";
                switch (TrainingBaseRare)
                {
                    case 4:
                        rare = "[SSR]";
                        break;
                    case 3:
                        rare = "[ SR]";
                        break;
                    case 2:
                        rare = "[  R]";
                        break;
                    case 1:
                        rare = "[  N]";
                        break;
                    default:
                        rare = "[???]";
                        break;
                }
                return string.Format("{0}{1}", rare, TrainingBaseName);
            }
        }

        public string TrainingBaseName
        {
            get
            {
                return _training_base_name;
            }
            set
            {
                _training_base_name = value;
                NotifyPropertyChanged("TrainingBaseName");
                NotifyPropertyChanged("TrainingBase");
            }
        }

        public int TrainingBaseRare
        {
            get
            {
                return _training_base_rare;
            }
            set
            {
                _training_base_rare = value;
                NotifyPropertyChanged("TrainingBaseRare");
                NotifyPropertyChanged("TrainingBase");
            }
        }

        public int TrainingBaseLevel
        {
            get
            {
                return _training_base_level;
            }
            set
            {
                _training_base_level = value;
                NotifyPropertyChanged("TrainingBaseLevel");
            }
        }

        public int TrainingBaseMaxLevel
        {
            get
            {
                return _training_base_max_level;
            }
            set
            {
                _training_base_max_level = value;
                NotifyPropertyChanged("TrainingBaseMaxLevel");
            }
        }

        public int TrainingBaseAbilityLevel
        {
            get
            {
                return _training_base_ability_level;
            }
            set
            {
                _training_base_ability_level = value;
                NotifyPropertyChanged("TrainingBaseAbilityLevel");
            }
        }

        public int TrainingBaseMaxAbilityLevel
        {
            get
            {
                return _training_base_max_ability_level;
            }
            set
            {
                _training_base_max_ability_level = value;
                NotifyPropertyChanged("TrainingBaseMaxAbilityLevel");
            }
        }

        public int TrainingBaseExpGrowth
        {
            get
            {
                return _training_base_exp_growth;
            }
            set
            {
                _training_base_exp_growth = value;
                NotifyPropertyChanged("TrainingBaseExpGrowth");
            }
        }

        public int TrainingBaseAbilityGrowth
        {
            get
            {
                return _training_base_ability_growth;
            }
            set
            {
                _training_base_ability_growth = value;
                NotifyPropertyChanged("TrainingBaseAbilityGrowth");
            }
        }

        public List<CardInfo> TrainingBaseList
        {
            get
            {
                return _training_base_list;
            }
            set
            {
                _training_base_list = value;
                NotifyPropertyChanged("TrainingBaseList");
            }
        }

        private int _ablility_book_gold_stock;
        private int _ablility_book_gold_available;
        private int _ablility_book_silver_stock;
        private int _ablility_book_silver_available;
        private int _ablility_book_bronze_stock;
        private int _ablility_book_bronze_available;

        private int _strength_statue_gold_stock;
        private int _strength_statue_gold_available;
        private int _strength_statue_silver_stock;
        private int _strength_statue_silver_available;
        private int _strength_statue_bronze_stock;
        private int _strength_statue_bronze_available;

        public int AbilityBookGoldStock
        {
            get
            {
                return _ablility_book_gold_stock;
            }
            set
            {
                _ablility_book_gold_stock = value;
                NotifyPropertyChanged("AbilityBookGoldStock");
            }
        }

        public int AbilityBookGoldAvailable
        {
            get
            {
                return _ablility_book_gold_available;
            }
            set
            {
                _ablility_book_gold_available = value;
                NotifyPropertyChanged("AbilityBookGoldAvailable");
            }
        }


        public int AbilityBookSilverStock
        {
            get
            {
                return _ablility_book_silver_stock;
            }
            set
            {
                _ablility_book_silver_stock = value;
                NotifyPropertyChanged("AbilityBookSilverStock");
            }
        }

        public int AbilityBookSilverAvailable
        {
            get
            {
                return _ablility_book_silver_available;
            }
            set
            {
                _ablility_book_silver_available = value;
                NotifyPropertyChanged("AbilityBookSilverAvailable");
            }
        }

        public int AbilityBookBronzeStock
        {
            get
            {
                return _ablility_book_bronze_stock;
            }
            set
            {
                _ablility_book_bronze_stock = value;
                NotifyPropertyChanged("AbilityBookBronzeStock");
            }
        }

        public int AbilityBookBronzeAvailable
        {
            get
            {
                return _ablility_book_bronze_available;
            }
            set
            {
                _ablility_book_bronze_available = value;
                NotifyPropertyChanged("AbilityBookBronzeAvailable");
            }
        }


        public int StrengthStatueGoldStock
        {
            get
            {
                return _strength_statue_gold_stock;
            }
            set
            {
                _strength_statue_gold_stock = value;
                NotifyPropertyChanged("StrengthStatueGoldStock");
            }
        }

        public int StrengthStatueGoldAvailable
        {
            get
            {
                return _strength_statue_gold_available;
            }
            set
            {
                _strength_statue_gold_available = value;
                NotifyPropertyChanged("StrengthStatueGoldAvailable");
            }
        }


        public int StrengthStatueSilverStock
        {
            get
            {
                return _strength_statue_silver_stock;
            }
            set
            {
                _strength_statue_silver_stock = value;
                NotifyPropertyChanged("StrengthStatueSilverStock");
            }
        }

        public int StrengthStatueSilverAvailable
        {
            get
            {
                return _strength_statue_silver_available;
            }
            set
            {
                _strength_statue_silver_available = value;
                NotifyPropertyChanged("StrengthStatueSilverAvailable");
            }
        }

        public int StrengthStatueBronzeStock
        {
            get
            {
                return _strength_statue_bronze_stock;
            }
            set
            {
                _strength_statue_bronze_stock = value;
                NotifyPropertyChanged("StrengthStatueBronzeStock");
            }
        }

        public int StrengthStatueBronzeAvailable
        {
            get
            {
                return _strength_statue_bronze_available;
            }
            set
            {
                _strength_statue_bronze_available = value;
                NotifyPropertyChanged("StrengthStatueBronzeAvailable");
            }
        }

        #endregion
    }
}