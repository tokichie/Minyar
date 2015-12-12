using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using FP.DAL.DAO;
using Microsoft.Win32;
using Minyar;
using MinyarClient.ViewModel;

namespace MinyarClient.Model {
    public class MainModel : Livet.NotificationObject {

        public MainModel() {
            MinedItemSets = new ObservableCollection<ItemSet>();
        }

        private ObservableCollection<ItemSet> _MinedItemSets;

        public ObservableCollection<ItemSet> MinedItemSets {
            get { return _MinedItemSets; }
            set {
                _MinedItemSets = value;
                RaisePropertyChanged();
            }
        }  

        public void StartMining(string filePath, int threshold) {
            var miner = new FPGrowthMiner(filePath, threshold);
            miner.GenerateFrequentItemsets();
            var itemSets = new ObservableCollection<ItemSet>(miner.GetAllMinedItemSets());
            MinedItemSets = itemSets;
        }
    }
}
