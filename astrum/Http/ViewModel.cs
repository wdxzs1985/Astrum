using Astrum.Json;
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
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

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

        public bool IsRunning { get; set; }

        public string WindowTitle
        {
            get
            {
                if(Name != null) {
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
                double expMax = (double) (_exp_max - _exp_min);
                double expVal = (double) (_exp_value - _exp_min);

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

        public long gacha { get; set; }
        public long lilu { get; set; }
        public int card_quantity { get; set; }
        public int card_max { get; set; }
        
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


        public bool IsFuryRaidEnable { get; set; }
        public string FuryRaidEventId { get; set; }
        public bool IsFuryRaid { get; set; }

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

        private int _maxKeepStamina = 100;
        public int MaxKeepStamina
        {
            get
            {
                return _maxKeepStamina;
            }
            set
            {
                _maxKeepStamina = value;
                NotifyPropertyChanged("MaxKeepStamina");
            }
        }

    }
}
