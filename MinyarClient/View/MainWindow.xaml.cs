using System.Collections.Generic;
using System.Windows;
using System.Windows.Data;
using System.IO;
using System.Linq;
using Minyar.Charm;
using MinyarClient.View;
using MinyarClient.ViewModel;
using Newtonsoft.Json;

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

        private void truthButton_Click(object sender, RoutedEventArgs e) {
            var selectedItems = dataGrid.SelectedItems.Cast<ItemTidSet<string, RepeatableTid>>().ToList();
            var path = Path.Combine("..", "..", "..", "data", "GroundTruth.json");
            Directory.CreateDirectory(Path.Combine(path, ".."));
            using (var writer = new StreamWriter(path)) {
                writer.Write(JsonConvert.SerializeObject(selectedItems.Select(i => i.Items)));
            }
        }

        private void reviewButton_Click(object sender, RoutedEventArgs e) {
            var window = new ReviewWindow();
            window.Show();
        }
    }
}
