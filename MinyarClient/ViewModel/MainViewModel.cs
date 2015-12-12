using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using MinyarClient.Model;

namespace MinyarClient.ViewModel {
    public class MainViewModel : ViewModelBase {
        public MainModel MyMainModel { get; set; }

        public MainViewModel() {
            MyMainModel = new MainModel();
        }

        public void SelectFile() {
            MyMainModel.SelectFile();
            RaisePropertyChanged("MyMainModel");
        }

        public void StartMining() {
            
            RaisePropertyChanged("MyMainModel");
        }
    }
}
