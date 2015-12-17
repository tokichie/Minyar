using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using FP.DAL.DAO;
using Microsoft.Win32;
using Minyar;
using Minyar.Charm;
using MinyarClient.ViewModel;

namespace MinyarClient.Model {
    public class MainModel : Livet.NotificationObject {

        public MainModel() {
            MinedItemSets = new ObservableCollection<ItemTidSet<string, RepeatableTid>>();
        }

        private ObservableCollection<ItemTidSet<string, RepeatableTid>> _MinedItemSets;

        public ObservableCollection<ItemTidSet<string, RepeatableTid>> MinedItemSets {
            get { return _MinedItemSets; }
            set {
                _MinedItemSets = value;
                RaisePropertyChanged();
            }
        }  

        public void StartMiningUsingFpGrowth(string filePath, int threshold) {
            var miner = new FPGrowthMiner(filePath, threshold);
            miner.GenerateFrequentItemsets();
            var itemSets = new ObservableCollection<ItemSet>(miner.GetAllMinedItemSets());
            //MinedItemSets = itemSets;
        }

        public void StartMiningUsingCharm(string filePath) {
            StartMiningUsingCharm(filePath, -1);
        }

        public void StartMiningUsingCharm(string filePath, int threshold) {
            var miner = threshold == -1 ? new ItTreeMiner(filePath) : new ItTreeMiner(filePath, threshold);
            miner.GenerateClosedItemSets();
            var itemSets = new ObservableCollection<ItemTidSet<string, RepeatableTid>>(miner.GetMinedItemSets());
            MinedItemSets = itemSets;
        }
    }
}
