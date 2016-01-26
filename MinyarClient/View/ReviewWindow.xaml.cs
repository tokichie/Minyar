using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Mapping;
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
using System.Windows.Shapes;
using MinyarClient.ViewModel;

namespace MinyarClient.View {
    /// <summary>
    /// ReviewWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class ReviewWindow : Window {
        public ReviewWindow() {
            InitializeComponent();
        }

        private void loadButton_Click(object sender, RoutedEventArgs e) {
            var vm = DataContext as ReviewViewModel;
            vm.LoadComments();
        }

        private void addButton_Click(object sender, RoutedEventArgs e) {
            var vm = DataContext as ReviewViewModel;
            vm.AddComment();
        }

        private void removeButton_Click(object sender, RoutedEventArgs e) {
            var vm = DataContext as ReviewViewModel;
            vm.RemoveComment();
        }

        private void rightButton_Click(object sender, RoutedEventArgs e) {
            var vm = DataContext as ReviewViewModel;
            vm.Next();
        }

        private void leftButton_Click(object sender, RoutedEventArgs e) {
            var vm = DataContext as ReviewViewModel;
            vm.Prev();
        }

        private void saveButton_Click(object sender, RoutedEventArgs e) {
            var vm = DataContext as ReviewViewModel;
            vm.Save();
            MessageBox.Show("Selected comments are saved.");
        }

        private void Window_KeyDown(object sender, KeyEventArgs e) {
            switch (e.Key) {
                case Key.A:
                    leftButton_Click(null, null);
                    break;
                case Key.S:
                    addButton_Click(null, null);
                    break;
                case Key.D:
                    rightButton_Click(null, null);
                    break;
                case Key.R:
                    removeButton_Click(null, null);
                    break;
                case Key.Left:
                    leftButton_Click(null, null);
                    break;
                case Key.Right:
                    rightButton_Click(null, null);
                    break;
            }
        }
    }
}
