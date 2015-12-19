using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using MinyarClient.View;
using MinyarClient.ViewModel;

namespace MinyarClient {
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
        }

        private void miningButton_Click(object sender, RoutedEventArgs e) {
            var vm = DataContext as MainViewModel;
            vm.StartMining();
            var itemSetsView = CollectionViewSource.GetDefaultView(vm.Model.MinedItemSets);
            itemSetsView.GroupDescriptions.Add(new PropertyGroupDescription("ItemCount"));
        }

        private void detailButton_Click(object sender, RoutedEventArgs e) {
            var vm = DataContext as MainViewModel;
            vm.OpenDetailWindow(dataGrid.SelectedItem);
        }
    }
}
