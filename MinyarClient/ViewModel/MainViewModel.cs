using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Livet.EventListeners;
using Livet.Messaging.IO;
using Microsoft.Win32;
using Minyar.Charm;
using MinyarClient.Model;
using MinyarClient.View;

namespace MinyarClient.ViewModel {
    public class MainViewModel : Livet.ViewModel {
        public MainModel Model { get; set; }

        public MainViewModel() {
            Model = new MainModel();
            Threshold = "Auto";
            FilePath = "Please open file...";
            var listener = new PropertyChangedEventListener(Model, 
                (sender, args) => RaisePropertyChanged(args.PropertyName));
            CompositeDisposable.Add(listener);
        }

        #region FilePath
        private string _FilePath;

        public string FilePath {
            get { return _FilePath; }
            private set {
                _FilePath = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region FileSelected

        private bool _FileSelected;

        public bool FileSelected {
            get { return _FileSelected; }
            private set {
                _FileSelected = value;
                RaisePropertyChanged();
            }
        }
        #endregion

        #region ItemsetCount
        private int _ItemsetCount;

        public string ItemsetCount {
            get { return _ItemsetCount.ToString(); }
            private set {
                _ItemsetCount = int.Parse(value);
                RaisePropertyChanged();
            }
        }
        #endregion

        #region Threshold
        private string _Threshold;

        public string Threshold {
            get { return _Threshold; }
            set {
                _Threshold = value;
                RaisePropertyChanged();
            }
        }
        #endregion


        public void SelectFile(OpeningFileSelectionMessage m) {
            if (m.Response == null) return;
            FilePath = m.Response[0];
            FileSelected = true;
            Threshold = "Auto";
            SetItemsetCount();
        }

        public void StartMining() {
            int threshold;
            int.TryParse(Threshold, out threshold);
            if (Threshold == "Auto") threshold = _ItemsetCount / 10 * 4;
            Threshold = threshold.ToString();
            //Model.StartMiningUsingFpGrowth(_FilePath, threshold);
            Model.StartMiningUsingCharm(_FilePath, threshold);
       }

        public void OpenDetailWindow(object selectedItem) {
            var item = selectedItem as ItemTidSet<string, RepeatableTid>;
            var detailWindow = new DetailWindow(item);
            detailWindow.Show();
        }

        private void SetItemsetCount() {
            using (var reader = new StreamReader(FilePath)) {
                var content = reader.ReadToEnd();
                ItemsetCount = content.Count(c => c == '\n').ToString();
            }
        }
    }
}
