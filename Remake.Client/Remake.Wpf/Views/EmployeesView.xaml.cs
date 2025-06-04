using Remake.Wpf.Services;
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

namespace Remake.Wpf.Views
{
    /// <summary>
    /// Логика взаимодействия для EmployeesView.xaml
    /// </summary>
    public partial class EmployeesView : UserControl
    {
        public EmployeesView()
        {
            InitializeComponent();
        }

        private void CollectionViewSource_Filter(object sender, FilterEventArgs e)
        {
            if (e.Item is Employee employee)
            {
                var filterText = FilterTextBox.Text ?? "";
                e.Accepted = string.IsNullOrWhiteSpace(filterText)
                             || employee.FirstName.StartsWith(filterText)
                             || employee.LastName.StartsWith(filterText);
            }
        }

        private void FilterTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var viewSource = (CollectionViewSource)Resources["EmployeesCollectionView"];
            viewSource?.View?.Refresh();
        }
    }
}
