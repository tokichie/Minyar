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
using MinyarClient.Model;

namespace MinyarClient.ViewModel {
    public class MainViewModel : Livet.ViewModel {
        public MainModel Model { get; set; }

        public MainViewModel() {
            Model = new MainModel();
            Threshold = "Auto";
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

        public string Threshold { get; set; }

        public void SelectFile(OpeningFileSelectionMessage m) {
            if (m.Response == null) return;
            FilePath = m.Response[0];
            FileSelected = true;
            SetItemsetCount();
        }

        public void StartMining() {
            int threshold;
            int.TryParse(Threshold, out threshold);
            if (Threshold == "Auto") threshold = _ItemsetCount/10;
            Model.StartMining(_FilePath, threshold);
       }

        private void SetItemsetCount() {
            using (var reader = new StreamReader(FilePath)) {
                var content = reader.ReadToEnd();
                ItemsetCount = content.Count(c => c == '\n').ToString();
            }
        }
    }
}
