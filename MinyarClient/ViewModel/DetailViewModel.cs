using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MinyarClient.Model;

namespace MinyarClient.ViewModel {
    public class DetailViewModel : Livet.ViewModel {
        public DetailModel Model;

        public DetailViewModel() {
            Model = new DetailModel();
        }
    }
}
