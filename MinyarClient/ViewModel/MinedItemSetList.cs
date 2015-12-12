using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FP.DAL.DAO;

namespace MinyarClient.ViewModel {
    public class MinedItemSetList : Livet.ViewModel {

        private ObservableCollection<ItemSet> _ItemSets;

        public ObservableCollection<ItemSet> ItemSets {
            get { return _ItemSets; }
            set {
                _ItemSets = value;
                RaisePropertyChanged();
            }
        }

        public MinedItemSetList() {
            ItemSets = new ObservableCollection<ItemSet>();
        }
    }
}
