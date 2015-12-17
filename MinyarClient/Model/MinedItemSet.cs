using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinyarClient.Model{
    public class MinedItemSet<T> : Livet.NotificationObject {
        private ObservableCollection<T> itemSets;

        public ObservableCollection<T> ItemSets {
            get { return itemSets; }
            set {
                itemSets = value;
                RaisePropertyChanged();
            }
        }

        public MinedItemSet(IMinedItemSet<T> sets) {
            ItemSets = new ObservableCollection<T>(sets);
        }
    }
}
