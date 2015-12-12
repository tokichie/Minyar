using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace MinyarClient.Model {
    public class MainModel {
        public string FilePath { get; private set; }
        public bool FileSelected { get; private set; }
        public string Threshold { get; set; }

        public MainModel() {
            Threshold = "200";
        }

        public void SelectFile() {
            var dialog = new OpenFileDialog();
            dialog.Multiselect = false;
            dialog.Filter = "Text File|*.txt|All Files|*.*";
            var res = dialog.ShowDialog();
            if (res == true) {
                FilePath = dialog.FileName;
                FileSelected = true;
            }
        }
    }
}
